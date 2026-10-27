using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PROYECTO
{
    public class MedicoRepositorio:IRepositorio<Medico>
    {
        private ConexionBD conexion = new ConexionBD();

        private const string SqlSeleccion =
      "SELECT m.id_Medico, m.Nombre, m.Apellido, m.id_especialidad, " +
      "       u.id_usuario, u.username, u.password " +
      "FROM Medico m INNER JOIN Usuario u ON u.id_Medico = m.id_Medico ";


        public Medico Buscarr(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                Medico m = null;
                using (SqlCommand cmd = new SqlCommand(SqlSeleccion + "WHERE m.id_Medico = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read()) m = Construir(r);
                    }
                }
                if (m != null) CargarContactos(cn, m);
                return m;
            }
            finally { conexion.CerrarConxion(); }
        }

        public void Guardar(Medico m)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Medico (Nombre, Apellido,id_especialidad) " +
                    "VALUES (@n, @a, @e); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", m.Nombre);
                    cmd.Parameters.AddWithValue("@a", m.Apellido);
                    cmd.Parameters.AddWithValue("@e", m.id_especialidad);

                    m.id_Medico = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (username, password, Rol, id_Medico) " +
                    "VALUES (@u, @p, 'Medico', @idp); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@u", m.Username);
                    cmd.Parameters.AddWithValue("@p", m.password);
                    cmd.Parameters.AddWithValue("@idp", m.id_Medico);
                    m.id_usuario = Convert.ToInt32(cmd.ExecuteScalar());
                }

                GuardarContactos(cn, tx, m);
                tx.Commit();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                tx.Rollback();
                throw new InvalidOperationException("Ese nombre de usuario ya existe.");
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally { conexion.CerrarConxion(); }
        }
        public void Actualizar(Medico m)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Medico SET Nombre = @n, Apellido = @a, id_especialidad = @e " +
                    "WHERE id_Medico = @id", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", m.Nombre);
                    cmd.Parameters.AddWithValue("@a", m.Apellido);
                    cmd.Parameters.AddWithValue("@id", m.id_Medico);
                    cmd.Parameters.AddWithValue("@e", m.id_especialidad);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Usuario SET username = @u, password = @p WHERE id_usuario = @idu", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@u", m.Username);
                    cmd.Parameters.AddWithValue("@p", m.password);
                    cmd.Parameters.AddWithValue("@idu", m.id_usuario);
                    cmd.ExecuteNonQuery();
                }

                // Se reemplazan los contactos: borrar los viejos e insertar los actuales
                Ejecutar(cn, tx, "DELETE FROM TelefonoMedico WHERE id_Medico = @id", m.id_Medico);
                Ejecutar(cn, tx, "DELETE FROM CorreoMedico WHERE id_Medico = @id", m.id_Medico);
                GuardarContactos(cn, tx, m);
                tx.Commit();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                tx.Rollback();
                throw new InvalidOperationException("Ese nombre de usuario ya existe.");
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally { conexion.CerrarConxion(); }
        }

        public void Eliminar(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                // Primero los hijos, al final el paciente (por las llaves foráneas)
                Ejecutar(cn, tx, "DELETE FROM TelefonoMedico WHERE id_Medico = @id", id);
                Ejecutar(cn, tx, "DELETE FROM CorreoMedico WHERE id_Medico = @id", id);
                Ejecutar(cn, tx, "DELETE FROM Usuario WHERE id_Medico = @id", id);
                Ejecutar(cn, tx, "DELETE FROM Medico WHERE id_Medico = @id", id);
                tx.Commit();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                tx.Rollback();
                throw new InvalidOperationException(
                    "No se puede eliminar: el Medico .");
            }
            catch
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                conexion.CerrarConxion();
            }
        }

        public List<Medico> ListarTodos()
        {
            List<Medico> lista = new List<Medico>();
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(SqlSeleccion + "ORDER BY m.Apellido, m.Nombre", cn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(Construir(r));
                }

                // El lector ya está cerrado: ahora se pueden cargar teléfonos y correos
                foreach (Medico m in lista) CargarContactos(cn, m);
            }
            finally
            {
                conexion.CerrarConxion();
            }
            return lista;
        }

        //Metodo privado que se opcupara en buscar y listar para l mantenimineto de estos metodos y evitar estar sobrescribiendo
        private Medico Construir(SqlDataReader r)
        {
            return new Medico(
        r.GetInt32(0), r.GetString(1), r.GetString(2),
        new List<string>(), new List<string>(),
        r.IsDBNull(3) ? 0 : r.GetInt32(3),      // id_especialidad (puede ser NULL en la BD)
        r.GetInt32(4), r.GetString(5), r.GetString(6));
        }

        //Metodo privado q se ocuparan en buscar y listar para evitar copiar las 2 consultas y correos en dos sitios
        private void CargarContactos(SqlConnection cn, Medico m)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT Telefono FROM TelefonoMedico WHERE id_Medico = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", m.id_Medico);
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) m.Telefono.Add(r.GetString(0));
            }

            using (SqlCommand cmd = new SqlCommand(
                "SELECT Correo FROM CorreoMedico WHERE id_Medico = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", m.id_Medico);
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) m.Correo.Add(r.GetString(0));
            }
        }

        //Metodo privado de buscar y actualizar para evitar copiar los bucles de insert n dos sitios
        private void GuardarContactos(SqlConnection cn, SqlTransaction tx, Medico m)
        {
            foreach (string tel in m.Telefono)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO TelefonoMedico (id_Medico, Telefono) VALUES (@id, @t)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", m.id_Medico);
                    cmd.Parameters.AddWithValue("@t", tel.Trim());
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (string correo in m.Correo)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO CorreoMedico (id_Medico, Correo) VALUES (@id, @c)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", m.id_Medico);
                    cmd.Parameters.AddWithValue("@c", correo.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Metodo privado q lo ocupa actualizar y liminar para evitar copiar 6 bloques d using sqlcomand casi identicos
        private void Ejecutar(SqlConnection cn, SqlTransaction tx, string sql, int id)
        {
            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

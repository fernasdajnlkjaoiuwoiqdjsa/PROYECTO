using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace PROYECTO
{
    public class PacienteRepositorio: IRepositorio<Paciente>
    {
        private ConexionBD conexion = new ConexionBD();

        private const string SqlSeleccion = "SELECT p.id_paciente, p.Nombre, p.Apellido, p.FechaNacimineto, " +
            "       u.id_usuario, u.username, u.password " +
            "FROM Pacientes p INNER JOIN Usuario u ON u.id_paciente = p.id_paciente ";


        
        public Paciente Buscarr(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                Paciente p = null;
                using (SqlCommand cmd = new SqlCommand(SqlSeleccion + "WHERE p.id_paciente = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read()) p = Construir(r);
                    }
                }
                if (p != null) CargarContactos(cn, p);
                return p;
            }
            finally { conexion.CerrarConxion(); }
        }

        public void Guardar(Paciente p)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Pacientes (Nombre, Apellido, FechaNacimineto) " +
                    "VALUES (@n, @a, @f); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", p.Nombre);
                    cmd.Parameters.AddWithValue("@a", p.Apellido);
                    cmd.Parameters.AddWithValue("@f", p.FechaNacimineto);
                    p.id_paciente = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (username, password, Rol, id_paciente) " +
                    "VALUES (@u, @p, 'Paciente', @idp); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@u", p.Username);
                    cmd.Parameters.AddWithValue("@p", p.password);
                    cmd.Parameters.AddWithValue("@idp", p.id_paciente);
                    p.id_usuario = Convert.ToInt32(cmd.ExecuteScalar());
                }

                GuardarContactos(cn, tx, p);
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
        public void Actualizar(Paciente p)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Pacientes SET Nombre = @n, Apellido = @a, FechaNacimineto = @f " +
                    "WHERE id_paciente = @id", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", p.Nombre);
                    cmd.Parameters.AddWithValue("@a", p.Apellido);
                    cmd.Parameters.AddWithValue("@f", p.FechaNacimineto);
                    cmd.Parameters.AddWithValue("@id", p.id_paciente);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Usuario SET username = @u, password = @p WHERE id_usuario = @idu", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@u", p.Username);
                    cmd.Parameters.AddWithValue("@p", p.password);
                    cmd.Parameters.AddWithValue("@idu", p.id_usuario);
                    cmd.ExecuteNonQuery();
                }

                // Se reemplazan los contactos: borrar los viejos e insertar los actuales
                Ejecutar(cn, tx, "DELETE FROM TelefonoPaciente WHERE id_paciente = @id", p.id_paciente);
                Ejecutar(cn, tx, "DELETE FROM Correopaciente WHERE id_paciente = @id", p.id_paciente);
                GuardarContactos(cn, tx, p);
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
                Ejecutar(cn, tx, "DELETE FROM TelefonoPaciente WHERE id_paciente = @id", id);
                Ejecutar(cn, tx, "DELETE FROM Correopaciente WHERE id_paciente = @id", id);
                Ejecutar(cn, tx, "DELETE FROM Usuario WHERE id_paciente = @id", id);
                Ejecutar(cn, tx, "DELETE FROM Pacientes WHERE id_paciente = @id", id);
                tx.Commit();
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                tx.Rollback();
                throw new InvalidOperationException(
                    "No se puede eliminar: el paciente tiene citas registradas.");
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

            public List<Paciente> ListarTodos()
        {
            List<Paciente> lista = new List<Paciente>();
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(SqlSeleccion + "ORDER BY p.Apellido, p.Nombre", cn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(Construir(r));
                }

                // El lector ya está cerrado: ahora se pueden cargar teléfonos y correos
                foreach (Paciente p in lista) CargarContactos(cn, p);
            }
            finally 
            { 
                conexion.CerrarConxion(); 
            }
            return lista;
        }

        //Metodo privado que se opcupara en buscar y listar para l mantenimineto de estos metodos y evitar estar sobrescribiendo
        private Paciente Construir(SqlDataReader r)
        {
            return new Paciente(
                r.GetInt32(0), r.GetString(1), r.GetString(2),
                new List<string>(), new List<string>(),
                r.IsDBNull(3) ? DateTime.MinValue : r.GetDateTime(3),
                r.GetInt32(4), r.GetString(5), r.GetString(6));
        }

        //Metodo privado q se ocuparan en buscar y listar para evitar copiar las 2 consultas y correos en dos sitios
        private void CargarContactos(SqlConnection cn, Paciente p)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT Telefono FROM TelefonoPaciente WHERE id_paciente = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", p.id_paciente);
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) p.Telefono.Add(r.GetString(0));
            }

            using (SqlCommand cmd = new SqlCommand(
                "SELECT Correo FROM Correopaciente WHERE id_paciente = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", p.id_paciente);
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read()) p.Correo.Add(r.GetString(0));
            }
        }

        //Metodo privado de buscar y actualizar para evitar copiar los bucles de insert n dos sitios
        private void GuardarContactos(SqlConnection cn, SqlTransaction tx, Paciente p)
        {
            foreach (string tel in p.Telefono)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO TelefonoPaciente (id_paciente, Telefono) VALUES (@id, @t)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", p.id_paciente);
                    cmd.Parameters.AddWithValue("@t", tel.Trim());
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (string correo in p.Correo)
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Correopaciente (id_paciente, Correo) VALUES (@id, @c)", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", p.id_paciente);
                    cmd.Parameters.AddWithValue("@c", correo.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Mtodo privado q lo ocupa actualizar y liminar para evitar copiar 6 bloques d using sqlcomand casi identicos
        private void Ejecutar(SqlConnection cn, SqlTransaction tx, string sql, int id)
        {
            using (SqlCommand cmd = new SqlCommand(sql, cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // TODOS ESTOS METODOS PRIVADOS LOS PONGO POR MANTENIMIENTO DEL SISTEEMA
    }
    
}

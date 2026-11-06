using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;
using Capa_Entidad;


namespace PROYECTO
{
    // Repositorio de médicos: hace el CRUD de la tabla Medico y también de su cuenta (tabla
    // Usuario) y de sus teléfonos y correos (TelefonoMedico y CorreoMedico). Implementa
    // IRepositorio<Medico>, así que el administrador lo usa sin saber cómo se guarda. Como un
    // médico está repartido en varias tablas, Guardar, Actualizar y Eliminar usan transacciones:
    // o se guarda todo o no se guarda nada. Aquí solo hay SQL; las reglas las pone
    // Administrador.GestionarMedicos.
    public class MedicoRepositorio : IRepositorio<Medico>
    {
        // Objeto que abre y cierra la conexión a la base de datos, igual que en los otros repositorios.
        private ConexionBD conexion = new ConexionBD();

        // Consulta base para leer médicos. Hace INNER JOIN con Usuario para traer en una sola
        // consulta los datos del médico y los de su cuenta (usuario y contraseña). Los teléfonos
        // y correos no vienen aquí porque son varios por médico y se cargan aparte con
        // CargarContactos. Cada método le concatena su WHERE u ORDER BY.
        private const string SqlSeleccion =
      "SELECT m.id_Medico, m.Nombre, m.Apellido, m.id_especialidad, " +
      "       u.id_usuario, u.username, u.password " +
      "FROM Medico m INNER JOIN Usuario u ON u.id_Medico = m.id_Medico ";


        // Primero lee el médico con la consulta base; después, si lo encontró, le carga sus
        // teléfonos y correos. Devuelve el médico o null si no existe. Cierro el lector antes de
        // cargar los contactos porque no se pueden tener dos lectores abiertos en la misma conexión.
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

        // Registra un médico nuevo. Son tres pasos: (1) insertar en Medico y guardar el id que
        // generó SQL, (2) insertar su cuenta en Usuario con rol "Medico" y ese id, y (3) guardar
        // sus teléfonos y correos. Todo va dentro de una transacción: si algo falla hago Rollback
        // y no queda un médico a medias (por ejemplo, médico sin cuenta). Si el nombre de usuario
        // ya existe, SQL lanza error 2627 o 2601 y lo cambio por un mensaje claro.
        public void Guardar(Medico m)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                // Paso 1: datos personales del médico
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Medico (Nombre, Apellido,id_especialidad) " +
                    "VALUES (@n, @a, @e); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", m.Nombre);
                    cmd.Parameters.AddWithValue("@a", m.Apellido);
                    cmd.Parameters.AddWithValue("@e", m.id_especialidad);

                    m.id_Medico = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Paso 2: su cuenta de login, enlazada al médico con id_Medico
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (username, password, Rol, id_Medico) " +
                    "VALUES (@u, @p, 'Medico', @idp); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@u", m.Username);
                    cmd.Parameters.AddWithValue("@p", m.password);
                    cmd.Parameters.AddWithValue("@idp", m.id_Medico);
                    m.id_usuario = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Paso 3: teléfonos y correos, y confirmo toda la transacción
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

        // Actualiza un médico existente: sus datos personales y su especialidad, su cuenta
        // (usuario y contraseña) y sus contactos. Para los contactos no comparo cuáles cambiaron:
        // borro todos los teléfonos y correos viejos y vuelvo a insertar los actuales, que es más
        // simple. Todo en una transacción para que no quede a medias.
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

        // Elimina un médico por su id. Hay que borrar en orden, primero las tablas hijas (teléfonos,
        // correos y cuenta de usuario) y al final el médico, porque si no las llaves foráneas lo
        // impiden. Va en una transacción. Si el médico ya tiene citas, SQL lanza el error 547 y
        // lo cambio por un mensaje para el usuario.
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

        // Devuelve todos los médicos ordenados por apellido y nombre. Llena las tablas del
        // administrador y los ComboBox de médicos al agendar. Primero leo todos los médicos y,
        // cuando el lector ya está cerrado, le cargo a cada uno sus teléfonos y correos.
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

        // Convierte una fila del lector en un objeto Medico con el constructor completo. Las
        // listas de teléfonos y correos las dejo vacías porque se llenan después en
        // CargarContactos. La especialidad puede ser NULL en la base, y en ese caso uso 0.
        private Medico Construir(SqlDataReader r)
        {
            return new Medico(
        r.GetInt32(0), r.GetString(1), r.GetString(2),
        new List<string>(), new List<string>(),
        r.IsDBNull(3) ? 0 : r.GetInt32(3),      // id_especialidad (puede ser NULL en la BD)
        r.GetInt32(4), r.GetString(5), r.GetString(6));
        }

        // Lee los teléfonos y correos del médico desde sus tablas y los agrega a las listas del
        // objeto. Hace dos consultas separadas porque un médico puede tener varios de cada uno.
        // Recibe la conexión ya abierta para reutilizarla y no abrir otra.
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

        // Guarda cada teléfono y cada correo del médico, uno por fila, en sus tablas. Recibe la
        // transacción (tx) para que estos inserts formen parte de la misma que Guardar o Actualizar
        // y, si algo falla, se deshagan también. Uso Trim() para quitar espacios sobrantes.
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

        // Ayudante para ejecutar un DELETE sencillo que solo recibe un @id, dentro de la
        // transacción. Lo hice para no repetir las mismas líneas en cada borrado de Actualizar y
        // Eliminar.
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
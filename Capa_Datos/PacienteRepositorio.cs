using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Capa_Entidad;
using Capa_Datos;


namespace PROYECTO
{
    // Repositorio de pacientes: hace el CRUD de la tabla Pacientes y también de su cuenta (tabla
    // Usuario) y de sus teléfonos y correos (TelefonoPaciente y Correopaciente). Implementa
    // IRepositorio<Paciente>, así que el administrador lo usa sin saber cómo se guarda. Como un
    // paciente está repartido en varias tablas, Guardar, Actualizar y Eliminar usan transacciones:
    // o se guarda todo o no se guarda nada. Es casi idéntico a MedicoRepositorio, con otras
    // tablas. Aquí solo hay SQL; las reglas las pone Administrador.GestionarPacintes.
    public class PacienteRepositorio : IRepositorio<Paciente>
    {
        // Objeto que abre y cierra la conexión a la base de datos, igual que en los otros repositorios.
        private ConexionBD conexion = new ConexionBD();

        // Consulta base para leer pacientes. Hace INNER JOIN con Usuario para traer en una sola
        // consulta los datos del paciente y los de su cuenta (usuario y contraseña). Los teléfonos
        // y correos no vienen aquí porque son varios por paciente y se cargan aparte con
        // CargarContactos. Cada método le concatena su WHERE u ORDER BY.
        private const string SqlSeleccion = "SELECT p.id_paciente, p.Nombre, p.Apellido, p.FechaNacimineto, " +
            "       u.id_usuario, u.username, u.password " +
            "FROM Pacientes p INNER JOIN Usuario u ON u.id_paciente = p.id_paciente ";



        // Busca un paciente por su id (el nombre lleva doble r porque así quedó en la interfaz).
        // Primero lee el paciente con la consulta base; después, si lo encontró, le carga sus
        // teléfonos y correos. Devuelve el paciente o null si no existe. Cierro el lector antes de
        // cargar los contactos porque no se pueden tener dos lectores abiertos en la misma conexión.
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

        // Registra un paciente nuevo. Son tres pasos: (1) insertar en Pacientes y guardar el id
        // que generó SQL, (2) insertar su cuenta en Usuario con rol "Paciente" y ese id, y (3)
        // guardar sus teléfonos y correos. Todo va dentro de una transacción: si algo falla hago
        // Rollback y no queda un paciente sin cuenta. Si el nombre de usuario ya existe, SQL lanza
        // error 2627 o 2601 y lo cambio por un mensaje claro.
        public void Guardar(Paciente p)
        {
            SqlConnection cn = conexion.AbrirConexion();
            SqlTransaction tx = cn.BeginTransaction();
            try
            {
                // Paso 1: datos personales del paciente
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Pacientes (Nombre, Apellido, FechaNacimineto) " +
                    "VALUES (@n, @a, @f); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@n", p.Nombre);
                    cmd.Parameters.AddWithValue("@a", p.Apellido);
                    cmd.Parameters.AddWithValue("@f", p.FechaNacimineto);
                    p.id_paciente = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Paso 2: su cuenta de login, enlazada al paciente con id_paciente
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (username, password, Rol, id_paciente) " +
                    "VALUES (@u, @p, 'Paciente', @idp); SELECT SCOPE_IDENTITY();", cn, tx))
                {
                    cmd.Parameters.AddWithValue("@u", p.Username);
                    cmd.Parameters.AddWithValue("@p", p.password);
                    cmd.Parameters.AddWithValue("@idp", p.id_paciente);
                    p.id_usuario = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Paso 3: teléfonos y correos, y confirmo toda la transacción
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
        // Actualiza un paciente existente: sus datos personales, su cuenta (usuario y contraseña)
        // y sus contactos. Para los contactos no comparo cuáles cambiaron: borro todos los
        // teléfonos y correos viejos y vuelvo a insertar los actuales, que es más simple. Todo en
        // una transacción para que no quede a medias.
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

        // Elimina un paciente por su id. Hay que borrar en orden, primero las tablas hijas
        // (teléfonos, correos y cuenta de usuario) y al final el paciente, porque si no las llaves
        // foráneas lo impiden. Va en una transacción. Si el paciente ya tiene citas, SQL lanza el
        // error 547 y lo cambio por un mensaje para el usuario.
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

        // Devuelve todos los pacientes ordenados por apellido y nombre. Llena las tablas del
        // administrador y los selectores de pacientes (por ejemplo en los reportes). Primero leo
        // todos los pacientes y, cuando el lector ya está cerrado, le cargo a cada uno sus
        // teléfonos y correos.
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

        // Convierte una fila del lector en un objeto Paciente con el constructor completo. Las
        // listas de teléfonos y correos las dejo vacías porque se llenan después en
        // CargarContactos. La fecha de nacimiento puede ser NULL en la base, y en ese caso uso
        // DateTime.MinValue para que no se caiga.
        private Paciente Construir(SqlDataReader r)
        {
            return new Paciente(
                r.GetInt32(0), r.GetString(1), r.GetString(2),
                new List<string>(), new List<string>(),
                r.IsDBNull(3) ? DateTime.MinValue : r.GetDateTime(3),
                r.GetInt32(4), r.GetString(5), r.GetString(6));
        }

        // Lee los teléfonos y correos del paciente desde sus tablas y los agrega a las listas del
        // objeto. Hace dos consultas separadas porque un paciente puede tener varios de cada uno.
        // Recibe la conexión ya abierta para reutilizarla y no abrir otra.
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

        // Guarda cada teléfono y cada correo del paciente, uno por fila, en sus tablas. Recibe la
        // transacción (tx) para que estos inserts formen parte de la misma que Guardar o Actualizar
        // y, si algo falla, se deshagan también. Uso Trim() para quitar espacios sobrantes.
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

        // TODOS ESTOS METODOS PRIVADOS LOS PONGO POR MANTENIMIENTO DEL SISTEEMA
    }

}
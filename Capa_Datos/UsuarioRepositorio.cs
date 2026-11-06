using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using PROYECTO;

namespace Capa_Datos
{
    // Repositorio de usuarios: maneja la tabla Usuario, que es la de las cuentas de login de los
    // tres roles. Implementa IRepositorio<Usuario>. Es el que usa el login (ServicioAutenticacion)
    // para buscar a la persona por su username, y el administrador para cambiar contraseñas y
    // activar o desactivar cuentas. Lo especial de esta clase es que, al leer una cuenta, devuelve
    // el objeto de la subclase correcta (Paciente, Medico o Administrador) según su rol.
    public class UsuarioRepositorio : IRepositorio<Usuario>
    {
        // Objeto que abre y cierra la conexión a la base de datos, igual que en los otros repositorios.
        private ConexionBD conexion = new ConexionBD();

        // Consulta base para leer cuentas. Trae id_paciente e id_Medico porque son los que dicen,
        // junto con el rol, a qué persona pertenece la cuenta. Un administrador tiene ambos en NULL.
        private const string SqlSeleccion =
            "SELECT id_usuario, username, password, Rol, id_paciente, id_Medico,activo FROM Usuario ";




        //Fila cruda de la tabla usuarios que lee primero y luego decide q sub clase arma

        // Clase interna y privada que guarda una fila de la tabla tal cual viene, sin convertirla
        // todavía en Paciente, Medico o Administrador. La necesito porque primero tengo que leer
        // todas las filas y cerrar la conexión, y recién después pedirle a los otros repositorios
        // que armen cada objeto completo. idPaciente e idMedico son int? porque pueden ser NULL.
        private class FilaUsuario
        {
            public bool Activo;
            public int IdUsuario;
            public string Username, password, Rol;
            public int? idPaciente, idMedico;

        }

        // Busca una cuenta por su nombre de usuario. Es el método clave del login: el servicio de
        // autenticación lo llama con lo que escribieron y luego compara la contraseña con
        // IniciarSesion. Si el texto viene vacío devuelvo null sin ir a la base, y le quito los
        // espacios con Trim(). Devuelve el usuario ya armado (con su subclase) o null si no existe.
        public Usuario BuscarPorUsername(string usrname)
        {
            if (string.IsNullOrWhiteSpace(usrname)) return null;
            List<FilaUsuario> filas = LeerFilas("WHERE username = @v", usrname.Trim());
            return filas.Count == 0 ? null : Construir(filas[0]);
        }

        // Busca una cuenta por su id (el nombre lleva doble r porque así quedó en la interfaz).
        // Devuelve el usuario de la subclase que corresponda o null si no existe.
        public Usuario Buscarr(int id)
        {
            List<FilaUsuario> filas = LeerFilas("WHERE id_usuario = @v", id);
            return filas.Count == 0 ? null : Construir(filas[0]);
        }

        // Inserta una cuenta nueva, pero solo si es de administrador. Los pacientes y médicos
        // también necesitan su fila en su propia tabla, así que se registran desde
        // PacienteRepositorio y MedicoRepositorio, que lo hacen todo en una transacción; si se
        // intentara por aquí quedaría una cuenta sin persona. Uso SCOPE_IDENTITY() para guardar el
        // id generado. Si el username ya existe (error 2627 o 2601), lanzo un mensaje claro.
        public void Guardar(Usuario u)
        {
            if (u.Rol != "Administrador")
                throw new InvalidOperationException(
                    "Los pacientes y médicos se registran desde su propia gestión.");

            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Usuario (username, password, Rol) " +
                    "VALUES (@u, @p, 'Administrador'); SELECT SCOPE_IDENTITY();", cn))
                {
                    cmd.Parameters.AddWithValue("@u", u.Username);
                    cmd.Parameters.AddWithValue("@p", u.password);
                    u.id_usuario = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("Ese nombre de usuario ya existe.");
            }
            finally { conexion.CerrarConxion(); }
        }

        // Actualiza username, contraseña y estado (activo) de una cuenta, sea del rol que sea. Es
        // lo que usan Administrador.GestionarUsuarios (cambiar usuario o contraseña) y
        // CambiarEstadoCuenta (activar o desactivar). Si el nuevo username ya lo tiene otra
        // cuenta, SQL lanza el error 2627 o 2601 y lo cambio por un mensaje claro.
        public void Actualizar(Usuario u)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Usuario SET username = @u, password = @p, activo = @a WHERE id_usuario = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@u", u.Username);
                    cmd.Parameters.AddWithValue("@p", u.password);
                    cmd.Parameters.AddWithValue("@id", u.id_usuario);
                    cmd.Parameters.AddWithValue("@a", u.Activo);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("Ese nombre de usuario ya existe.");
            }
            finally { conexion.CerrarConxion(); }
        }

        // Elimina una cuenta, pero solo si es de administrador (lo fuerzo en el WHERE con
        // Rol = 'Administrador'). Para pacientes y médicos hay que borrar también sus datos
        // personales, y eso lo hacen sus propios repositorios. ExecuteNonQuery devuelve cuántas
        // filas se borraron: si es 0 es que no era administrador o no existía, y aviso con un
        // mensaje.
        public void Eliminar(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Usuario WHERE id_usuario = @id AND Rol = 'Administrador'", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException(
                            "Solo se pueden eliminar administradores desde aquí. " +
                            "Pacientes y médicos se eliminan desde su gestión.");
                }
            }
            finally { conexion.CerrarConxion(); }
        }

        // Devuelve todas las cuentas ordenadas por username, ya convertidas a su subclase. Es la
        // que llena el formulario de gestionar usuarios. La lista es de Usuario pero cada
        // elemento es en realidad un Paciente, Medico o Administrador, y eso es polimorfismo.
        public List<Usuario> ListarTodos()
        {
            List<FilaUsuario> filas = LeerFilas("ORDER BY username", null);
            List<Usuario> lista = new List<Usuario>();
            foreach (FilaUsuario f in filas) lista.Add(Construir(f));   // lista polimórfica
            return lista;
        }

        // Método privado que ejecuta la consulta base más la condición que le pase y devuelve las
        // filas crudas (sin convertir). El valor del parámetro @v es opcional (object, para que
        // sirva con texto o con número). Cierro la conexión en el finally ANTES de construir los
        // objetos, porque Construir llama a otros repositorios que abren su propia conexión y no
        // quiero dejar conexiones abiertas de más.
        private List<FilaUsuario> LeerFilas(string condicion, object valor)
        {
            List<FilaUsuario> filas = new List<FilaUsuario>();
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(SqlSeleccion + condicion, cn))
                {
                    if (valor != null) cmd.Parameters.AddWithValue("@v", valor);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            filas.Add(new FilaUsuario
                            {
                                IdUsuario = r.GetInt32(0),
                                Username = r.GetString(1),
                                password = r.GetString(2),
                                Rol = r.GetString(3),
                                // Si la columna es NULL, guardo null en vez de fallar
                                idPaciente = r.IsDBNull(4) ? (int?)null : r.GetInt32(4),
                                idMedico = r.IsDBNull(5) ? (int?)null : r.GetInt32(5),
                                Activo = r.GetBoolean(6)



                            });
                    }
                }
            }
            finally { conexion.CerrarConxion(); }   // se cierra ANTES de delegar en otros repositorios
            return filas;
        }

        // Convierte una fila cruda en el objeto de la subclase que le corresponde según su rol:
        // si es Paciente se lo pido a PacienteRepositorio, si es Medico a MedicoRepositorio, y si
        // no (administrador) lo creo aquí. Así el login recibe un objeto completo, con nombre,
        // contactos y su id de paciente o médico. Al final le pongo a todos el id de usuario, la
        // contraseña y el estado Activo que vienen de la tabla Usuario, para que se conserven al
        // actualizar y para que la cuenta desactivada se refleje al leer.
        private Usuario Construir(FilaUsuario f)
        {
            Usuario u;
            switch (f.Rol)
            {
                case "Paciente": u = new PacienteRepositorio().Buscarr(f.idPaciente.Value); break;
                case "Medico": u = new MedicoRepositorio().Buscarr(f.idMedico.Value); break;
                default: u = new Administrador(f.IdUsuario, f.Username, f.password); break;
            }
            if (u != null)
            {
                u.id_usuario = f.IdUsuario;
                u.password = f.password;     // Actualizar escribe la contraseña: no debe quedar nula
                u.Activo = f.Activo;
            }
            return u;
        }

    }
}
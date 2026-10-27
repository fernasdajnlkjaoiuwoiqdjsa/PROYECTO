using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PROYECTO
{
    public class UsuarioRepositorio:IRepositorio<Usuario>
    {
        private ConexionBD conexion = new ConexionBD();

        private const string SqlSeleccion =
            "SELECT id_usuario, username, password, Rol, id_paciente, id_Medico,activo FROM Usuario ";




        //Fila cruda de la tabla usuarios que lee primero y luego decide q sub clase arma

        private class FilaUsuario
        {
            public bool Activo;
            public int IdUsuario;
            public string Username,password,Rol;
            public int? idPaciente, idMedico;

        }

        public Usuario BuscarPorUsername(string usrname)
        {
            if (string.IsNullOrWhiteSpace(usrname)) return null;
            List<FilaUsuario> filas = LeerFilas("WHERE username = @v", usrname.Trim());
            return filas.Count == 0 ? null : Construir(filas[0]);
        }
        public Usuario Buscarr(int id)
        {
            List<FilaUsuario> filas = LeerFilas("WHERE id_usuario = @v", id);
            return filas.Count == 0 ? null : Construir(filas[0]);
        }

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

        public List<Usuario> ListarTodos()
        {
            List<FilaUsuario> filas = LeerFilas("ORDER BY username", null);
            List<Usuario> lista = new List<Usuario>();
            foreach (FilaUsuario f in filas) lista.Add(Construir(f));   // lista polimórfica
            return lista;
        }
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

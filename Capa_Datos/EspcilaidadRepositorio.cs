using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capa_Entidad;
using Capoa_Entidad;

namespace Capa_Datos
{
    // Repositorio de especialidades: es la clase que hace el CRUD de la tabla Especialidad
    // (Pediatría, Cardiología, etc.). Implementa IRepositorio<Especialidad>, así que cumple el
    // mismo contrato que los demás repositorios y el administrador la usa sin saber cómo se
    // guarda (abstracción). Aquí solo hay SQL; las reglas las pone Administrador.GestionarEspecialidad.
    // (El nombre de la clase lleva typo, "Espcilaidad", y lo dejo igual porque otros archivos lo usan.)
    public class EspcilaidadRepositorio : IRepositorio<Especialidad>
    {
        // Objeto que abre y cierra la conexión a la base de datos, igual que en los otros repositorios.
        private ConexionBD conexion = new ConexionBD();

        // Inserta una especialidad nueva. Uso SCOPE_IDENTITY() para que SQL me devuelva el id que
        // acaba de generar y se lo asigno a e.id_especialidad, así el objeto queda con su id real.
        // Uso parámetro (@nombre) y no pego el texto en el SQL para evitar inyección SQL. El
        // finally cierra la conexión aunque falle algo.
        public void Guardar(Especialidad e)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                string sql = "INSERT INTO Especialidad (NombreEspecialidad) VALUES (@nombre); " +
                            "SELECT SCOPE_IDENTITY();";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@nombre", e.NombreEspecialidad);
                    e.id_especialidad = Convert.ToInt32(cmd.ExecuteScalar());   // id que generó la BD
                }
            }
            finally
            {
                conexion.CerrarConxion();
            }
        }

        // Busca una especialidad por su id (el nombre lleva doble r porque así quedó en la
        // interfaz). Devuelve la especialidad si la encuentra o null si no existe. Sirve, por
        // ejemplo, para mostrar el nombre de la especialidad de un médico.
        public Especialidad Buscarr(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();

            try
            {
                string sql = "SELECT id_especialidad, NombreEspecialidad " +
                             "FROM Especialidad WHERE id_especialidad = @id";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        // Si hay una fila la convierto en objeto, si no devuelvo null
                        if (r.Read())
                            return new Especialidad(r.GetInt32(0), r.GetString(1));
                        return null;
                    }
                }
            }
            finally
            {
                conexion.CerrarConxion();

            }
        }

        // Cambia el nombre de una especialidad que ya existe, identificándola por su id.
        // Es lo que usa el formulario de gestionar especialidades al editar.
        public void Actualizar(Especialidad e)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                string sql = "UPDATE Especialidad SET NombreEspecialidad = @nombre " +
                             "WHERE id_especialidad = @id";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@nombre", e.NombreEspecialidad);
                    cmd.Parameters.AddWithValue("@id", e.id_especialidad);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                conexion.CerrarConxion();
            }
        }

        // Borra una especialidad por su id. Si ya hay médicos que la usan, SQL lanza el error 547
        // (violación de llave foránea) y lo cambio por un mensaje claro, para que no se pueda
        // borrar una especialidad que está en uso y dejar médicos sin especialidad.
        public void Eliminar(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();

            try
            {
                string sql = "DELETE FROM Especialidad WHERE id_especialidad = @id";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 547)   // violación de llave foránea
            {
                throw new InvalidOperationException(
                    "No se puede eliminar: hay médicos con esta especialidad.");
            }
            finally
            {
                conexion.CerrarConxion();
            }

        }

        // Devuelve todas las especialidades ordenadas alfabéticamente por nombre. Es la que llena
        // los ComboBox y las tablas, por ejemplo al registrar un médico o al agendar una cita.
        public List<Especialidad> ListarTodos()
        {
            List<Especialidad> lista = new List<Especialidad>();

            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                string sql = "SELECT id_especialidad, NombreEspecialidad " +
                             "FROM Especialidad ORDER BY NombreEspecialidad";
                using (SqlCommand cmd = new SqlCommand(sql, cn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    // Por cada fila armo una Especialidad y la agrego a la lista
                    while (r.Read())
                        lista.Add(new Especialidad(r.GetInt32(0), r.GetString(1)));
                }
            }
            finally { conexion.CerrarConxion(); }
            return lista;
        }
    }
}
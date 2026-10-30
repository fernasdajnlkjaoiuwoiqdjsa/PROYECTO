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
    public class EspcilaidadRepositorio:IRepositorio<Especialidad>
    {
        private ConexionBD conexion = new ConexionBD();

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
                    while (r.Read())
                        lista.Add(new Especialidad(r.GetInt32(0), r.GetString(1)));
                }
            }
            finally { conexion.CerrarConxion(); }
            return lista;
        }
    }
}

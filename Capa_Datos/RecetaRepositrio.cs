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
    public class RecetaRepositrio:IRepositorio<Receta>
    {
        private ConexionBD conexion = new ConexionBD();

        private const string SqlSelection = "SELECT id_receta, Descripcion, id_cita FROM Receta ";

        public void Guardar(Receta rec)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Receta (Descripcion, id_cita) VALUES (@d, @c); " +
                    "SELECT SCOPE_IDENTITY();", cn))
                {
                    cmd.Parameters.AddWithValue("@d", rec.Descripcion);
                    cmd.Parameters.AddWithValue("@c", rec.id_cita);
                    rec.id_receta = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("Esta cita ya tiene una receta.");   // UNIQUE de id_cita
            }
            finally
            { 
                conexion.CerrarConxion(); 
            }
        }
        public Receta Buscarr(int id)
        {
            return BuscarUna("WHERE id_receta = @v", id);

        }

        public void Actualizar(Receta rec)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Receta SET Descripcion = @d WHERE id_receta = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@d", rec.Descripcion);
                    cmd.Parameters.AddWithValue("@id", rec.id_receta);
                    cmd.ExecuteNonQuery();
                }
            }
            finally { conexion.CerrarConxion(); }
        }

        public void Eliminar(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Receta WHERE id_receta = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            finally { conexion.CerrarConxion(); }
        }

        public List<Receta> ListarTodos()
        {
            List<Receta> lista = new List<Receta>();
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(SqlSelection + "ORDER BY id_receta DESC", cn))
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                        lista.Add(new Receta(r.GetInt32(0), r.GetString(1), r.GetInt32(2)));
            }
            finally { conexion.CerrarConxion(); }
            return lista;
        }

        private Receta BuscarUna(string condicion, int valor)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(SqlSelection + condicion, cn))
                {
                    cmd.Parameters.AddWithValue("@v", valor);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                            return new Receta(r.GetInt32(0), r.GetString(1), r.GetInt32(2));
                        return null;
                    }
                }
            }
            finally { conexion.CerrarConxion(); }
        }




    }
}

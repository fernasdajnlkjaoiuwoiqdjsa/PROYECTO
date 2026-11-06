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
    // Repositorio de recetas: hace el CRUD de la tabla Receta. Implementa IRepositorio<Receta>,
    // así que cumple el mismo contrato que los demás repositorios. Es el que guarda lo que el
    // médico escribe al atender (Medico.EmitirReceta solo crea el objeto en memoria) y el que
    // permite después que el paciente la vea y la descargue en PDF. Aquí solo hay SQL.
    // (El nombre de la clase lleva typo, "Repositrio", y lo dejo igual porque otros archivos lo usan.)
    public class RecetaRepositrio : IRepositorio<Receta>
    {
        // Objeto que abre y cierra la conexión a la base de datos, igual que en los otros repositorios.
        private ConexionBD conexion = new ConexionBD();

        // Consulta base para leer recetas. Cada método le concatena su WHERE u ORDER BY al final.
        private const string SqlSelection = "SELECT id_receta, Descripcion, id_cita FROM Receta ";

        // Inserta una receta nueva ligada a su cita por id_cita. Uso SCOPE_IDENTITY() para que SQL
        // me devuelva el id generado y se lo asigno a rec.id_receta. Como solo debe haber una
        // receta por cita (la columna id_cita es UNIQUE), si ya existe una SQL lanza error 2627 o
        // 2601 y lo cambio por un mensaje claro.
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

        // Usa BuscarUna con un WHERE y devuelve la receta o null si no existe.
        public Receta Buscarr(int id)
        {
            return BuscarUna("WHERE id_receta = @v", id);

        }

        // Cambia el texto de una receta que ya existe, identificándola por su id. Solo actualiza
        // la descripción: la cita a la que pertenece no se puede cambiar.
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

        // Borra una receta por su id. Después de borrarla, esa cita puede volver a tener una
        // receta nueva.
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

        // Devuelve todas las recetas, de la más reciente (id más alto) a la más antigua. Por cada
        // fila armo un objeto Receta con id, descripción y el id de su cita.
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

        // Método privado que busca una sola receta con la condición que le pase y un único valor
        // (@v). Lo hice para poder buscar luego por otros campos sin repetir el código. Devuelve
        // la receta, o null si no hay ninguna fila.
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
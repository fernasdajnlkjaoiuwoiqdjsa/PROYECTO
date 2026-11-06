using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using System.Data.SqlClient;


namespace Capa_Datos
{
    // Repositorio de citas: es la única clase que habla con la tabla Cita (y con Receta, porque
    // la cita trae su receta con un JOIN). Implementa IRepositorio<Cita>, así que cumple el
    // contrato de Guardar, Buscarr, Actualizar, Eliminar y ListarTodos. Aquí solo hay SQL y
    // conversión de datos; las reglas del negocio viven en ServicioCitas y en las entidades.
    public class CitaRepositorio : IRepositorio<Cita>
    {
        // Objeto que abre y cierra la conexión a SQL Server. Cada método lo usa
        // para no repetir la cadena de conexión.
        private ConexionBD conexion = new ConexionBD();

        // Consulta base que reutilizo en todas las lecturas. Hace LEFT JOIN con Receta para traer
        // la cita y, si la tiene, su receta en una sola consulta. Uso LEFT (no INNER) porque
        // muchas citas todavía no tienen receta y no quiero que desaparezcan. Cada método solo le
        // concatena su condición (WHERE / ORDER BY) al final.
        private const string SqlSeleccion =
          "SELECT c.id_cita, c.fecha, c.Hora, c.Estado, c.id_paciente, c.id_Medico, " +
          "       r.id_receta, r.Descripcion " +
          "FROM Cita c LEFT JOIN Receta r ON r.id_cita = c.id_cita ";

        // Inserta una cita nueva. Es lo último que pasa al agendar. Uso SCOPE_IDENTITY() para que
        // SQL me devuelva el id que generó y se lo asigno a c.id_cita, así el objeto queda con su
        // id real. Si la base tiene una restricción única (médico + fecha + hora) y el horario ya
        // está tomado, SQL lanza error 2627 o 2601 y lo convierto en un mensaje claro para el
        // usuario. El finally cierra la conexión pase lo que pase.
        public void Guardar(Cita c)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Cita (fecha, Hora, Estado, id_paciente, id_Medico) " +
                    "VALUES (@f, @h, @e, @p, @m); SELECT SCOPE_IDENTITY();", cn))
                {
                    AgregarParametros(cmd, c);
                    c.id_cita = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("Ese horario ya está ocupado para el médico.");
            }
            finally { conexion.CerrarConxion(); }
        }

        // Busca una cita por su id (el nombre lleva doble r porque así quedó en la interfaz).
        // Reutiliza Consultar con un WHERE y devuelve la cita, o null si no existe.
        public Cita Buscarr(int id)
        {
            List<Cita> r = Consultar("WHERE c.id_cita = @id", id);
            return r.Count > 0 ? r[0] : null;
        }

        // Actualiza una cita existente. Es lo que guarda los cambios de estado después de
        // Cancelar() o MarcarAtendida(), porque esos métodos solo cambian el objeto en memoria.
        // Reutiliza AgregarParametros y le suma el @id para el WHERE. Igual que en Guardar,
        // si el nuevo horario choca con otra cita se avisa con un mensaje claro.
        public void Actualizar(Cita c)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Cita SET fecha = @f, Hora = @h, Estado = @e, " +
                    "id_paciente = @p, id_Medico = @m WHERE id_cita = @id", cn))
                {
                    AgregarParametros(cmd, c);
                    cmd.Parameters.AddWithValue("@id", c.id_cita);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new InvalidOperationException("Ese horario ya está ocupado para el médico.");
            }
            finally { conexion.CerrarConxion(); }
        }

        // Borra una cita por su id. Si la cita ya tiene receta, SQL lanza error 547 (violación de
        // llave foránea) y lo traduzco a un mensaje entendible, para no dejar recetas huérfanas.
        public void Eliminar(int id)
        {
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Cita WHERE id_cita = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException("No se puede eliminar la cita tiene una receta.");
            }
            finally { conexion.CerrarConxion(); }
        }

        // Trae todas las citas, de la más reciente a la más antigua. La usan los reportes del
        // administrador, que luego filtran en memoria con Administrador.ReportesdeCitas.
        public List<Cita> ListarTodos()
        {
            return Consultar("ORDER BY c.fecha DESC, c.Hora DESC", null);
        }
        //Metodo propio para listar los pacientes en orden de forma desendiente 
        // Trae solo las citas de un paciente. Es la fuente del historial y de las tarjetas de su
        // dashboard. Es un método propio, no viene de la interfaz.
        public List<Cita> ListarPorPaciente(int idPaciente)
        {
            return Consultar("WHERE c.id_paciente = @id ORDER BY c.fecha DESC, c.Hora DESC", idPaciente);
        }
        //Metodo propio para listar los medicos en orden  

        // Trae solo las citas de un médico, de la más próxima a la más lejana (orden ascendente,
        // porque en la agenda interesa lo que viene primero). Alimenta su agenda y su dashboard.
        public List<Cita> ListarPorMedico(int idMedico)
        {
            return Consultar("WHERE c.id_Medico = @id ORDER BY c.fecha, c.Hora", idMedico);

        }

        // Devuelve las horas que un médico ya tiene tomadas en un día. Sirve para no mostrar esos
        // horarios al agendar y evitar citas dobles. Ignora las canceladas para que ese horario
        // quede libre otra vez. Leo la columna time de SQL como TimeSpan.
        public List<TimeSpan> HorariosOcupados(int idMedico, DateTime fecha)
        {
            List<TimeSpan> ocupadas = new List<TimeSpan>();

            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT Hora FROM Cita WHERE id_Medico = @m AND fecha = @f " +
                    "AND Estado <> 'Cancelada'", cn))
                {
                    cmd.Parameters.AddWithValue("@m", idMedico);
                    cmd.Parameters.AddWithValue("@f", fecha.Date);
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) ocupadas.Add((TimeSpan)r.GetValue(0));
                }
            }
            finally { conexion.CerrarConxion(); }
            return ocupadas;
        }

        // Método privado que ejecuta la consulta base más la condición que le pase. Lo hice para
        // no repetir en cada Listar/Buscar el mismo código de abrir conexión, leer filas y cerrar.
        // El id es opcional (int?): solo se agrega el parámetro @id cuando viene un valor. Por cada
        // fila llama a Construir para convertirla en un objeto Cita.
        private List<Cita> Consultar(string condicion, int? id)
        {
            List<Cita> lista = new List<Cita>();
            SqlConnection cn = conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand(SqlSeleccion + condicion, cn))
                {
                    if (id.HasValue) cmd.Parameters.AddWithValue("@id", id.Value);
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read()) lista.Add(Construir(r));
                }
            }
            finally { conexion.CerrarConxion(); }
            return lista;
        }

        // Convierte una fila del lector en un objeto Cita. Uso el constructor completo porque
        // el estado y los ids no tienen set público. La hora en SQL es time (TimeSpan) y en C#
        // es DateTime, así que la sumo a la fecha de hoy. Si la columna de la receta no es NULL,
        // también armo la Receta y se la dejo a la cita.
        private Cita Construir(SqlDataReader r)
        {
            int idCita = r.GetInt32(0);
            Cita c = new Cita(idCita,
                r.GetDateTime(1),
                DateTime.Today.Add((TimeSpan)r.GetValue(2)),   // time de sql a DateTime en c#
                r.GetString(3),
                r.GetInt32(4), r.GetInt32(5));

            if (!r.IsDBNull(6))
                c.Receta = new Receta(r.GetInt32(6), r.GetString(7), idCita);
            return c;
        }

        // Pone en el comando los parámetros que Guardar y Actualizar comparten (fecha, hora, estado,
        // paciente y médico). Los uso como parámetros y no pego los valores en el texto SQL para
        // evitar inyección SQL. Hace la conversión inversa de la hora: DateTime a time.
        private void AgregarParametros(SqlCommand cmd, Cita c)
        {
            cmd.Parameters.AddWithValue("@f", c.Fecha.Date);
            cmd.Parameters.AddWithValue("@h", c.Hora.TimeOfDay);   // DateTime de c# a time de sql
            cmd.Parameters.AddWithValue("@e", c.Estado);
            cmd.Parameters.AddWithValue("@p", c.id_paciente);
            cmd.Parameters.AddWithValue("@m", c.id_Medico);
        }



    }

}
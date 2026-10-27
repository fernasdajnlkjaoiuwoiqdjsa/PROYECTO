using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PROYECTO
{
    public class CitaRepositorio:IRepositorio<Cita>
    {
        private ConexionBD conexion = new ConexionBD();

        private const string SqlSeleccion =
          "SELECT c.id_cita, c.fecha, c.Hora, c.Estado, c.id_paciente, c.id_Medico, " +
          "       r.id_receta, r.Descripcion " +
          "FROM Cita c LEFT JOIN Receta r ON r.id_cita = c.id_cita ";

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

        public Cita Buscarr(int id)
        {
            List<Cita> r = Consultar("WHERE c.id_cita = @id", id);
            return r.Count > 0 ? r[0] : null;
        }

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
                throw new InvalidOperationException("No se puede eliminar: la cita tiene una receta.");
            }
            finally { conexion.CerrarConxion(); }
        }

        public List<Cita> ListarTodos()
        {
            return Consultar("ORDER BY c.fecha DESC, c.Hora DESC", null);
        }
        //Metodo propio para listar los pacientes en orden de forma desendiente 
        public List<Cita> ListarPorPaciente(int idPaciente)
        {
            return Consultar("WHERE c.id_paciente = @id ORDER BY c.fecha DESC, c.Hora DESC", idPaciente);
        }
        //Metodo propio para listar los medicos en orden  

        public List<Cita> ListarPorMedico(int idMedico)
        {
            return Consultar("WHERE c.id_Medico = @id ORDER BY c.fecha, c.Hora", idMedico);

        }

        public List<TimeSpan> HorariosOcupados(int idMedico,DateTime fecha)
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
        private Cita Construir(SqlDataReader r)
        {
            int idCita = r.GetInt32(0);
            Cita c = new Cita(idCita,
                r.GetDateTime(1),
                DateTime.Today.Add((TimeSpan)r.GetValue(2)),   // time (SQL) → DateTime (C#)
                r.GetString(3),
                r.GetInt32(4), r.GetInt32(5));

            if (!r.IsDBNull(6))
                c.Receta = new Receta(r.GetInt32(6), r.GetString(7), idCita);
            return c;
        }
        private void AgregarParametros(SqlCommand cmd, Cita c)
        {
            cmd.Parameters.AddWithValue("@f", c.Fecha.Date);
            cmd.Parameters.AddWithValue("@h", c.Hora.TimeOfDay);   // DateTime (C#) → time (SQL)
            cmd.Parameters.AddWithValue("@e", c.Estado);
            cmd.Parameters.AddWithValue("@p", c.id_paciente);
            cmd.Parameters.AddWithValue("@m", c.id_Medico);
        }



    }

}

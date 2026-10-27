using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PROYECTO
{
    public class Medico:Usuario
    {
        public int id_Medico {  get; set; }
        public string Nombre { get; set; }
        public string Apellido {  get; set; }
        public List<string> Telefono {  get; set; } = new List<string>();
        public List<string> Correo {  get; set; } = new List<string>();
        public int id_especialidad { get; set; }

        public Medico(int idMedico, string nombre, string apellido, List<string> telefonos, List<string> correos, int idEspecialidad, int idUsuario, string username, string password)
            :base(idUsuario, username, password, "Medico")
        {
            id_Medico = idMedico;
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefonos ?? new List<string>();
            Correo = correos ?? new List<string>();
            id_especialidad = idEspecialidad;

        }
       

        public List<Cita> VerAgenda(IEnumerable<Cita> citas)
        {
            // Agenda = mis citas de hoy en adelante, sin las canceladas
            return citas.Where(c => c.id_Medico == id_Medico
                                 && c.Fecha.Date >= DateTime.Today
                                 && c.Estado != EstadosCita.Cancelar)
                        .OrderBy(c => c.Fecha)
                        .ThenBy(c => c.Hora.TimeOfDay)
                        .ToList();
        }
        public void AtenderCita(Cita cita)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_Medico != id_Medico)
                throw new InvalidOperationException("Esa cita no está asignada a este médico.");

            cita.MarcarAtendida();    // Cita valida que esté Pendiente
        }
        public Receta EmitirReceta(Cita cita, string descripcion)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_Medico != id_Medico)
                throw new InvalidOperationException("Esa cita no está asignada a este médico.");
            if (cita.Estado != EstadosCita.Atendida)
                throw new InvalidOperationException("Solo se emite receta de una cita atendida.");
            if (cita.Receta != null)
                throw new InvalidOperationException("Esta cita ya tiene una receta.");
            

            Receta receta = new Receta(0, descripcion, cita.id_cita);
            cita.Receta = receta;     // composición la receta queda dentro de la cita
            return receta;
        }
        public List<Cita> HistorialPacientesAtendidos(IEnumerable<Cita> citas)
        {
            return citas.Where(c => c.id_Medico == id_Medico
                                 && c.Estado == EstadosCita.Atendida)
                        .OrderByDescending(c => c.Fecha)
                        .ThenByDescending(c => c.Hora.TimeOfDay)
                        .ToList();
        }

    }
}

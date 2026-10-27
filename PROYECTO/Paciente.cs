using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO
{
    public  class Paciente:Usuario
    {
        public int id_paciente {  get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public List<string> Telefono { get; set; } = new List<string>();
        public List<string> Correo { get; set; } = new List<string>(); 

        public DateTime FechaNacimineto { get; set;  }

        public Paciente(int idpaciente, string nombre, string apellido, List<string> telefonos, List<string> correos, DateTime fechanaci, int idUsuario, string username, string password)
            :base(idUsuario, username, password, "Paciente")

        {
            id_paciente = idpaciente;
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefonos ?? new List<string>();
            Correo = correos ?? new List<string>();
            FechaNacimineto = fechanaci;
        }


        public Cita AgendarCita(int idMedico, DateTime fecha, DateTime hora)
        {
            if (fecha.Date + hora.TimeOfDay <= DateTime.Now)
                throw new ArgumentException("No se puede agendar una cita en el pasado.");

            // Solo crea la cita en memoria. Guardarla y validar disponibilidad
            // lo hacen ServicioCitas y CitaRepositorio.
            return new Cita(0, fecha.Date, hora, EstadosCita.Pendiente, id_paciente, idMedico);
        }
        //Dejar Vacio ya q consultar la disponibilidad es de la clase SevicioCitas
        public void VerMedicosDisponibles()
        {

        }
        public void CancelarCita(Cita cita)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_paciente != id_paciente)
                throw new InvalidOperationException("Solo puedes cancelar tus propias citas.");

            cita.Cancelar();
        }
        public List<Cita>VerHistorial(IEnumerable<Cita> citas)
        {
            return citas.Where(c => c.id_paciente == id_paciente)
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.Hora.TimeOfDay)
                .ToList();
        }
        public Receta VerRecetaEmitida(Cita cita)
        {
            if (cita.id_paciente != id_paciente)
                throw new InvalidOperationException("Esa cita no es tuya.");

            if (cita.Estado != EstadosCita.Atendida) return null;
            return cita.Receta;
        }
    }
}

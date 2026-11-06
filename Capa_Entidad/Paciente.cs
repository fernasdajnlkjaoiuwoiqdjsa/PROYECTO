using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad
{
    // El paciente hereda de Usuario, así que ya trae username, contraseña, rol y activo. Aquí
    // agrego sus datos personales y las acciones que puede hacer con sus citas (agendar,
    // cancelar, ver su historial y ver su receta). Esta clase no toca la base de datos: arma
    // objetos y filtra listas que ya le pasan; guardar lo hacen el servicio y el repositorio.
    public class Paciente : Usuario
    {
        // Id del paciente en la tabla Paciente. Es distinto de id_usuario (el de la cuenta de
        // login). En las citas se guarda este id para saber de quién es cada una.
        public int id_paciente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        // Un paciente puede tener varios teléfonos y correos, por eso son listas. Las inicializo
        // con new List<string>() para que nunca sean null y no se caiga al recorrerlas.
        public List<string> Telefono { get; set; } = new List<string>();
        public List<string> Correo { get; set; } = new List<string>();

        // Fecha de nacimiento (el nombre lleva el typo "Nacimineto", lo dejo igual porque otros
        // archivos lo usan). El administrador la valida al registrar al paciente.
        public DateTime FechaNacimineto { get; set; }

        // Constructor completo: recibe los datos del paciente y los de su cuenta. Los de la cuenta
        // (idUsuario, username, password) se los paso a Usuario con base(...) y el rol lo dejo
        // fijo en "Paciente" para que no se cree uno con otro rol. Lo uso cuando el repositorio
        // arma el objeto desde la base, o al registrar uno nuevo con idpaciente = 0.
        public Paciente(int idpaciente, string nombre, string apellido, List<string> telefonos, List<string> correos, DateTime fechanaci, int idUsuario, string username, string password)
            : base(idUsuario, username, password, "Paciente")

        {
            id_paciente = idpaciente;
            Nombre = nombre;
            Apellido = apellido;
            // Si me mandan null en vez de lista, creo una vacía con ?? para evitar errores después
            Telefono = telefonos ?? new List<string>();
            Correo = correos ?? new List<string>();
            FechaNacimineto = fechanaci;
        }


        // Crea una cita nueva para este paciente con el médico, fecha y hora que eligió. Es el
        // primer paso de agendar: aquí solo reviso que no sea en el pasado (sumo la fecha más la
        // hora y la comparo con DateTime.Now). Devuelve la cita en estado Pendiente y ya con mi
        // id_paciente puesto, para que nadie agende a nombre de otro.
        public Cita AgendarCita(int idMedico, DateTime fecha, DateTime hora)
        {
            if (fecha.Date + hora.TimeOfDay <= DateTime.Now)
                throw new ArgumentException("No se puede agendar una cita en el pasado.");

            // Solo crea la cita en memoria. Guardarla y validar disponibilidad
            // lo hacen ServicioCitas y CitaRepositorio.
            return new Cita(0, fecha.Date, hora, EstadosCita.Pendiente, id_paciente, idMedico);
        }
        //Dejar Vacio ya q consultar la disponibilidad es de la clase SevicioCitas
        // Lo dejé vacío a propósito: consultar qué médicos están libres requiere leer la base de
        // datos, y eso le toca a ServicioCitas, no a la entidad. Lo conservo para que el método
        // aparezca en el diagrama de clases como parte de lo que hace el paciente.
        public void VerMedicosDisponibles()
        {

        }

        // Cancela una cita del paciente. Primero reviso que la cita exista y que sea suya, porque
        // no debe poder cancelar las de otros. La regla de que solo se cancela una Pendiente la
        // aplica Cita.Cancelar(). Cambia el estado en memoria; guardarlo lo hace el servicio.
        public void CancelarCita(Cita cita)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_paciente != id_paciente)
                throw new InvalidOperationException("Solo puedes cancelar tus propias citas.");

            cita.Cancelar();
        }

        // Devuelve el historial de citas del paciente (todos los estados), de la más reciente a la
        // más antigua. Es lo que ve en su formulario de historial y de donde salen las tarjetas de
        // su dashboard. Recibe la lista completa de citas y filtra solo las suyas por id_paciente.
        public List<Cita> VerHistorial(IEnumerable<Cita> citas)
        {
            return citas.Where(c => c.id_paciente == id_paciente)
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.Hora.TimeOfDay)
                .ToList();
        }

        // Devuelve la receta de una cita para que el paciente la vea o la descargue en PDF.
        // Reviso que la cita sea suya, y si todavía no está Atendida devuelvo null porque aún no
        // puede haber receta. Si está atendida, devuelvo cita.Receta, que también puede ser null
        // si el médico no emitió ninguna, así que quien la llame debe revisar el null.
        public Receta VerRecetaEmitida(Cita cita)
        {
            if (cita.id_paciente != id_paciente)
                throw new InvalidOperationException("Esa cita no es tuya.");

            if (cita.Estado != EstadosCita.Atendida) return null;
            return cita.Receta;
        }
    }
}
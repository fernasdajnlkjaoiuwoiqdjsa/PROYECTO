using Capa_Datos;
using Capa_Entidad;
using Capoa_Entidad;
using PROYECTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio
{
    // Servicio de citas: es la capa de negocio de todo lo que tiene que ver con citas. Los
    // formularios (agendar, historial, agenda del médico, recetas, dashboards) hablan con esta
    // clase y no con la base de datos. Aquí junto las entidades (que tienen las reglas, como
    // Paciente.AgendarCita o Medico.AtenderCita) con los repositorios (que guardan y leen), y
    // por eso casi cada método sigue el mismo patrón: traer de la base, aplicar la regla en la
    // entidad y volver a guardar.
    public class ServicioCitas
    {
        //Creamos una rejilla provisional de horarios por confirmar

        // Horario de atención de la clínica: de 8:00 a 16:00, con citas de 30 minutos. Son
        // constantes de solo lectura (static readonly) porque son reglas del negocio y no deben
        // cambiar mientras corre el programa. Si la clínica cambia de horario, solo toco aquí.
        private static readonly TimeSpan HoraInicio = new TimeSpan(8, 0, 0);
        private static readonly TimeSpan HoraFin = new TimeSpan(16, 0, 0);
        private static readonly TimeSpan Duracion = new TimeSpan(0, 30, 0);

        // Los repositorios que necesita este servicio. Cada uno se encarga de una tabla.
        private CitaRepositorio citaRepo = new CitaRepositorio();
        private MedicoRepositorio medicoRepo = new MedicoRepositorio();
        private RecetaRepositrio recetaRepo = new RecetaRepositrio();
        private EspcilaidadRepositorio especialidadRepo = new EspcilaidadRepositorio();
        private PacienteRepositorio pacienteRepo = new PacienteRepositorio();

        // Devuelve todas las especialidades para llenar el ComboBox al agendar una cita.
        public List<Especialidad> ObtenerEspecialidades()
        {
            return especialidadRepo.ListarTodos();
        }

        // id_paciente → "Nombre Apellido" (para mostrar la agenda del médico)
        // Arma un diccionario para traducir el id de un paciente a su nombre completo. Lo uso
        // en la agenda del médico, porque la cita solo trae el id_paciente y en la tabla quiero
        // mostrar el nombre. Es más cómodo cargarlo una vez que buscar paciente por paciente.
        public Dictionary<int, string> NombresDePacientes()
        {
            return pacienteRepo.ListarTodos()
                .ToDictionary(p => p.id_paciente, p => p.Nombre + " " + p.Apellido);
        }

        // Devuelve los médicos de una especialidad. Es el segundo ComboBox al agendar: el
        // paciente elige primero la especialidad y aquí salen solo los médicos de esa. Traigo todos
        // los médicos y filtro en memoria con LINQ.
        public List<Medico> ObtenerMedicosPorEspecialidad(int idEspecialidad)
        {
            return medicoRepo.ListarTodos().Where(m => m.id_especialidad == idEspecialidad).ToList();

        }

        // Calcula los horarios libres de un médico en un día. Recorro la rejilla de 8:00 a 16:00
        // de 30 en 30 minutos y me quedo con las horas que no están ocupadas (las que da
        // CitaRepositorio.HorariosOcupados). Si el día es hoy, salto las horas que ya pasaron
        // para no ofrecer una hora anterior a la actual. Es lo que llena la lista de horas del
        // formulario de agendar.
        public List<TimeSpan> ObtenerHorariosDisponibles(int idMedico, DateTime fecha)
        {
            List<TimeSpan> ocupados = citaRepo.HorariosOcupados(idMedico, fecha);
            List<TimeSpan> libres = new List<TimeSpan>();

            for (TimeSpan h = HoraInicio; h < HoraFin; h = h.Add(Duracion))
            {
                if (fecha.Date + h <= DateTime.Now) continue;   // hoy no se ofrecen horas pasadas
                if (!ocupados.Contains(h)) libres.Add(h);
            }
            return libres;
        }

        // Dice si una hora concreta sigue libre para ese médico y fecha. Reutiliza
        // ObtenerHorariosDisponibles y revisa si la hora está en la lista. Es la comprobación
        // final antes de guardar, por si entre que el paciente eligió la hora y presionó el botón,
        // otra persona la tomó.
        public bool VerificarDisponibilidad(int idMedico, DateTime fecha, TimeSpan hora)
        {
            return ObtenerHorariosDisponibles(idMedico, fecha).Contains(hora);

        }

        // Agenda una cita completa. Reviso que el paciente exista, que el médico exista y que el
        // horario siga libre. Después le pido a la entidad Paciente que cree la cita (ahí se
        // valida que no sea en el pasado) y la guardo con el repositorio. La hora llega como
        // TimeSpan y la convierto a DateTime sumándola a la fecha de hoy, que es lo que espera la
        // entidad. Devuelve la cita ya guardada, con su id real.
        public Cita AgendarCita(Paciente paciente, int idMedico, DateTime fecha, TimeSpan hora)
        {
            if (paciente == null) throw new ArgumentNullException(nameof(paciente));
            if (medicoRepo.Buscarr(idMedico) == null)
                throw new InvalidOperationException("El médico seleccionado no existe.");
            if (!VerificarDisponibilidad(idMedico, fecha, hora))
                throw new InvalidOperationException("Ese horario ya no está disponible. Elige otro.");

            Cita cita = paciente.AgendarCita(idMedico, fecha, DateTime.Today.Add(hora));
            citaRepo.Guardar(cita);
            return cita;
        }

        // Cancela una cita del paciente. Primero la busco en la base, luego le pido a la entidad
        // Paciente que la cancele (ahí se revisa que sea suya y que esté pendiente), y como eso
        // solo cambia el objeto en memoria, al final la guardo con Actualizar.
        public void CancelarCita(Paciente paciente, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            paciente.CancelarCita(cita);     // valida que sea suya y que esté pendiente
            citaRepo.Actualizar(cita);
        }

        // Marca una cita como atendida. Mismo patrón que cancelar: busco la cita, el Medico la
        // marca (valida que sea suya y que esté pendiente) y guardo el nuevo estado.
        public void MarcarAtendida(Medico medico, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            medico.AtenderCita(cita);        // valida que sea suya y que esté pendiente
            citaRepo.Actualizar(cita);
        }

        // Emite la receta de una cita. Busco la cita, el Medico crea la receta en memoria (valida
        // que sea suya, que esté atendida y que no tenga ya una) y yo la guardo con el repositorio
        // de recetas. Esa guarda es la que le asigna el id real. Devuelve la receta guardada.
        public Receta EmitirReceta(Medico medico, int idCita, string descripcion)
        {
            Cita cita = ObtenerCita(idCita);
            Receta receta = medico.EmitirReceta(cita, descripcion);
            recetaRepo.Guardar(receta);
            return receta;
        }

        // Texto para FrmVerReceta; null si la cita no tiene receta
        // Devuelve el texto completo de la receta para mostrarla o descargarla en PDF. Busco la
        // cita, le pido a Paciente la receta (valida que la cita sea suya y esté atendida) y, si
        // hay, busco al médico para que Receta.Generar arme el texto con los nombres, la fecha y
        // la descripción. Si no hay receta devuelvo null y el formulario avisa al paciente.
        public string VerReceta(Paciente paciente, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            Receta receta = paciente.VerRecetaEmitida(cita);
            if (receta == null) return null;

            Medico medico = medicoRepo.Buscarr(cita.id_Medico);
            return receta.Generar(medico, paciente, cita);
        }

        // Historial de citas del paciente: traigo las suyas de la base y la entidad Paciente las
        // filtra y ordena (de la más reciente a la más antigua). Lo usa el formulario de historial
        // y casi todos los métodos del dashboard del paciente.
        public List<Cita> HistorialPaciente(Paciente paciente)
        {
            return paciente.VerHistorial(citaRepo.ListarPorPaciente(paciente.id_paciente));
        }

        // Agenda del médico (sus citas de hoy en adelante, sin canceladas): traigo las suyas y la
        // entidad Medico las filtra y ordena.
        public List<Cita> AgendaMedico(Medico medico)
        {
            return medico.VerAgenda(citaRepo.ListarPorMedico(medico.id_Medico));
        }

        // Historial de las citas que el médico ya atendió, de la más reciente a la más antigua.
        public List<Cita> PacientesAtendidos(Medico medico)
        {
            return medico.HistorialPacientesAtendidos(citaRepo.ListarPorMedico(medico.id_Medico));
        }

        // Método privado que busca una cita por id y lanza un error claro si no existe. Lo uso en
        // casi todos los de arriba para no repetir la misma comprobación de null.
        private Cita ObtenerCita(int idCita)
        {
            Cita cita = citaRepo.Buscarr(idCita);
            if (cita == null) throw new InvalidOperationException("La cita no existe.");
            return cita;
        }

        // Devuelve una cita pero solo si es de ese médico. Lo usa FrmAtenderCita para cargar la
        // cita que se va a atender y evitar que un médico abra una de otro.
        public Cita ObtenerCitaDeMedico(Medico medico, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            if (cita.id_Medico != medico.id_Medico)
                throw new InvalidOperationException("Esa cita no está asignada a este médico.");
            return cita;
        }

        // Busca un paciente por su id (con todos sus datos). Sirve para mostrar quién es el
        // paciente de una cita.
        public Paciente ObtenerPaciente(int idPaciente)
        {
            return pacienteRepo.Buscarr(idPaciente);
        }

        // Citas atendidas con receta, de la más reciente a la más antigua
        // Es la lista del apartado de recetas del paciente: del historial me quedo solo con las
        // citas atendidas que sí tienen una receta.
        public List<Cita> RecetasDePaciente(Paciente paciente)
        {
            return HistorialPaciente(paciente)
                .Where(c => c.Estado == EstadosCita.Atendida && c.Receta != null)
                .ToList();
        }

        // Devuelve "Nombre Apellido" de un médico, o texto vacío si no existe. Para mostrar el
        // nombre en vez del id en tablas e historial.
        public string NombreMedico(int idMedico)
        {
            Medico m = medicoRepo.Buscarr(idMedico);
            return m == null ? "" : m.Nombre + " " + m.Apellido;
        }

        // id_Medico → "Nombre Apellido"
        // Igual que NombresDePacientes pero para médicos: diccionario para traducir el id del
        // médico a su nombre en tablas con muchas citas, sin buscar uno por uno.
        public Dictionary<int, string> NombresDeMedicos()
        {
            return medicoRepo.ListarTodos()
                .ToDictionary(m => m.id_Medico, m => m.Nombre + " " + m.Apellido);
        }

        // Próxima cita pendiente que todavía no ha pasado
        // Es la tarjeta de "Próxima cita" del dashboard del paciente. Del historial me quedo solo
        // con las pendientes cuya fecha y hora aún no pasan, las ordeno de la más cercana a la
        // más lejana y tomo la primera. Devuelve null si no tiene ninguna.
        public Cita ProximaCita(Paciente paciente)
        {
            return HistorialPaciente(paciente)
                .Where(c => c.Estado == EstadosCita.Pendiente
                         && c.Fecha.Date + c.Hora.TimeOfDay >= DateTime.Now)
                .OrderBy(c => c.Fecha.Date + c.Hora.TimeOfDay)
                .FirstOrDefault();
        }

        // La cita más reciente (el historial ya viene de la más nueva a la más antigua)
        // Como el historial ya viene ordenado, la primera es la más reciente. Devuelve null si el
        // paciente no tiene citas.
        public Cita UltimaCita(Paciente paciente)
        {
            return HistorialPaciente(paciente).FirstOrDefault();
        }

        // Cuenta cuántas citas del paciente tienen un estado dado (Pendiente, Atendida o
        // Cancelar). Son los números de las tarjetas del dashboard del paciente.
        public int CantidadCitas(Paciente paciente, string estado)
        {
            return HistorialPaciente(paciente).Count(c => c.Estado == estado);
        }

        // "Nombre Apellido · Especialidad"
        // Arma el texto del médico con su especialidad, por ejemplo "Ana López · Pediatría", para
        // mostrar en las tarjetas y tablas. Busco al médico y luego su especialidad por id. Si no
        // encuentra la especialidad, muestra solo el nombre.
        public string DescripcionMedico(int idMedico)
        {
            Medico m = medicoRepo.Buscarr(idMedico);
            if (m == null) return "";

            Especialidad esp = especialidadRepo.Buscarr(m.id_especialidad);
            return m.Nombre + " " + m.Apellido + (esp != null ? " · " + esp.NombreEspecialidad : "");
        }

        // Método privado que trae todas las citas de un médico, y avisa si el médico es null. Lo
        // reutilizo en los dos métodos del dashboard del médico.
        private List<Cita> CitasDeMedico(Medico medico)
        {
            if (medico == null) throw new ArgumentNullException(nameof(medico));
            return citaRepo.ListarPorMedico(medico.id_Medico);
        }

        // Números de las 3 tarjetas del dashboard del médico
        // Devuelve un diccionario con tres números: citas de hoy (sin contar canceladas),
        // pendientes de hoy en adelante y atendidas hoy. Uso un diccionario de texto a entero
        // para que el formulario lea cada tarjeta por su nombre (CitasHoy, Pendientes,
        // AtendidasHoy) con una sola consulta a la base.
        public Dictionary<string, int> ResumenMedico(Medico medico)
        {
            List<Cita> citas = CitasDeMedico(medico);
            DateTime hoy = DateTime.Today;

            return new Dictionary<string, int>
    {
        { "CitasHoy",     citas.Count(c => c.Fecha.Date == hoy && c.Estado != EstadosCita.Cancelar) },
        { "Pendientes",   citas.Count(c => c.Estado == EstadosCita.Pendiente && c.Fecha.Date >= hoy) },
        { "AtendidasHoy", citas.Count(c => c.Fecha.Date == hoy && c.Estado == EstadosCita.Atendida) }
    };
        }

        // Próximas citas pendientes de hoy en adelante, ordenadas por fecha y hora
        // Es la lista del dashboard del médico con sus próximas citas. Filtro las pendientes de
        // hoy en adelante, las ordeno y con Take(max) me quedo solo con las primeras. Así, si
        // tiene dos pendientes con distintos pacientes, salen las dos.
        public List<Cita> ProximasCitasMedico(Medico medico, int max)
        {
            return CitasDeMedico(medico)
                .Where(c => c.Estado == EstadosCita.Pendiente && c.Fecha.Date >= DateTime.Today)
                .OrderBy(c => c.Fecha.Date + c.Hora.TimeOfDay)
                .Take(max)
                .ToList();
        }
    }
}
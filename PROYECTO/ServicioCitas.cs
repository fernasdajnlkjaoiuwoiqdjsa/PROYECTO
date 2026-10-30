using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    public class ServicioCitas
    {
        //Creamos una rejilla provisional de horarios por confirmar

        private static readonly TimeSpan HoraInicio = new TimeSpan(8, 0, 0);
        private static readonly TimeSpan HoraFin = new TimeSpan(16, 0, 0);
        private static readonly TimeSpan Duracion = new TimeSpan(0, 30, 0);

        private CitaRepositorio citaRepo = new CitaRepositorio();
        private MedicoRepositorio medicoRepo = new MedicoRepositorio();
        private RecetaRepositrio recetaRepo = new RecetaRepositrio();

        private EspcilaidadRepositorio especialidadRepo = new EspcilaidadRepositorio();
        private PacienteRepositorio pacienteRepo = new PacienteRepositorio();

        public List<Especialidad> ObtenerEspecialidades()
        {
            return especialidadRepo.ListarTodos();
        }

        // id_paciente → "Nombre Apellido" (para mostrar la agenda del médico)
        public Dictionary<int, string> NombresDePacientes()
        {
            return pacienteRepo.ListarTodos()
                .ToDictionary(p => p.id_paciente, p => p.Nombre + " " + p.Apellido);
        }

        public List<Medico> ObtenerMedicosPorEspecialidad(int idEspecialidad)
        {
            return medicoRepo.ListarTodos().Where(m => m.id_especialidad == idEspecialidad).ToList();

        }
        public List<TimeSpan> ObtenerHorariosDisponibles(int idMedico,DateTime fecha)
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

        public bool VerificarDisponibilidad(int idMedico,DateTime fecha, TimeSpan hora)
        {
            return ObtenerHorariosDisponibles(idMedico, fecha).Contains(hora);

        }
        public Cita AgendarCita(Paciente paciente, int idMedico ,DateTime fecha, TimeSpan hora)
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
        public void CancelarCita(Paciente paciente, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            paciente.CancelarCita(cita);     // valida que sea suya y que esté pendiente
            citaRepo.Actualizar(cita);
        }
        public void MarcarAtendida(Medico medico, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            medico.AtenderCita(cita);        // valida que sea suya y que esté pendiente
            citaRepo.Actualizar(cita);
        }
        public Receta EmitirReceta(Medico medico, int idCita, string descripcion)
        {
            Cita cita = ObtenerCita(idCita);
            Receta receta = medico.EmitirReceta(cita, descripcion);
            recetaRepo.Guardar(receta);
            return receta;
        }

        // Texto para FrmVerReceta; null si la cita no tiene receta
        public string VerReceta(Paciente paciente, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            Receta receta = paciente.VerRecetaEmitida(cita);
            if (receta == null) return null;

            Medico medico = medicoRepo.Buscarr(cita.id_Medico);
            return receta.Generar(medico, paciente, cita);
        }
        public List<Cita> HistorialPaciente(Paciente paciente)
        {
            return paciente.VerHistorial(citaRepo.ListarPorPaciente(paciente.id_paciente));
        }

        public List<Cita> AgendaMedico(Medico medico)
        {
            return medico.VerAgenda(citaRepo.ListarPorMedico(medico.id_Medico));
        }

        public List<Cita> PacientesAtendidos(Medico medico)
        {
            return medico.HistorialPacientesAtendidos(citaRepo.ListarPorMedico(medico.id_Medico));
        }

        private Cita ObtenerCita(int idCita)
        {
            Cita cita = citaRepo.Buscarr(idCita);
            if (cita == null) throw new InvalidOperationException("La cita no existe.");
            return cita;
        }
        public Cita ObtenerCitaDeMedico(Medico medico, int idCita)
        {
            Cita cita = ObtenerCita(idCita);
            if (cita.id_Medico != medico.id_Medico)
                throw new InvalidOperationException("Esa cita no está asignada a este médico.");
            return cita;
        }

        public Paciente ObtenerPaciente(int idPaciente)
        {
            return pacienteRepo.Buscarr(idPaciente);
        }
        // Citas atendidas con receta, de la más reciente a la más antigua
        public List<Cita> RecetasDePaciente(Paciente paciente)
        {
            return HistorialPaciente(paciente)
                .Where(c => c.Estado == EstadosCita.Atendida && c.Receta != null)
                .ToList();
        }

        public string NombreMedico(int idMedico)
        {
            Medico m = medicoRepo.Buscarr(idMedico);
            return m == null ? "" : m.Nombre + " " + m.Apellido;
        }

        // id_Medico → "Nombre Apellido"
        public Dictionary<int, string> NombresDeMedicos()
        {
            return medicoRepo.ListarTodos()
                .ToDictionary(m => m.id_Medico, m => m.Nombre + " " + m.Apellido);
        }
        // Próxima cita pendiente que todavía no ha pasado
        public Cita ProximaCita(Paciente paciente)
        {
            return HistorialPaciente(paciente)
                .Where(c => c.Estado == EstadosCita.Pendiente
                         && c.Fecha.Date + c.Hora.TimeOfDay >= DateTime.Now)
                .OrderBy(c => c.Fecha.Date + c.Hora.TimeOfDay)
                .FirstOrDefault();
        }

        // La cita más reciente (el historial ya viene de la más nueva a la más antigua)
        public Cita UltimaCita(Paciente paciente)
        {
            return HistorialPaciente(paciente).FirstOrDefault();
        }

        public int CantidadCitas(Paciente paciente, string estado)
        {
            return HistorialPaciente(paciente).Count(c => c.Estado == estado);
        }

        // "Nombre Apellido · Especialidad"
        public string DescripcionMedico(int idMedico)
        {
            Medico m = medicoRepo.Buscarr(idMedico);
            if (m == null) return "";

            Especialidad esp = especialidadRepo.Buscarr(m.id_especialidad);
            return m.Nombre + " " + m.Apellido + (esp != null ? " · " + esp.NombreEspecialidad : "");
        }
        private List<Cita> CitasDeMedico(Medico medico)
        {
            if (medico == null) throw new ArgumentNullException(nameof(medico));
            return citaRepo.ListarPorMedico(medico.id_Medico);
        }

        // Números de las 3 tarjetas del dashboard del médico
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

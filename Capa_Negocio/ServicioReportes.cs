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
    public class ServicioReportes
    {

        private CitaRepositorio cita = new CitaRepositorio();

        private PacienteRepositorio pacienteRepo = new PacienteRepositorio();
        private MedicoRepositorio medicoRepo = new MedicoRepositorio();
        private EspcilaidadRepositorio especialidadRepo = new EspcilaidadRepositorio();
        private UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();

        // Números de las 6 tarjetas del dashboard
        public Dictionary<string, int> ResumenAdministrador(Administrador admin)
        {
            List<Cita> citas = GenerarReporteCitas(admin, null, null, null);   // también valida al admin
            List<Usuario> usuarios = usuarioRepo.ListarTodos();

            return new Dictionary<string, int>
    {
        { "CitasHoy",        citas.Count(c => c.Fecha.Date == DateTime.Today && c.Estado != EstadosCita.Cancelar) },
        { "Canceladas",      citas.Count(c => c.Estado == EstadosCita.Cancelar) },
        { "Pendientes",      citas.Count(c => c.Estado == EstadosCita.Pendiente) },
        { "Pacientes",       pacienteRepo.ListarTodos().Count },
        { "MedicosActivos",  usuarios.Count(u => u.Rol == "Medico" && u.Activo) },
        { "Especialidades",  especialidadRepo.ListarTodos().Count }
    };
        }

        // Para el pastel "Citas por especialidad"
        public Dictionary<string, int> ReporteEspecialidad(Administrador admin)
        {
            List<Cita> citas = GenerarReporteCitas(admin, null, null, null)
                               .Where(c => c.Estado != EstadosCita.Cancelar).ToList();

            Dictionary<int, int> espDeMedico = medicoRepo.ListarTodos()
                .ToDictionary(m => m.id_Medico, m => m.id_especialidad);

            Dictionary<string, int> resultado = new Dictionary<string, int>();
            foreach (Especialidad esp in especialidadRepo.ListarTodos())
            {
                int cantidad = citas.Count(c => espDeMedico.ContainsKey(c.id_Medico)
                                             && espDeMedico[c.id_Medico] == esp.id_especialidad);
                resultado[esp.NombreEspecialidad] = cantidad;
            }
            return resultado;
        }
        //Para la tabla de frmVerReportes

        public List<Cita> GenerarReporteCitas(Administrador admin, DateTime? desde,
                                              DateTime? hasta, string estado)
        {
            if (admin == null) throw new ArgumentNullException(nameof(admin));
            if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.");

            return admin.ReportesdeCitas(cita.ListarTodos(), desde, hasta, estado);
        }

        //Para la grefica de pastl q se implementara: Cuantas citas hay por estado
        public Dictionary<string, int> ReporteEstado(Administrador admin, DateTime? desde, DateTime? hasta)
        {
            List<Cita> citas = GenerarReporteCitas(admin, desde, hasta, null);

            return new Dictionary<string, int>
            {
                { EstadosCita.Pendiente, citas.Count(c => c.Estado == EstadosCita.Pendiente) },
                { EstadosCita.Atendida,  citas.Count(c => c.Estado == EstadosCita.Atendida) },
                { EstadosCita.Cancelar, citas.Count(c => c.Estado == EstadosCita.Cancelar) }
            };
        }
        public Dictionary<int, string> NombresDePacientes()
        {
            return pacienteRepo.ListarTodos()
                .ToDictionary(p => p.id_paciente, p => p.Nombre + " " + p.Apellido);
        }

        public Dictionary<int, string> NombresDeMedicos()
        {
            return medicoRepo.ListarTodos()
                .ToDictionary(m => m.id_Medico, m => m.Nombre + " " + m.Apellido);
        }

        // fecha = null: todas las fechas. estado = null: todos. nombre: busca en paciente o médico
        public List<Cita> BuscarCitas(Administrador admin, DateTime? fecha, string estado, string nombre)
        {
            List<Cita> citas = GenerarReporteCitas(admin, fecha, fecha, estado);

            string buscado = (nombre ?? "").Trim().ToLower();
            if (buscado == "") return citas;

            Dictionary<int, string> pac = NombresDePacientes();
            Dictionary<int, string> med = NombresDeMedicos();

            return citas.Where(c =>
                (pac.ContainsKey(c.id_paciente) && pac[c.id_paciente].ToLower().Contains(buscado)) ||
                (med.ContainsKey(c.id_Medico) && med[c.id_Medico].ToLower().Contains(buscado))
            ).ToList();
        }
        // agrupar = "Mes" o "Día". Devuelve etiqueta → cantidad de citas
        public Dictionary<string, int> CitasPorPeriodo(Administrador admin, string agrupar,
            DateTime? referencia, string estado, string nombre, out string titulo)
        {
            List<Cita> citas = BuscarCitas(admin, null, estado, nombre);   // sin filtro de fecha
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            titulo = "Citas por mes";

            if (citas.Count == 0) return resultado;

            if (agrupar == "Mes")
            {
                foreach (var grupo in citas
                    .GroupBy(c => new DateTime(c.Fecha.Year, c.Fecha.Month, 1))
                    .OrderBy(g => g.Key))
                {
                    resultado[grupo.Key.ToString("MMM yyyy")] = grupo.Count();
                }
                return resultado;
            }

            // Por día: todos los días del mes de referencia, también los que tienen 0
            DateTime mes = referencia ?? citas.Max(c => c.Fecha);
            titulo = "Citas por día - " + mes.ToString("MMMM yyyy");

            int dias = DateTime.DaysInMonth(mes.Year, mes.Month);
            for (int d = 1; d <= dias; d++)
            {
                DateTime dia = new DateTime(mes.Year, mes.Month, d);
                resultado[d.ToString("00")] = citas.Count(c => c.Fecha.Date == dia);
            }
            return resultado;
        }
    }
}

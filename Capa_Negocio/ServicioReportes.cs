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
    // Servicio de reportes: es la capa de negocio del administrador para todo lo que son números,
    // tablas y gráficas (dashboard, reporte de citas, búsqueda y gráficas por estado, por
    // especialidad y por mes o día). Los formularios le piden los datos a esta clase y ella los
    // arma juntando los repositorios. El filtrado de las citas lo hace la entidad Administrador
    // (ReportesdeCitas); aquí solo agrupo y cuento.
    public class ServicioReportes
    {

        // Los repositorios que necesita este servicio. Cada uno se encarga de una tabla.
        // (El de citas se llama "cita" a secas y lo dejo así porque se usa más abajo.)
        private CitaRepositorio cita = new CitaRepositorio();

        private PacienteRepositorio pacienteRepo = new PacienteRepositorio();
        private MedicoRepositorio medicoRepo = new MedicoRepositorio();
        private EspcilaidadRepositorio especialidadRepo = new EspcilaidadRepositorio();
        private UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();

        // Números de las 6 tarjetas del dashboard
        // Devuelve un diccionario con los seis números del dashboard del administrador: citas de
        // hoy (sin canceladas), canceladas, pendientes, total de pacientes, médicos activos y
        // total de especialidades. El formulario lee cada tarjeta por su nombre. Para las citas
        // llamo a GenerarReporteCitas sin filtros, y de paso eso confirma que me pasaron un
        // administrador. Los médicos activos salen de la lista de usuarios (rol Medico y Activo).
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
        // Cuenta cuántas citas (sin las canceladas) tiene cada especialidad. La cita solo trae el
        // id del médico y el médico trae su especialidad, así que armo primero un diccionario
        // médico → especialidad y luego, por cada especialidad del catálogo, cuento las citas cuyo
        // médico pertenece a ella. Incluyo también las especialidades con 0 citas para que
        // aparezcan en la gráfica. Devuelve nombre de la especialidad → cantidad.
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

        // Es el método base de todos los reportes: trae todas las citas de la base y le pide a la
        // entidad Administrador que las filtre por fecha inicial, fecha final y estado (los tres
        // son opcionales). Antes valido que haya un administrador y que la fecha inicial no sea
        // mayor que la final. Casi todos los demás métodos de esta clase se apoyan en este.
        public List<Cita> GenerarReporteCitas(Administrador admin, DateTime? desde,
                                              DateTime? hasta, string estado)
        {
            if (admin == null) throw new ArgumentNullException(nameof(admin));
            if (desde.HasValue && hasta.HasValue && desde.Value.Date > hasta.Value.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la final.");

            return admin.ReportesdeCitas(cita.ListarTodos(), desde, hasta, estado);
        }

        //Para la grefica de pastl q se implementara: Cuantas citas hay por estado
        // Cuenta las citas por cada estado (Pendiente, Atendida y Cancelar) dentro de un rango de
        // fechas opcional. Siempre devuelve los tres estados, aunque alguno tenga 0, para que la
        // gráfica de pastel los muestre todos.
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

        // id_paciente → "Nombre Apellido". Sirve para mostrar el nombre del paciente en la tabla
        // de reportes y para buscar por nombre.
        public Dictionary<int, string> NombresDePacientes()
        {
            return pacienteRepo.ListarTodos()
                .ToDictionary(p => p.id_paciente, p => p.Nombre + " " + p.Apellido);
        }

        // id_Medico → "Nombre Apellido". Igual que el anterior pero para médicos.
        public Dictionary<int, string> NombresDeMedicos()
        {
            return medicoRepo.ListarTodos()
                .ToDictionary(m => m.id_Medico, m => m.Nombre + " " + m.Apellido);
        }

        // fecha = null: todas las fechas. estado = null: todos. nombre: busca en paciente o médico
        // Es el buscador del formulario de reportes. Primero filtro por fecha y estado con
        // GenerarReporteCitas (mando la misma fecha como inicio y fin para buscar un día exacto) y
        // después, si escribieron un nombre, me quedo con las citas cuyo paciente o médico
        // contienen ese texto. Paso todo a minúsculas para que no importe cómo lo escriban. Como
        // la cita solo trae ids, uso los diccionarios de nombres para comparar.
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
        // Prepara los datos de la gráfica de columnas de citas por mes o por día. Busca las citas
        // con BuscarCitas sin filtro de fecha (pero sí con estado y nombre). Si agrupan por "Mes",
        // agrupo por año y mes y devuelvo una barra por cada mes que tenga citas, en orden
        // cronológico. Si no, agrupo por día: tomo el mes de referencia (o el del último
        // registro) y devuelvo todos los días de ese mes, incluso los de 0 citas, para que el eje
        // se vea completo. El parámetro out titulo le devuelve al formulario el título que debe
        // llevar la gráfica.
        public Dictionary<string, int> CitasPorPeriodo(Administrador admin, string agrupar,
            DateTime? referencia, string estado, string nombre, out string titulo)
        {
            List<Cita> citas = BuscarCitas(admin, null, estado, nombre);   // sin filtro de fecha
            Dictionary<string, int> resultado = new Dictionary<string, int>();
            titulo = "Citas por mes";

            // Si no hay citas devuelvo vacío para que el formulario muestre la gráfica sin datos
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
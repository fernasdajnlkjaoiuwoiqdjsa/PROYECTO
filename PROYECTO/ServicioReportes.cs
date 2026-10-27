using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    public class ServicioReportes
    {
        private CitaRepositorio cita = new CitaRepositorio();


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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    public static class EstadosCita
    {
        public const string Pendiente = "Pendiente";
        public const string Atendida = "Atendida";
        public const string Cancelar = "Cancelar";

    }
    public class Cita
    {
        public int id_cita {  get; set; }

        public DateTime Fecha { get; set;}

        public DateTime Hora { get; set;}
        public string Estado { get; private set; }
        public int id_paciente { get; private set; }
        public int id_Medico { get; private set; }
        public Receta Receta { get; set; }

        public Cita(int idCita, DateTime fecha,DateTime hora,string estado, int idPaciente, int idMedico)
        {
            id_cita = idCita;
            Fecha = fecha;
            Hora = hora;
            Estado = estado;
            id_paciente = idPaciente;
            id_Medico = idMedico;
        }

        public Cita() 
        {
            Estado = EstadosCita.Pendiente;
        }
      
        public void Cancelar()
        {
            if (Estado != EstadosCita.Pendiente)
                throw new InvalidOperationException("Solo se puede cancelar una cita pendiente.");
            Estado = EstadosCita.Cancelar;
        }
        public void MarcarAtendida()
        {
            if (Estado != EstadosCita.Pendiente)
                throw new InvalidOperationException("Solo se puede atender una cita pendiente.");
            Estado = EstadosCita.Atendida;
        }
    }
}

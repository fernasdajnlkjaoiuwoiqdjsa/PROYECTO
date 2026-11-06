using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad
{
    //Interfaz que define los métodos que deben implementar las clases que la implementen
    public class Receta
    {
        //Propiedades de la clase Receta referenciando  a la base de datos
        public int id_receta {  get; set; }
        public string Descripcion { get; set; }
        // Propiedad de navegación que representa la relación con la clase Cita
        public int id_cita { get; private set; }
        //Constructor de la clase Receta con parámetros para inicializar las propiedades
        public Receta(int idreceta, string dscripcion, int idCita)
        {
            id_receta = idreceta;
            Descripcion = dscripcion;
            id_cita = idCita;
        }
        public Receta() 
        {

        }
        //Método para mostrar la información de la receta en formato de cadena
        public string Generar(Medico medico, Paciente paciente, Cita cita)
        {
            if (medico == null) throw new ArgumentNullException(nameof(medico));
            if (paciente == null) throw new ArgumentNullException(nameof(paciente));
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_cita != id_cita)
                throw new InvalidOperationException("La cita no corresponde a esta receta.");
            if (cita.id_paciente != paciente.id_paciente)
                throw new InvalidOperationException("Esta receta no pertenece a este paciente.");
            if (cita.id_Medico != medico.id_Medico)
                throw new InvalidOperationException("Esta receta no fue emitida por este médico.");
            // Generar la información de la receta en formato de cadena
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("RECETA MÉDICA");
            sb.AppendLine($"Paciente: {paciente.Nombre} {paciente.Apellido}");
            sb.AppendLine($"Médico: {medico.Nombre} {medico.Apellido}");
            sb.AppendLine($"Fecha: {cita.Fecha:dd/MM/yyyy}");
            sb.AppendLine($"Hora: {cita.Hora:HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("Indicaciones:");
            sb.AppendLine(Descripcion);
            return sb.ToString();
        }
    }
}

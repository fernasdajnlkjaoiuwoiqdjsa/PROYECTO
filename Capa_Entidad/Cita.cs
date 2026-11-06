using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad
{
    // Clase estática con los tres estados posibles de una cita. La hice para no escribir
    // "Pendiente" o "Atendida" a mano en cada lugar: si me equivoco en una letra, el compilador
    // me avisa en vez de fallar en silencio. La usan la clase Cita, el filtro de reportes
    // (Administrador.ReportesdeCitas), los dashboards y las gráficas. Son const porque los
    // valores nunca cambian mientras corre el programa.
    public static class EstadosCita
    {
        public const string Pendiente = "Pendiente";
        public const string Atendida = "Atendida";
        public const string Cancelar = "Cancelar";

    }

    // Representa una cita médica: el paciente, el médico, el día, la hora y en qué estado va.
    // Es la clase central del sistema porque casi todo gira alrededor de ella: agendar, atender,
    // cancelar, el historial, la receta y los reportes. Esta clase no habla con la base de datos,
    // solo guarda los datos y protege sus reglas; de guardar se encarga CitaRepositorio.
    public class Cita
    {
        // Identificador de la cita en la base de datos (llave primaria). Vale 0 mientras la cita
        // no se ha guardado; cuando se inserta, SQL Server le asigna el número real.
        public int id_cita { get; set; }

        // Día de la cita. Uso DateTime aunque solo me interese la fecha, y por eso en los
        // filtros siempre uso .Date para ignorar la parte de la hora.
        public DateTime Fecha { get; set; }

        // Hora de la cita. También es un DateTime (no un TimeSpan), así que cuando necesito
        // solo la hora uso .TimeOfDay, por ejemplo al ordenar en los reportes.
        public DateTime Hora { get; set; }

        // Estado actual de la cita (Pendiente, Atendida o Cancelar). Le puse private set a
        // propósito (encapsulamiento): nadie desde un formulario puede escribir
        // cita.Estado = "lo que sea", solo puede cambiarlo por medio de Cancelar() o
        // MarcarAtendida(), que revisan que el cambio tenga sentido.
        public string Estado { get; private set; }

        // A qué paciente pertenece la cita. Solo se puede asignar desde el constructor para que
        // una cita ya creada no "cambie de dueño" por accidente.
        public int id_paciente { get; private set; }

        // Qué médico la va a atender. Igual que el paciente, solo se asigna en el constructor.
        public int id_Medico { get; private set; }

        // La receta que el médico emite al atender la cita. Es una relación de composición:
        // la receta no tiene sentido sin su cita. Empieza en null y se llena solo cuando el
        // médico escribe la receta; por eso hay que revisar que no sea null antes de usarla.
        public Receta Receta { get; set; }

        // Constructor completo: recibe todos los datos de golpe. Lo uso sobre todo cuando el
        // repositorio lee una fila de la base de datos y arma el objeto con lo que ya estaba
        // guardado, incluyendo el estado y los ids de paciente y médico (que no tienen set
        // público). Solo asigna los valores, no valida nada.
        public Cita(int idCita, DateTime fecha, DateTime hora, string estado, int idPaciente, int idMedico)
        {
            id_cita = idCita;
            Fecha = fecha;
            Hora = hora;
            Estado = estado;
            id_paciente = idPaciente;
            id_Medico = idMedico;
        }

        // Constructor vacío para crear una cita nueva. Lo importante aquí es que toda cita
        // recién creada arranca en estado Pendiente, así no hay que acordarse de ponerlo cada
        // vez que se agenda una.
        public Cita()
        {
            Estado = EstadosCita.Pendiente;
        }

        // Cancela la cita. La regla es que solo se puede cancelar una que esté Pendiente: una
        // cita ya atendida o ya cancelada no tiene sentido cancelarla otra vez. Si no se cumple
        // lanza una excepción con mensaje claro y el formulario la muestra en un MessageBox.
        // Solo cambia el estado en memoria; para que quede guardado hay que llamar después al
        // repositorio (Actualizar). La usa Paciente.CancelarCita.
        public void Cancelar()
        {
            if (Estado != EstadosCita.Pendiente)
                throw new InvalidOperationException("Solo se puede cancelar una cita pendiente.");
            Estado = EstadosCita.Cancelar;
        }

        // Marca la cita como atendida cuando el médico termina la consulta. Misma idea que
        // Cancelar(): solo funciona si la cita está Pendiente, así no se puede "atender" una
        // cita cancelada ni atender dos veces la misma. La usa Medico.AtenderCita. Igual que
        // arriba, el cambio queda en memoria hasta que se guarde con el repositorio.
        public void MarcarAtendida()
        {
            if (Estado != EstadosCita.Pendiente)
                throw new InvalidOperationException("Solo se puede atender una cita pendiente.");
            Estado = EstadosCita.Atendida;
        }
    }
}
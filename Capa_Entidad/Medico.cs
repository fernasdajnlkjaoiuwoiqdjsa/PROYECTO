using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Entidad
{
    // El médico hereda de Usuario, así que ya trae username, contraseña, rol y activo, y aquí
    // solo agrego lo propio de un médico: su id, nombre, contactos, especialidad y las acciones
    // que puede hacer con las citas (ver su agenda, atender, emitir receta y ver su historial).
    // Igual que en las demás entidades, esta clase no toca la base de datos: recibe las listas
    // de citas ya cargadas y las filtra en memoria con LINQ.
    public class Medico : Usuario
    {
        // Id del médico en la tabla Medico. Es distinto de id_usuario (el de la cuenta de login),
        // y por eso en las citas se guarda este id para saber quién atiende.
        public int id_Medico { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        // Un médico puede tener varios teléfonos y correos, por eso son listas y no un solo texto.
        // Las inicializo con new List<string>() para que nunca sean null y no se caiga el programa
        // al recorrerlas o mostrarlas aunque no tenga datos.
        public List<string> Telefono { get; set; } = new List<string>();
        public List<string> Correo { get; set; } = new List<string>();
        // Llave foránea a la especialidad (por ejemplo Pediatría). Solo guardo el id; el nombre
        // de la especialidad lo busco aparte en la tabla Especialidad.
        public int id_especialidad { get; set; }

        // Constructor completo: recibe los datos del médico y los de su cuenta de usuario.
        // Los de la cuenta (idUsuario, username, password) se los paso a Usuario con base(...) y
        // el rol lo dejo fijo en "Medico" para que no se pueda crear un médico con otro rol.
        // Lo uso cuando el repositorio arma el objeto desde la base de datos, o el administrador
        // al registrar uno nuevo (con idMedico = 0 porque todavía no existe en la base).
        public Medico(int idMedico, string nombre, string apellido, List<string> telefonos, List<string> correos, int idEspecialidad, int idUsuario, string username, string password)
            : base(idUsuario, username, password, "Medico")
        {
            id_Medico = idMedico;
            Nombre = nombre;
            Apellido = apellido;
            // Si me mandan null en vez de una lista, creo una vacía con ?? para evitar errores después
            Telefono = telefonos ?? new List<string>();
            Correo = correos ?? new List<string>();
            id_especialidad = idEspecialidad;

        }


        // Devuelve la agenda del médico, o sea las citas que le tocan de hoy en adelante.
        // Es lo que ve en su formulario de agenda (FrmVerAgenda) y de donde salen las tarjetas
        // de su dashboard. Recibe la lista completa de citas y filtra solo las que son de este
        // médico, y ordena de la más próxima a la más lejana (primero por fecha y luego por hora).
        public List<Cita> VerAgenda(IEnumerable<Cita> citas)
        {
            // Agenda = mis citas de hoy en adelante, sin las canceladas
            return citas.Where(c => c.id_Medico == id_Medico
                                 && c.Fecha.Date >= DateTime.Today
                                 && c.Estado != EstadosCita.Cancelar)
                        .OrderBy(c => c.Fecha)
                        .ThenBy(c => c.Hora.TimeOfDay)
                        .ToList();
        }

        // Marca una cita como atendida. Es el paso que el médico hace con el botón de atender.
        // Antes de hacerlo reviso dos cosas: que la cita exista (no sea null) y que sea de este
        // médico, porque no debería poder atender las citas de otro. El cambio de estado como
        // tal lo hace Cita.MarcarAtendida(), que además comprueba que esté Pendiente. Ojo: solo
        // cambia el objeto en memoria; guardarlo en la base lo hace el servicio con el repositorio.
        public void AtenderCita(Cita cita)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_Medico != id_Medico)
                throw new InvalidOperationException("Esa cita no está asignada a este médico.");

            cita.MarcarAtendida();    // Cita valida que esté Pendiente
        }

        // Crea la receta de una cita y se la deja dentro a la cita. Es importante porque de aquí
        // sale lo que el paciente después ve y descarga en PDF. Pongo varias reglas antes de
        // crearla: la cita debe existir, ser de este médico, estar ya Atendida (no se receta
        // antes de la consulta) y no tener receta todavía (una receta por cita). Si todo está
        // bien, creo la Receta con id 0 (aún no está en la base) y la devuelvo para que el
        // servicio la guarde con el repositorio de recetas.
        public Receta EmitirReceta(Cita cita, string descripcion)
        {
            if (cita == null) throw new ArgumentNullException(nameof(cita));
            if (cita.id_Medico != id_Medico)
                throw new InvalidOperationException("Esa cita no está asignada a este médico.");
            if (cita.Estado != EstadosCita.Atendida)
                throw new InvalidOperationException("Solo se emite receta de una cita atendida.");
            if (cita.Receta != null)
                throw new InvalidOperationException("Esta cita ya tiene una receta.");


            Receta receta = new Receta(0, descripcion, cita.id_cita);
            cita.Receta = receta;     // composición la receta queda dentro de la cita
            return receta;
        }

        // Devuelve las citas que este médico ya atendió, de la más reciente a la más antigua.
        // Es su historial de pacientes atendidos. Filtro por su id y por estado Atendida, y las
        // ordeno por fecha y hora de forma descendente para que arriba salga lo último que hizo.
        public List<Cita> HistorialPacientesAtendidos(IEnumerable<Cita> citas)
        {
            return citas.Where(c => c.id_Medico == id_Medico
                                 && c.Estado == EstadosCita.Atendida)
                        .OrderByDescending(c => c.Fecha)
                        .ThenByDescending(c => c.Hora.TimeOfDay)
                        .ToList();
        }

    }
}
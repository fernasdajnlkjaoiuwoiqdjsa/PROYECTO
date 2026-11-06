using Capoa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Entidad
{
    // Enumeración con las tres cosas que el administrador puede hacerle a un registro.
    // La hice para no andar pasando strings como "registrar" o "eliminar", que si me
    // equivoco al escribirlos el compilador no me avisa. Así el método Gestionar... recibe
    // la operación y decide con un switch qué hacer.
    public enum Operacion { Registrar, Actualizar, Eliminar }

    // Administrador hereda de Usuario, por eso ya trae username, password, rol y activo.
    // No le puse atributos propios porque en la base de datos no tiene tabla propia, solo es
    // un usuario con Rol = "Administrador". Lo que lo hace distinto son sus métodos: todas las
    // acciones de gestión (pacientes, médicos, especialidades, usuarios) y los reportes.
    // Aquí vive la lógica de las reglas del administrador: valida los datos y recién después
    // le pide al repositorio que guarde, así el repositorio solo se encarga del SQL.
    public class Administrador : Usuario
    {
        // Constructor: recibe el id, el username y la contraseña, y se los manda al constructor
        // de Usuario con base(...). El rol lo dejé fijo en "Administrador" para que nadie pueda
        // crear un administrador con otro rol por error. El cuerpo queda vacío porque todo el
        // trabajo de guardar esos datos ya lo hace la clase padre.
        public Administrador(int idUsuario, string useername, string password)
            : base(idUsuario, useername, password, "Administrador")
        {

        }

        // Genera la lista de citas para el reporte. Es importante porque es el filtro que usa
        // ServicioReportes (GenerarReporteCitas) y de ahí salen la tabla, el PDF y las gráficas.
        // No consulta la base de datos: recibe la lista de citas que ya trajo el repositorio y
        // solo la filtra en memoria con LINQ. Los tres filtros son opcionales: si no mando
        // fecha o estado, simplemente no se aplica ese filtro.
        public List<Cita> ReportesdeCitas(IEnumerable<Cita> citas, DateTime? desde, DateTime? hasta, string estado)
        {
            // Arranco con todas las citas y le voy quitando las que no cumplen cada filtro
            IEnumerable<Cita> resultado = citas;

            // Filtro por fecha inicial: me quedo con las citas de esa fecha en adelante
            if (desde.HasValue) resultado = resultado.Where(c => c.Fecha.Date >= desde.Value.Date);
            // Aquí va el filtro de la fecha final (hasta)
            if (hasta.HasValue) resultado = resultado.Where(c => c.Fecha.Date >= desde.Value.Date);
            // Filtro por estado (Pendiente, Atendida o Cancelada); si viene vacío se ignora
            if (!string.IsNullOrEmpty(estado)) resultado = resultado.Where(c => c.Estado == estado);

            // Devuelvo las más recientes primero: ordeno por fecha y, si hay empate, por la hora
            return resultado.OrderByDescending(c => c.Fecha)
                            .ThenByDescending(c => c.Hora.TimeOfDay)
                            .ToList();
        }

        // Registrar, actualizar o eliminar un paciente. Es el método que usa el formulario de
        // gestión de pacientes. Lo importante es que aquí se validan los datos antes de tocar la
        // base de datos, para que no se guarde basura. Recibe IRepositorio<Paciente> y no
        // PacienteRepositorio directamente, así este método no depende de cómo se guarda (abstracción).
        // Llama a ValidarPersona, ValidarFechaNacimiento, RegistrarUsuario y al repositorio
        // (Guardar, Actualizar o Eliminar) según la operación.
        public void GestionarPacintes(Operacion operacion, Paciente paciente, IRepositorio<Paciente> repositorio)
        {
            // Primero reviso que no me lleguen objetos vacíos, si no se caería más adelante
            if (paciente == null) throw new ArgumentNullException(nameof(paciente));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));

            // Según la operación que pidió el administrador hago una cosa u otra
            switch (operacion)
            {
                case Operacion.Registrar:
                    // Valido nombre, apellido, teléfonos y correos (los junto en un solo texto para revisarlos)
                    ValidarPersona(paciente.Nombre, paciente.Apellido, string.Join(", ", paciente.Telefono), string.Join(", ", paciente.Correo));
                    ValidarFechaNacimiento(paciente.FechaNacimineto);
                    RegistrarUsuario(paciente);              // el <<include>> del caso de uso
                    repositorio.Guardar(paciente);           // ya validado, ahora sí lo mando a la base
                    break;

                case Operacion.Actualizar:
                    // Para actualizar el paciente ya tiene que existir, por eso exijo un id válido
                    ExigirID(paciente.id_paciente);
                    ValidarPersona(paciente.Nombre, paciente.Apellido, string.Join(", ", paciente.Telefono), string.Join(", ", paciente.Correo));
                    ValidarFechaNacimiento(paciente.FechaNacimineto);
                    repositorio.Actualizar(paciente);
                    break;

                case Operacion.Eliminar:
                    // Para eliminar solo necesito el id, no vale la pena validar nombre ni teléfono
                    ExigirID(paciente.id_paciente);
                    repositorio.Eliminar(paciente.id_paciente);
                    break;
            }
        }

        // Valida los datos de la cuenta (usuario, contraseña y rol) antes de crear al paciente o
        // al médico. Ojo: este método NO guarda nada en la base de datos, solo revisa los datos y
        // le quita los espacios sobrantes al username. El guardado real lo hace el repositorio
        // cuando el Gestionar... llama a Guardar. Lo separé en su propio método porque pacientes
        // y médicos necesitan exactamente las mismas reglas y así no repito el código.
        public void RegistrarUsuario(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            // El username no puede venir vacío ni con puros espacios
            if (string.IsNullOrWhiteSpace(usuario.Username))
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            // Puse un mínimo de 4 caracteres para que la contraseña no sea tan débil
            if (string.IsNullOrWhiteSpace(usuario.password) || usuario.password.Length < 4)
                throw new ArgumentException("La contraseña debe tener al menos 4 caracteres.");
            // Todo usuario necesita un rol, si no el sistema no sabría a qué menú mandarlo
            if (string.IsNullOrWhiteSpace(usuario.Rol))
                throw new ArgumentException("El usuario necesita un rol.");

            // Limpio los espacios al inicio y al final para que "maria " y "maria" no sean distintos
            usuario.Username = usuario.Username.Trim();
        }

        // Igual que GestionarPacintes pero para los médicos. La diferencia es que un médico
        // siempre tiene que tener una especialidad asignada, entonces aquí también reviso eso.
        // Llama a ValidarPersona, RegistrarUsuario, ExigirID y al repositorio de médicos.
        // Es importante porque un médico sin especialidad no podría aparecer al agendar citas.
        public void GestionarMedicos(Operacion operacion, Medico medico, IRepositorio<Medico> reposirotio)
        {
            if (medico == null) throw new ArgumentNullException(nameof(medico));
            if (reposirotio == null) throw new ArgumentNullException(nameof(reposirotio));

            switch (operacion)
            {
                case Operacion.Registrar:
                    ValidarPersona(medico.Nombre, medico.Apellido, string.Join(", ", medico.Telefono), string.Join(", ", medico.Correo));
                    // La especialidad es obligatoria: si el id es 0 es que no eligieron ninguna
                    if (medico.id_especialidad <= 0)
                        throw new ArgumentException("Debe asignar una especialidad al medico");
                    RegistrarUsuario(medico);
                    reposirotio.Guardar(medico);
                    break;


                case Operacion.Actualizar:
                    // El médico ya debe existir en la base para poder modificarlo
                    ExigirID(medico.id_Medico);
                    ValidarPersona(medico.Nombre, medico.Apellido, string.Join(", ", medico.Telefono), string.Join(", ", medico.Correo));
                    if (medico.id_especialidad <= 0)
                        throw new ArgumentException("Debe asignar una especialidad al medico");
                    RegistrarUsuario(medico);
                    reposirotio.Actualizar(medico);
                    break;


                case Operacion.Eliminar:
                    ExigirID(medico.id_Medico);
                    reposirotio.Eliminar(medico.id_Medico);
                    break;
            }

        }

        // Le asigna una especialidad a un médico. Solo cambia el id_especialidad del objeto en
        // memoria, no guarda nada en la base de datos; el guardado ocurre después, cuando se
        // llama a GestionarMedicos con Actualizar. Reviso que la especialidad ya exista (que tenga
        // id) porque no tiene sentido asignarle a un médico una especialidad que no está guardada.
        public void AsignarEspecialidad(Medico medico, Especialidad especialidad)
        {
            if (medico == null) throw new ArgumentNullException(nameof(medico));
            if (especialidad == null) throw new ArgumentNullException(nameof(especialidad));
            ExigirID(especialidad.id_especialidad);
            medico.id_especialidad = especialidad.id_especialidad;

        }

        // Registrar, actualizar o eliminar una especialidad del catálogo. Es el más sencillo de
        // los tres Gestionar... porque una especialidad solo tiene nombre y no necesité más
        // validaciones, solo reviso que no vengan objetos nulos y que el id exista cuando
        // actualizo o elimino. Llama al repositorio que le pasen (IRepositorio<Especialidad>).
        public void GestionarEspecialidad(Operacion operacion, Especialidad especialidad,
                                          IRepositorio<Especialidad> repositorio)
        {
            if (especialidad == null) throw new ArgumentNullException(nameof(especialidad));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));

            switch (operacion)
            {
                case Operacion.Registrar:

                    repositorio.Guardar(especialidad);
                    break;

                case Operacion.Actualizar:
                    ExigirID(especialidad.id_especialidad);

                    repositorio.Actualizar(especialidad);
                    break;

                case Operacion.Eliminar:
                    ExigirID(especialidad.id_especialidad);
                    repositorio.Eliminar(especialidad.id_especialidad);
                    break;
            }
        }

        // Para modificar los datos de una cuenta de usuario ya existente (username y contraseña).
        // Se usa en el formulario de gestionar usuarios. Primero valida el id, luego limpia el
        // username y revisa la contraseña, y al final llama a repositorio.Actualizar para
        // guardar los cambios. El parámetro nuevacontraseña es opcional (por eso lleva = null).
        public void GestionarUsuarios(Usuario usuario, IRepositorio<Usuario> repositorio, string nuevacontraseña = null)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));
            // La cuenta tiene que existir ya en la base de datos para poder modificarla
            ExigirID(usuario.id_usuario);

            if (string.IsNullOrWhiteSpace(usuario.Username))
                throw new ArgumentException("El nombre de usuario debe ir");
            usuario.Username = usuario.Username.Trim();

            if (nuevacontraseña != null)
                throw new ArgumentException("La contraseña de usuario debe ir");
            usuario.password = usuario.password.Trim();

            // Con todo revisado, mando a guardar los cambios
            repositorio.Actualizar(usuario);

        }

        //Creacion de metodo estatico para ver si el registro existe o no en la base de datos
        // Lo uso en casi todos los métodos de arriba. La idea es simple: cuando un objeto todavía
        // no se ha guardado en la base, su id vale 0, y si se intenta actualizar o eliminar algo
        // que no existe daría un error raro. Con esto lanzo un mensaje claro antes de llegar al SQL.
        private static void ExigirID(int id)
        {
            if (id <= 0)
                throw new InvalidOperationException("Ese registro todavía no existe en la base de datos.");
        }
        //Cracion de metodod privado para validar con condicionales los requisitos principales de una persona (nombre,apellido,telefono,correo)
        // Lo compartimos entre pacientes y médicos porque piden lo mismo. Lanza una excepción con
        // un mensaje en cuanto algo está mal, y el formulario la atrapa y se la muestra al usuario
        // en un MessageBox. Revisa que nombre y apellido no estén vacíos, que el teléfono sean
        // solo dígitos con máximo 9 y que el correo tenga una arroba.
        private void ValidarPersona(string nombre, string apellido, string telefono, string correo)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("Nombre y apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(telefono) || !telefono.Trim().All(char.IsDigit)
                || telefono.Trim().Length > 9)
                throw new ArgumentException("El teléfono debe tener solo 9 digitos.");
            if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                throw new ArgumentException("El correo no es válido.");
        }

        // Revisa que la fecha de nacimiento tenga sentido: una persona no puede haber nacido hoy
        // ni en el futuro. Solo se usa al registrar o actualizar pacientes, porque los médicos
        // no tienen fecha de nacimiento en mi base de datos.
        private static void ValidarFechaNacimiento(DateTime fecha)
        {
            if (fecha.Date >= DateTime.Today)
                throw new ArgumentException("La fecha de nacimiento debe ser anterior a hoy.");
        }
        //  Activar o desactivar una cuenta (no borra datos) 
        // Prefiero desactivar y no borrar al usuario, así se conserva su historial de citas y recetas.
        // Cuando una cuenta está inactiva, el login la rechaza. Valida que la cuenta exista y que
        // el administrador no se desactive a sí mismo (si no se quedaría sin poder entrar al
        // sistema). Cambia la propiedad Activo y llama a repositorio.Actualizar para guardarlo.
        public void CambiarEstadoCuenta(Usuario usuario, bool activo, IRepositorio<Usuario> repositorio)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));
            ExigirID(usuario.id_usuario);

            // this.id_usuario es el del administrador que está haciendo la acción
            if (!activo && usuario.id_usuario == this.id_usuario)
                throw new InvalidOperationException("No puedes desactivar tu propia cuenta.");

            usuario.Activo = activo;
            repositorio.Actualizar(usuario);
        }




    }
}
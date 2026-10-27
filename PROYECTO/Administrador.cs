using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    public enum Operacion { Registrar, Actualizar, Eliminar }

    public class Administrador:Usuario
    {
        public Administrador(int idUsuario, string useername, string password)
            :base(idUsuario, useername, password, "Administrador")
        {
        
        }
    
        public List<Cita> ReportesdeCitas(IEnumerable<Cita> citas, DateTime? desde, DateTime? hasta, string estado)
        {
            IEnumerable<Cita> resultado = citas;

            if (desde.HasValue) resultado = resultado.Where(c => c.Fecha.Date >= desde.Value.Date);
            if (hasta.HasValue) resultado = resultado.Where(c => c.Fecha.Date >= desde.Value.Date);
            if (!string.IsNullOrEmpty(estado)) resultado = resultado.Where(c => c.Estado == estado);

            return resultado.OrderByDescending(c => c.Fecha)
                            .ThenByDescending(c => c.Hora.TimeOfDay)
                            .ToList();
        }
        public void GestionarPacintes(Operacion operacion, Paciente paciente,IRepositorio<Paciente> repositorio)
        {
            if (paciente == null) throw new ArgumentNullException(nameof(paciente));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));

            switch (operacion)
            {
                case Operacion.Registrar:
                    ValidarPersona(paciente.Nombre, paciente.Apellido, string.Join(", ", paciente.Telefono), string.Join(", ", paciente.Correo));
                    ValidarFechaNacimiento(paciente.FechaNacimineto);
                    RegistrarUsuario(paciente);              // el <<include>> del caso de uso
                    repositorio.Guardar(paciente);
                    break;

                case Operacion.Actualizar:
                    ExigirID(paciente.id_paciente);
                    ValidarPersona(paciente.Nombre, paciente.Apellido, string.Join(", ", paciente.Telefono), string.Join(", ", paciente.Correo));
                    ValidarFechaNacimiento(paciente.FechaNacimineto);
                    repositorio.Actualizar(paciente);
                    break;

                case Operacion.Eliminar:
                    ExigirID(paciente.id_paciente);
                    repositorio.Eliminar(paciente.id_paciente);
                    break;
            }
        }
        public void RegistrarUsuario(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            if (string.IsNullOrWhiteSpace(usuario.Username))
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuario.password) || usuario.password.Length < 4)
                throw new ArgumentException("La contraseña debe tener al menos 4 caracteres.");
            if (string.IsNullOrWhiteSpace(usuario.Rol))
                throw new ArgumentException("El usuario necesita un rol.");

            usuario.Username = usuario.Username.Trim();
        }
        public void GestionarMedicos(Operacion operacion, Medico medico, IRepositorio<Medico> reposirotio)
        {
            if(medico == null) throw new ArgumentNullException(nameof(medico));
            if (reposirotio == null) throw new ArgumentNullException(nameof(reposirotio));

            switch (operacion)
            {
                case Operacion.Registrar:
                    ValidarPersona(medico.Nombre, medico.Apellido, string.Join(", ", medico.Telefono), string.Join(", ", medico.Correo));
                    if (medico.id_especialidad <= 0)
                        throw new ArgumentException("Debe asignar una especialidad al medico");
                    RegistrarUsuario(medico);
                    reposirotio.Guardar(medico);
                    break;


                case Operacion.Actualizar:
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
        public void AsignarEspecialidad(Medico medico, Especialidad especialidad)
        {
            if (medico == null) throw new ArgumentNullException(nameof(medico));
            if (especialidad == null) throw new ArgumentNullException(nameof(especialidad));
            ExigirID(especialidad.id_especialidad);
            medico.id_especialidad = especialidad.id_especialidad;

        }
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
        public void GestionarUsuarios(Usuario usuario, IRepositorio<Usuario> repositorio,string nuevacontraseña=null)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));
            ExigirID(usuario.id_usuario);

            if (string.IsNullOrWhiteSpace(usuario.Username))
                throw new ArgumentException("El nombre de usuario debe ir");
            usuario.Username = usuario.Username.Trim();

            if(nuevacontraseña != null)
                throw new ArgumentException("La contraseña de usuario debe ir");
            usuario.password = usuario.password.Trim();

            repositorio.Actualizar(usuario);

        }

        //Creacion de metodo estatico para ver si el registro existe o no en la base de datos
        private static void ExigirID(int id)
        {
            if (id <= 0)
                throw new InvalidOperationException("Ese registro todavía no existe en la base de datos.");
        }
        //Cracion de metodod privado para validar con condicionales los requisitos principales de una persona (nombre,apellido,telefono,correo)
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
        private static void ValidarFechaNacimiento(DateTime fecha)
        {
            if (fecha.Date >= DateTime.Today)
                throw new ArgumentException("La fecha de nacimiento debe ser anterior a hoy.");
        }
        //  Activar o desactivar una cuenta (no borra datos) 
        public void CambiarEstadoCuenta(Usuario usuario, bool activo, IRepositorio<Usuario> repositorio)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));
            if (repositorio == null) throw new ArgumentNullException(nameof(repositorio));
            ExigirID(usuario.id_usuario);

            if (!activo && usuario.id_usuario == this.id_usuario)
                throw new InvalidOperationException("No puedes desactivar tu propia cuenta.");

            usuario.Activo = activo;
            repositorio.Actualizar(usuario);
        }




    }
}

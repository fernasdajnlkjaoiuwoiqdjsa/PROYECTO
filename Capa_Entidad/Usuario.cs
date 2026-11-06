using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad
{
    // Clase base de todos los usuarios del sistema. Paciente, Medico y Administrador heredan de
    // aquí, y así no repito en cada una los datos de la cuenta (id, username, contraseña, rol) ni
    // la lógica de iniciar y cerrar sesión. Es la parte de herencia del proyecto. No habla con la
    // base de datos: el repositorio de usuarios carga los datos y ServicioAutenticacion la usa.
    public class Usuario
    {
        // Id de la cuenta en la tabla Usuario (llave primaria). Vale 0 mientras no se ha guardado.
        public int id_usuario { get; set; }
        // Nombre con el que la persona entra al login.
        public string Username { get; set; }
        // Contraseña de la cuenta. Aquí la manejo como texto normal porque es un proyecto de clase,
        // en un sistema real tendría que ir cifrada con hash.
        public string password { get; set; }
        // Rol de la cuenta ("Paciente", "Medico" o "Administrador"). Con esto el login decide a
        // qué menú mandar a la persona.
        public string Rol { get; set; }
        // Indica si la cuenta está habilitada. Empieza en true para que toda cuenta nueva nazca
        // activa; el administrador la puede desactivar sin borrar nada (CambiarEstadoCuenta).
        public bool Activo { get; set; } = true;

        // Constructor de la clase base: recibe los cuatro datos de la cuenta y los guarda. Las
        // clases hijas lo llaman con base(...) y le mandan su rol fijo. Los nombres de los
        // parámetros (id_usu, uusario) los dejo como los escribí.
        public Usuario(int id_usu, string uusario, string contraseña, string rol)
        {
            id_usuario = id_usu;
            Username = uusario;
            password = contraseña;
            Rol = rol;

        }

        // Dice si la sesión de este usuario está abierta. Tiene private set para que nadie desde un
        // formulario pueda poner SesionActiva = true sin pasar por IniciarSesion (encapsulamiento).
        public bool SesionActiva { get; private set; }

        // Revisa si el usuario y la clave que escribieron en el login coinciden con los de este
        // objeto. No consulta la base: el repositorio ya trajo el usuario y aquí solo comparo.
        // La usa IniciarSesion, y devuelve true o false.
        public bool ValidarCredenciales(string usuario, string clave)
        {
            // Compara contra los datos de ESTE objeto (ya cargado desde la BD por el repositorio).
            return Username == usuario && password == clave;
        }

        // Intenta abrir la sesión: llama a ValidarCredenciales y guarda el resultado en
        // SesionActiva (true si coinciden, false si no). También devuelve ese resultado para que
        // el login sepa si dejar entrar o mostrar el mensaje de error.
        public bool IniciarSesion(string usuario, string clave)
        {
            SesionActiva = ValidarCredenciales(usuario, clave);
            return SesionActiva;
        }

        // Cierra la sesión poniendo SesionActiva en false. Es lo que se usa al presionar el botón
        // de cerrar sesión, antes de volver al login.
        public void CerrarSesion()
        {
            SesionActiva = false;
        }
    }


}
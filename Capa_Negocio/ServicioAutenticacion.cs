using Capa_Datos;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio
{
    // Servicio de autenticación: es la capa de negocio del login. El formulario de login no habla
    // directo con la base de datos, le pide a este servicio que valide al usuario, y este usa el
    // repositorio para buscarlo y la entidad Usuario para comparar la contraseña. Así el
    // formulario solo se ocupa de la pantalla (Presentación → Negocio → Datos).
    public class ServicioAutenticacion
    {
        // Repositorio que uso para buscar la cuenta en la base de datos por su username.
        private UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();

        //Devuelve el usuario(paciente,medico o administrador) o un valor nulo si no coinciden

        // Valida lo que escribieron en el login (el nombre lleva el typo "Valirdar" y lo dejo igual
        // porque el formulario lo llama así). Primero reviso que no manden usuario o contraseña
        // vacíos y, si es así, lanzo una excepción con un mensaje que el login muestra. Luego busco
        // la cuenta con BuscarPorUsername: si no existe devuelvo null. Si existe, llamo a
        // IniciarSesion de la entidad, que compara la contraseña y deja SesionActiva en true o
        // false. Devuelve el usuario ya armado con su subclase (Paciente, Medico o Administrador)
        // si todo coincide, o null si no, y con eso el login decide a qué menú mandarlo.
        public Usuario ValirdarCredenciales(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                throw new System.ArgumentException("Escribe tu usuario y tu contraseña.");

            Usuario u = usuarioRepo.BuscarPorUsername(username);
            if (u == null) return null;

            // SQL ya comparó el username sin distinguir mayúsculas aquí solo se valida la contraseña
            return u.IniciarSesion(u.Username, password) ? u : null;
        }

        // Devuelve solo el rol ("Paciente", "Medico" o "Administrador") de un username, o null si
        // no existe. Sirve cuando solo necesito saber a qué menú pertenece la persona sin validar
        // la contraseña. El ?. evita el error si la búsqueda devolvió null.
        public string RolUsuario(string username)
        {
            Usuario u = usuarioRepo.BuscarPorUsername(username);
            return u?.Rol;
        }
    }
}
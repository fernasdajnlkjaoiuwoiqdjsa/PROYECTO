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
    public class ServicioAutenticacion
    {
        private UsuarioRepositorio usuarioRepo = new UsuarioRepositorio();

        //Devuelve el usuario(paciente,medico o administrador) o un valor nulo si no coinciden

        public Usuario ValirdarCredenciales(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                throw new System.ArgumentException("Escribe tu usuario y tu contraseña.");

            Usuario u = usuarioRepo.BuscarPorUsername(username);
            if (u == null) return null;

            // SQL ya comparó el username sin distinguir mayúsculas aquí solo se valida la contraseña
            return u.IniciarSesion(u.Username, password) ? u : null;
        }

        public string RolUsuario(string username)
        {
            Usuario u = usuarioRepo.BuscarPorUsername(username);
            return u?.Rol;
        }
    }
}

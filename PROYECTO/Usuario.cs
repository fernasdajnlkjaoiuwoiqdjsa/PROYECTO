using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    public class Usuario
    {
        public int id_usuario {  get; set; }
        public string Username { get ; set; }
        public string password { get; set; }
        public string Rol {  get; set; }
        public bool Activo { get; set; } = true;

        public Usuario(int id_usu,string uusario, string contraseña, string rol)
        {
            id_usuario = id_usu;
            Username = uusario;
            password = contraseña;
            Rol = rol;
        
        }

        public bool SesionActiva { get; private set; }

        public bool ValidarCredenciales(string usuario, string clave)
        {
            // Compara contra los datos de ESTE objeto (ya cargado desde la BD por el repositorio).
            return Username == usuario && password == clave;   
        }

        public bool IniciarSesion(string usuario, string clave)
        {
            SesionActiva = ValidarCredenciales(usuario, clave);
            return SesionActiva;
        }

        public void CerrarSesion()
        {
            SesionActiva = false;
        }
    }


}

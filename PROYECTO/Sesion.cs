using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO
{
    //Creacion de esta clase para conectar los menus y que sepan quien inicio la sesion
    public static class Sesion
    {

        public static Usuario UsuarioActual {  get; set; }

        public static void Cerrar()
        {
            if (UsuarioActual != null)  UsuarioActual.CerrarSesion();
            UsuarioActual = null;
        }
    }
}

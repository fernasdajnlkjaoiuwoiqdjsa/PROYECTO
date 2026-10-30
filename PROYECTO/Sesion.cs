using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace PROYECTO
{
    //Creacion de esta clase para conectar los menus y que sepan quien inicio la sesion
    public static class Sesion
    {

        public static Usuario UsuarioActual {  get; set; }
        public static Form VentanaLogin { get; set; }
        public static void CerrarYVolverAlLogin()
        {
            if (MessageBox.Show("¿Deseas cerrar sesión?", "Cerrar sesión",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            Cerrar();   // UsuarioActual pasa a null

            // Esconde todas las ventanas de la sesión anterior, menos el login
            foreach (Form f in Application.OpenForms.Cast<Form>().ToList())
            {
                if (f != VentanaLogin) f.Hide();
            }

            if (VentanaLogin != null)
            {
                VentanaLogin.Show();
                VentanaLogin.Activate();
            }
        }

        public static void Cerrar()
        {
            if (UsuarioActual != null)  UsuarioActual.CerrarSesion();
            UsuarioActual = null;
        }
    }
}

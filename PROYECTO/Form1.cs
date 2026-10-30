using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;   // ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            this.VisibleChanged += (s, ev) =>
            {
                if (Visible)
                {
                    txtUsuario.Clear();
                    txtContraseña.Clear();
                    txtUsuario.Focus();
                }
            };
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {

                Usuario u = new ServicioAutenticacion()
                            .ValirdarCredenciales(txtUsuario.Text, txtContraseña.Text);

                if (u == null)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Sesion.UsuarioActual = u;
                Sesion.VentanaLogin = this;

                Form menu;
                if (u is Paciente) menu = new FrmMenuPacient();
                else if (u is Medico) menu = new FmrMenuMedico();
                else menu = new FrmMenuAdministrador();

                menu.FormClosed += (s, ev) => Environment.Exit(0); //Cerra el menu tambien la app
                menu.Show();
                this.Hide();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo iniciar sesión",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}

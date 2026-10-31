using Capa_Entidad;
using Capa_Negocio;
using Capa_Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO
{
    public partial class FrmEnviarRecordatorio : Form
    {
        private Medico medico;

        // Se ejecuta al abrir el formulario (no hace falta crear el evento Load en el diseñador)
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            medico = Sesion.UsuarioActual as Medico;
            if (medico == null)
            {
                MessageBox.Show("Debes iniciar sesión como médico.");
                Close();
                return;
            }

            // Mensaje sugerido, el médico puede editarlo
            txtMensaje.Text = "Estimado paciente, le recordamos que tenía una cita programada " +
                              "y no pudo asistir. Por favor comuníquese con nosotros para reprogramarla.";

            // Evento enlazado por código: NO lo crees también desde el diseñador
            btnEnviarCorreo.Click += btnEnviar_Click;
        }

        private async void btnEnviar_Click(object sender, EventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string mensaje = txtMensaje.Text.Trim();

            if (correo == "" || mensaje == "")
            {
                MessageBox.Show("Escribe el correo del paciente y el mensaje.");
                return;
            }

            try { new MailAddress(correo); }
            catch (FormatException)
            {
                MessageBox.Show("El correo del paciente no es válido.");
                return;
            }

            btnEnviarCorreo.Enabled = false;
            try
            {
                string nombre = "Dr(a). " + medico.Nombre + " " + medico.Apellido;
                await Task.Run(() => ServicioCorreo.EnviarRecordatorio(correo, nombre, mensaje));

                MessageBox.Show("Recordatorio enviado.");
                txtCorreo.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo enviar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                btnEnviarCorreo.Enabled = true;
            }
        }
        public FrmEnviarRecordatorio()
        {
            InitializeComponent();
        }

        private void FrmEnviarRecordatorio_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            FmrMenuMedico frmPaciente = new FmrMenuMedico();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmVerAgenda frmPaciente = new FrmVerAgenda();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            FrmPacientesAtendidos frmPaciente = new FrmPacientesAtendidos();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmAtenderCita frmPaciente = new FrmAtenderCita();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Sesion.CerrarYVolverAlLogin();
        }
    }
}

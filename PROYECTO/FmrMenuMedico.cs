using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;
using Capa_Datos;// ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    public partial class FmrMenuMedico : Form
    {
        private void CargarDashboardMedico()
        {
            Medico med = Sesion.UsuarioActual as Medico;
            if (med == null) return;
            lblMenuMedico.Text = "Bienvenido, " + med.Username;


            try
            {
                ServicioCitas servicio = new ServicioCitas();

                // Tarjetas
                Dictionary<string, int> r = servicio.ResumenMedico(med);
                lblTarjetaHoy.Text = r["CitasHoy"].ToString();
                lblTarjetaPendientes.Text = r["Pendientes"].ToString();
                lblTarjetaAtendidasHoy.Text = r["AtendidasHoy"].ToString();

                // Próximas citas: una fila por cita pendiente
                Control[] textos = { lblCita1, lblCita2, lblCita3 };
                Guna2Button[] estados = { btnEstado1, btnEstado2, btnEstado3 };

                List<Cita> proximas = servicio.ProximasCitasMedico(med, textos.Length);
                Dictionary<int, string> pacientes = servicio.NombresDePacientes();

                for (int i = 0; i < textos.Length; i++)
                {
                    if (i < proximas.Count)
                    {
                        Cita c = proximas[i];
                        string nombre = pacientes.ContainsKey(c.id_paciente) ? pacientes[c.id_paciente] : "Paciente";
                        string dia = c.Fecha.Date == DateTime.Today ? "" : c.Fecha.ToString("dd/MM") + " ";

                        textos[i].Text = dia + c.Hora.ToString("h:mm tt") + " - " + nombre;
                        textos[i].Visible = true;

                        estados[i].Text = c.Estado;
                        estados[i].FillColor = Color.FromArgb(255, 228, 181);
                        estados[i].ForeColor = Color.FromArgb(120, 80, 0);
                        estados[i].HoverState.FillColor = Color.FromArgb(255, 228, 181);
                        estados[i].Visible = true;
                    }
                    else
                    {
                        textos[i].Visible = false;
                        estados[i].Visible = false;
                    }
                }

                if (proximas.Count == 0)
                {
                    textos[0].Text = "No tienes citas pendientes.";
                    textos[0].Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar el dashboard");
            }
        }
        public FmrMenuMedico()
        {
            InitializeComponent();

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarDashboardMedico();
        }

        private void Guna2Button1_Click(object sender, EventArgs e)
        {
      
        }

        private void Guna2Button2_Click(object sender, EventArgs e)
        {

        }

        private void Guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void Guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void ConfigurarBotonMenu(Guna.UI2.WinForms.Guna2Button boton)
        {
            boton.FillColor = Color.FromArgb(55, 138, 221); // el azul de tu sidebar
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 11F, FontStyle.Regular);

            boton.TextAlign = HorizontalAlignment.Left;
            boton.ImageAlign = HorizontalAlignment.Left;
            boton.Padding = new Padding(20, 0, 0, 0);

            boton.BorderRadius = 0; // sin bordes redondeados en el sidebar, para que se vea como un bloque solido
          
            boton.Height = 50;

            // Efecto hover (al pasar el mouse)
            boton.HoverState.FillColor = Color.FromArgb(45, 120, 200);
        }
        private void MarcarActivo(Guna.UI2.WinForms.Guna2Button botonActivo)
        {
           
            // Resalta el activo con un tono mas claro/transparente
            botonActivo.FillColor = Color.FromArgb(80, 255, 255, 255);
        }

        private void FmrMenuMedico_Load(object sender, EventArgs e)
        {
            

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            FrmVerAgenda frmMedico = new FrmVerAgenda();
            frmMedico.Show();
            this.Hide();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            FrmPacientesAtendidos frmMedico = new FrmPacientesAtendidos();
            frmMedico.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmAtenderCita frmMedico = new FrmAtenderCita();
            frmMedico.Show();
            this.Hide();
        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2Button6_Click_1(object sender, EventArgs e)
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

        private void guna2Button9_Click_1(object sender, EventArgs e)
        {
            FrmAtenderCita frmPaciente = new FrmAtenderCita();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            Sesion.CerrarYVolverAlLogin();

        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            FrmEnviarRecordatorio frmPaciente = new FrmEnviarRecordatorio();
            frmPaciente.Show();
            this.Hide();
        }
    }
    
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;
using Capa_Datos;// ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    public partial class FrmMenuAdministrador : Form
    {
        private readonly ServicioReportes servicioReportes = new ServicioReportes();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CargarDashboard();
        }

        private void CargarDashboard()
        {
            Administrador admin = Sesion.UsuarioActual as Administrador;
            if (admin == null) return;
            lblMenuAdministrador.Text = "Bienvenido, " + admin.Username;

            try
            {
                Dictionary<string, int> r = servicioReportes.ResumenAdministrador(admin);
                lblTituloCitasHoy.Text = r["CitasHoy"].ToString();
                label3.Text = r["Canceladas"].ToString();
                label8.Text = r["Pendientes"].ToString();
                label13.Text = r["Pacientes"].ToString();
                label11.Text = r["MedicosActivos"].ToString();
                label15.Text = r["Especialidades"].ToString();

                // Orden de ReporteEstado: Pendiente, Atendida, Cancelada
                DibujarPastel(ChartEstado,
                    servicioReportes.ReporteEstado(admin, null, null),
                    new[] { Color.FromArgb(240, 158, 38), Color.FromArgb(30, 158, 116), Color.FromArgb(217, 95, 45) });

                DibujarPastel(chartEspecialidad,
                    servicioReportes.ReporteEspecialidad(admin),
                    new[] { Color.FromArgb(59, 139, 224), Color.FromArgb(142, 95, 217), Color.FromArgb(240, 158, 38),
                    Color.FromArgb(30, 158, 116), Color.FromArgb(255, 105, 180), Color.FromArgb(217, 95, 45) });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar el dashboard");
            }
        }

        private void DibujarPastel(Chart chart, Dictionary<string, int> datos, Color[] colores)
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Legends.Clear();

            int total = datos.Values.Sum();
            if (total == 0) return;                       // sin citas todavía: gráfica vacía

            chart.ChartAreas.Add(new ChartArea());
            Series serie = new Series("datos") { ChartType = SeriesChartType.Pie };

            int i = 0;
            foreach (KeyValuePair<string, int> par in datos)
            {
                if (par.Value > 0)
                {
                    int idx = serie.Points.AddXY(par.Key, par.Value);
                    serie.Points[idx].Color = colores[i % colores.Length];
                    serie.Points[idx].LegendText = par.Key + " " + (int)Math.Round(par.Value * 100.0 / total) + "%";
                }
                i++;
            }

            chart.Series.Add(serie);
            chart.Legends.Add(new Legend { Docking = Docking.Bottom });
        }
        public FrmMenuAdministrador()
        {
            InitializeComponent();
        }

        private void FrmMenuAdministrador_Load(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            FreGestionarPacientes frmPaciente = new FreGestionarPacientes();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuAdministrador frmPaciente = new FrmMenuAdministrador();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FrmGestionarMedico frmPaciente = new FrmGestionarMedico();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmGestionarEspecialidades frmPaciente = new FrmGestionarEspecialidades();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            FrmVerReportesCitas frmPaciente = new FrmVerReportesCitas();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            FrmGestionarUsuario frmPaciente = new FrmGestionarUsuario();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {

        }
    }
}

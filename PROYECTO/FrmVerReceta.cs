using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO
{
    public partial class FrmVerReceta : Form
    {
        private readonly ServicioCitas servicioCitas = new ServicioCitas();
        private Paciente paciente;
        private string textoReceta;

        
        public int IdCita { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            paciente = Sesion.UsuarioActual as Paciente;
            if (paciente == null)
            {
                MessageBox.Show("Debes iniciar sesión como paciente.");
                Close();
                return;
            }

            btnDscargar.Click += btnDescargar_Click;   // no lo crees también en el diseñador
            MostrarReceta();
        }

        private void MostrarReceta()
        {
            try
            {
                List<Cita> citas = servicioCitas.RecetasDePaciente(paciente);
                Cita cita = null;

                if (IdCita > 0) cita = citas.Find(c => c.id_cita == IdCita);
                else if (citas.Count > 0) cita = citas[0];

                if (cita == null)
                {
                    label10.Text = "";
                    label5.Text = "";
                    label8.Text = "";
                    label11.Text = "Todavía no tienes recetas.";
                    btnDscargar.Enabled = false;
                    return;
                }

                label10.Text = servicioCitas.NombreMedico(cita.id_Medico);
                label5.Text = cita.Fecha.ToString("dd MMM yyyy");
                label8.Text = cita.Hora.ToString("h:mm tt");
                label11.Text = cita.Receta.Descripcion;

                textoReceta = servicioCitas.VerReceta(paciente, cita.id_cita);   // para descargar
                btnDscargar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar la receta");
            }
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textoReceta)) return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Texto (*.txt)|*.txt";
                dlg.FileName = "Receta_" + DateTime.Now.ToString("yyyyMMdd");
                if (dlg.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(dlg.FileName, textoReceta, Encoding.UTF8);
            }
        }
        public FrmVerReceta()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FrmHistorialCitas frmPaciente = new FrmHistorialCitas();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuPacient frmPaciente = new FrmMenuPacient();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmVerReceta frmPaciente = new FrmVerReceta();
            frmPaciente.Show();
            this.Hide();
        }

        private void FrmVerReceta_Load(object sender, EventArgs e)
        {

        }
    }
}

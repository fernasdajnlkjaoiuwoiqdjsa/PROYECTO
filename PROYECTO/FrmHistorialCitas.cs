using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO
{
    public partial class FrmHistorialCitas : Form
    {
        private readonly ServicioCitas servicioCitas = new ServicioCitas();
        private Paciente paciente;

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

            dgvHistorialCitas.AutoGenerateColumns = true;
            dgvHistorialCitas.ReadOnly = true;
            dgvHistorialCitas.AllowUserToAddRows = false;
            dgvHistorialCitas.AllowUserToDeleteRows = false;
            dgvHistorialCitas.MultiSelect = false;
            dgvHistorialCitas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorialCitas.RowHeadersVisible = false;
            dgvHistorialCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            btnVerReceta.Click += btnVerReceta_Click;   // no lo crees también en el diseñador
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                List<Cita> citas = servicioCitas.HistorialPaciente(paciente);
                Dictionary<int, string> medicos = servicioCitas.NombresDeMedicos();

                dgvHistorialCitas.DataSource = citas.Select(c => new
                {
                    IdCita = c.id_cita,
                    Fecha = c.Fecha.ToString("dd/MM/yyyy"),
                    Hora = c.Hora.ToString("h:mm tt"),
                    Medico = medicos.ContainsKey(c.id_Medico) ? medicos[c.id_Medico] : "",
                    Estado = c.Estado,
                    Receta = c.Receta != null ? "Sí" : "No"
                }).ToList();

                if (dgvHistorialCitas.Columns.Contains("IdCita"))
                    dgvHistorialCitas.Columns["IdCita"].Visible = false;   // se usa para abrir la receta
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar el historial");
            }
        }

        private void btnVerReceta_Click(object sender, EventArgs e)
        {
            if (dgvHistorialCitas.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una cita del historial.");
                return;
            }

            if (Convert.ToString(dgvHistorialCitas.CurrentRow.Cells["Receta"].Value) != "Sí")
            {
                MessageBox.Show("Esa cita no tiene receta.");
                return;
            }

            int idCita = Convert.ToInt32(dgvHistorialCitas.CurrentRow.Cells["IdCita"].Value);

            using (FrmVerReceta frm = new FrmVerReceta())
            {
                frm.IdCita = idCita;      // la propiedad que dejamos en FrmVerReceta
                frm.ShowDialog();
            }
        }

        public FrmHistorialCitas()
        {
            InitializeComponent();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuPacient frmPaciente = new FrmMenuPacient();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FrmHistorialCitas frmPaciente = new FrmHistorialCitas();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmVerReceta frmPaciente = new FrmVerReceta();
            frmPaciente.Show();
            this.Hide();
        }

        private void FrmHistorialCitas_Load(object sender, EventArgs e)
        {

        }
    }
}

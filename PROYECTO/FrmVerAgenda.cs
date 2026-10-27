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
    public partial class FrmVerAgenda : Form
    {
        private readonly ServicioCitas ServicioCitas = new ServicioCitas();
        public FrmVerAgenda()
        {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            dgvpacinetesenagendapendientes.ReadOnly = true;
            dgvpacinetesenagendapendientes.MultiSelect = false;
            dgvpacinetesenagendapendientes.AllowUserToAddRows = false;
            dgvpacinetesenagendapendientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvpacinetesenagendapendientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Evento enlazado por código: NO lo crees también desde el diseñador
            btnAtender.Click += btnAtender_Click;

            CargarAgenda();
        }

        // Las citas que los pacientes agendan con este médico, de hoy en adelante, sin las canceladas
        private void CargarAgenda()
        {
            Medico medico = Sesion.UsuarioActual as Medico;
            if (medico == null)
            {
                MessageBox.Show("Debes iniciar sesión como médico.");
                Close();
                return;
            }

            try
            {
                List<Cita> citas = ServicioCitas.AgendaMedico(medico);
                Dictionary<int, string> pacientes = ServicioCitas.NombresDePacientes();

                dgvpacinetesenagendapendientes.DataSource = citas
                    .Select(c => new
                    {
                        c.id_cita,
                        Fecha = c.Fecha.ToString("dd/MM/yyyy"),
                        Hora = c.Hora.ToString("HH:mm"),
                        Paciente = pacientes.ContainsKey(c.id_paciente) ? pacientes[c.id_paciente] : "(sin datos)",
                        c.Estado
                    })
                    .ToList();

                if (dgvpacinetesenagendapendientes.Columns.Contains("id_cita"))
                    dgvpacinetesenagendapendientes.Columns["id_cita"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar la agenda",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Abre la pantalla de atención con la cita elegida en la tabla
        private void btnAtender_Click(object sender, EventArgs e)
        {
            if (dgvpacinetesenagendapendientes.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una cita de la agenda.");
                return;
            }

            int idCita = (int)dgvpacinetesenagendapendientes.CurrentRow.Cells["id_cita"].Value;

            FrmAtenderCita frm = new FrmAtenderCita { IdCita = idCita };
            frm.Show();
            this.Hide();
        }
        private void txtBuscar_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void guna2Button9_Click_2(object sender, EventArgs e) { }
        private void guna2Button3_Click_1(object sender, EventArgs e) 
        {
            FrmPacientesAtendidos frmMedico = new FrmPacientesAtendidos();
            frmMedico.Show();
            this.Hide();
        }
        private void guna2Button4_Click_1(object sender, EventArgs e) 
        {
            FrmVerAgenda frmMedico = new FrmVerAgenda();
            frmMedico.Show();
            this.Hide();
        }
        private void guna2Button5_Click(object sender, EventArgs e) 
       
        {
            FmrMenuMedico frmMedico = new FmrMenuMedico();
            frmMedico.Show();
            this.Hide();
        }
        private void FrmVerAgenda_Load(object sender, EventArgs e) { }
    }
}

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
    public partial class FrmPacientesAtendidos : Form
    {
        private readonly CitaRepositorio citaRepo = new CitaRepositorio();
        private readonly PacienteRepositorio pacienteRepo = new PacienteRepositorio();
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

            // Configuración de la grilla
            dgvPacintesAtendidos.ReadOnly = true;
            dgvPacintesAtendidos.MultiSelect = false;
            dgvPacintesAtendidos.AllowUserToAddRows = false;
            dgvPacintesAtendidos.RowHeadersVisible = false;
            dgvPacintesAtendidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPacintesAtendidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                // Solo mis citas ya atendidas, las más recientes primero
                List<Cita> atendidas = citaRepo.ListarPorMedico(medico.id_Medico)
                    .Where(c => c.Estado == EstadosCita.Atendida)
                    .OrderByDescending(c => c.Fecha)
                    .ThenByDescending(c => c.Hora.TimeOfDay)
                    .ToList();

                // Una consulta por paciente distinto (no una por fila)
                Dictionary<int, string> nombres = atendidas
                    .Select(c => c.id_paciente)
                    .Distinct()
                    .ToDictionary(id => id, id => NombreDePaciente(id));

                dgvPacintesAtendidos.DataSource = atendidas
                    .Select(c => new
                    {
                        Fecha = c.Fecha.ToString("dd/MM/yyyy"),
                        Hora = c.Hora.ToString("HH:mm"),
                        Paciente = nombres[c.id_paciente],
                        Receta = c.Receta != null ? c.Receta.Descripcion : "Sin receta"
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar el historial",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string NombreDePaciente(int idPaciente)
        {
            Paciente p = pacienteRepo.Buscarr(idPaciente);
            return p == null ? "(paciente eliminado)" : p.Nombre + " " + p.Apellido;
        }
        public FrmPacientesAtendidos()
        {
            InitializeComponent();
        }


        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FmrMenuMedico frmPaciente = new FmrMenuMedico();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            FrmVerAgenda frmPaciente = new FrmVerAgenda();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
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

        private void dgvPacintesAtendidos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FrmPacientesAtendidos_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PROYECTO
{
    // Clase parcial: se suma a tu FrmVerAgenda sin reemplazarla.
    // Controles que debe tener el formulario: dgvAgenda (DataGridView) y btnAtender
    public partial class FrmVerAgenda
    {
        private readonly ServicioCitas servicioCitas = new ServicioCitas();

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
                List<Cita> citas = servicioCitas.AgendaMedico(medico);
                Dictionary<int, string> pacientes = servicioCitas.NombresDePacientes();

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
    }
}

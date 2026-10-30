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
    public partial class FrmGestionarEspecialidades : Form
    {
        private readonly EspcilaidadRepositorio repo = new EspcilaidadRepositorio();
        private Administrador admin;
        private Especialidad seleccionada;


        public FrmGestionarEspecialidades()
        {
            InitializeComponent();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuAdministrador frmPaciente = new FrmMenuAdministrador();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            FreGestionarPacientes frmPaciente = new FreGestionarPacientes();
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

        private void dgvGestionarEspecialidad_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvGestionarEspecialidad_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            seleccionada = (Especialidad)dgvGestionarEspecialidad.Rows[e.RowIndex].DataBoundItem;
            txtNombreEspacilidad.Text = seleccionada.NombreEspecialidad;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (seleccionada == null)
                {
                    admin.GestionarEspecialidad(Operacion.Registrar,
                        new Especialidad(0, txtNombreEspacilidad.Text.Trim()), repo);
                    MessageBox.Show("Especialidad registrada.");
                }
                else
                {
                    seleccionada.NombreEspecialidad = txtNombreEspacilidad.Text.Trim();
                    admin.GestionarEspecialidad(Operacion.Actualizar, seleccionada, repo);
                    MessageBox.Show("Especialidad actualizada.");
                }
                Recargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (seleccionada == null)
            {
                MessageBox.Show("Selecciona una especialidad de la tabla.");
                return;
            }
            if (MessageBox.Show("¿Eliminar \"" + seleccionada.NombreEspecialidad + "\"?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                admin.GestionarEspecialidad(Operacion.Eliminar, seleccionada, repo);
                Recargar();
            }
            catch (Exception ex)    // aquí sale "hay médicos con esta especialidad" si aplica
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void FrmGestionarEspecialidades_Load(object sender, EventArgs e)
        {
            admin = (Administrador)Sesion.UsuarioActual;
            Recargar();
        }
        private void Recargar()
        {
            dgvGestionarEspecialidad.DataSource = repo.ListarTodos();
            LimpiarCampos();
        }
        private void LimpiarCampos()
        {
            seleccionada = null;
            txtNombreEspacilidad.Clear();
            txtNombreEspacilidad.Focus();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Sesion.CerrarYVolverAlLogin();

        }
    }
}

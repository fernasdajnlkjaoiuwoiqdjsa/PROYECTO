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
    public partial class FreGestionarPacientes : Form
    {
        private readonly PacienteRepositorio repo = new PacienteRepositorio();
        private Administrador admin;
        private List<Paciente> pacientes = new List<Paciente> ();
        private Paciente seleccionado;

        public FreGestionarPacientes()
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

        private void FreGestionarPacientes_Load(object sender, EventArgs e)
        {
            admin = (Administrador)Sesion.UsuarioActual;
            dtmFechaNacimientoAdmin.MaxDate = DateTime.Today.AddDays(-1);
            CargarDatos();

        }
        //Metodo privado para cargar los datos en la tabla
        private void CargarDatos()
        {
            pacientes = repo.ListarTodos();
            MostarTabla();
            LimpiarCampos();
        }

        private void MostarTabla()
        {
            string filtro = txtBusqueda.Text.Trim().ToLower();

            dgvGestionarPacientes.DataSource = pacientes
                .Where(p => filtro == ""
                    || (p.Nombre + " " + p.Apellido + " " + p.Username).ToLower().Contains(filtro))
                .Select(p => new
                {
                    p.id_paciente,
                    p.Nombre,
                    p.Apellido,
                    Telefonos = string.Join(", ", p.Telefono),
                    Correos = string.Join(", ", p.Correo),
                    Usuario = p.Username
                })
                .ToList();

            if (dgvGestionarPacientes.Columns.Contains("id_paciente"))
                dgvGestionarPacientes.Columns["id_paciente"].Visible = false;
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            MostarTabla();
        }

        private void dgvGestionarPacientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) return;
            int id = (int)dgvGestionarPacientes.Rows[e.RowIndex].Cells["id_paciente"].Value;
            seleccionado = pacientes.FirstOrDefault(p => p.id_paciente == id);
            if (seleccionado == null) return;

            txtNombrePacineteAdmin.Text = seleccionado.Nombre;
            txtApellidoPacineteAdmin.Text = seleccionado.Apellido;
            DateTime f = seleccionado.FechaNacimineto;
            dtmFechaNacimientoAdmin.Value = (f > dtmFechaNacimientoAdmin.MinDate && f <= dtmFechaNacimientoAdmin.MaxDate)
                                  ? f : dtmFechaNacimientoAdmin.MaxDate;

        

            txtUsername.Text = seleccionado.Username;
            txtContraseña.Clear();                    // vacío = no cambiar la contraseña
        }
        private void LimpiarCampos()
        {
            seleccionado = null;
            txtNombrePacineteAdmin.Clear();
            txtApellidoPacineteAdmin.Clear();
            txtBusqueda.Clear();
            txtUsername.Clear();
            txtContraseña.Clear();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> telefonos = TextoLista.Telefonos(txtTelefonoPacienteAdmin.Text);
                List<string> correos = TextoLista.Correos(txtCorreoPacientAdmin.Text);
                if (seleccionado == null)
                {
                    Paciente nuevo = new Paciente(0, txtNombrePacineteAdmin.Text.Trim(), txtApellidoPacineteAdmin.Text.Trim(),
                        telefonos,correos, dtmFechaNacimientoAdmin.Value.Date,
                        0, txtUsername.Text.Trim(), txtContraseña.Text);

                    admin.GestionarPacintes(Operacion.Registrar, nuevo, repo);
                    MessageBox.Show("Paciente registrado.");
                }
                else
                {
                    seleccionado.Nombre = txtNombrePacineteAdmin.Text.Trim();
                    seleccionado.Apellido = txtApellidoPacineteAdmin.Text.Trim();
                    seleccionado.FechaNacimineto = dtmFechaNacimientoAdmin.Value.Date;
                    seleccionado.Telefono = telefonos;
                    seleccionado.Correo = correos;
                    seleccionado.Username = txtUsername.Text.Trim();
                    if (txtContraseña.Text != "") seleccionado.password = txtContraseña.Text;

                    admin.GestionarPacintes(Operacion.Actualizar, seleccionado, repo);
                    MessageBox.Show("Paciente actualizado.");
                }
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (seleccionado == null)
            {
                MessageBox.Show("Selecciona un paciente de la tabla.");
                return;
            }
            if (MessageBox.Show("¿Eliminar a " + seleccionado.Nombre + " " + seleccionado.Apellido +
                    "? También se borra su cuenta de usuario.",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                admin.GestionarPacintes(Operacion.Eliminar, seleccionado, repo);
                CargarDatos();
            }
            catch (Exception ex)    // "el paciente tiene citas registradas", por ejemplo
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

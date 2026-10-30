using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;
using Capa_Datos;
using Capoa_Entidad;// ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    public partial class FrmGestionarMedico : Form
    {
        private readonly MedicoRepositorio repo = new MedicoRepositorio();
        private Administrador admin;
        private List<Medico> medico = new List<Medico>();
        private List<Especialidad> especialidades = new List<Especialidad>();
        private Medico seleccionado;          // null = se está registrando uno nuevo

        public FrmGestionarMedico()
        {
            InitializeComponent();
            this.Load += FrmGestionarMedico_Load;   // NO crees otro Load desde el diseñador
        }

        // ---------- Carga inicial ----------
        private void FrmGestionarMedico_Load(object sender, EventArgs e)
        {
            admin = Sesion.UsuarioActual as Administrador;
            if (admin == null)
            {
                MessageBox.Show("Debes iniciar sesión como administrador.");
                Close();
                return;
            }

            // Combo de especialidad: se llena desde la BD
            especialidades = new EspcilaidadRepositorio().ListarTodos();
            cmbEspecialidad.DisplayMember = "NombreEspecialidad";
            cmbEspecialidad.ValueMember = "id_especialidad";
            cmbEspecialidad.DataSource = especialidades;
            cmbEspecialidad.SelectedIndex = -1;

            CargarDatos();
        }

        // ---------- Navegación (sin cambios) ----------
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

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuAdministrador frmPaciente = new FrmMenuAdministrador();
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

        // ---------- Acciones ----------
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            dgvGestionarMedico.ClearSelection();
            LimpiarCampos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> telefonos = TextoLista.Telefonos(txtTelefono.Text);
                List<string> correos = TextoLista.Correos(txtCorreo.Text);
                int idEsp = cmbEspecialidad.SelectedIndex < 0 ? 0 : (int)cmbEspecialidad.SelectedValue;

                if (seleccionado == null)
                {
                    // Constructor: id, nombre, apellido, teléfonos, correos, especialidad, idUsuario, username, password
                    Medico nuevo = new Medico(0, txtNOmbre.Text.Trim(), txtApellido.Text.Trim(),
                        telefonos, correos, idEsp,
                        0, txtUsuario.Text.Trim(), txtContraseña.Text);

                    admin.GestionarMedicos(Operacion.Registrar, nuevo, repo);
                    MessageBox.Show("Médico registrado.");
                }
                else
                {
                    seleccionado.Nombre = txtNOmbre.Text.Trim();
                    seleccionado.Apellido = txtApellido.Text.Trim();
                    seleccionado.Telefono = telefonos;
                    seleccionado.Correo = correos;
                    seleccionado.id_especialidad = idEsp;
                    seleccionado.Username = txtUsuario.Text.Trim();
                    if (txtContraseña.Text != "") seleccionado.password = txtContraseña.Text;

                    admin.GestionarMedicos(Operacion.Actualizar, seleccionado, repo);
                    MessageBox.Show("Médico actualizado.");
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
                MessageBox.Show("Selecciona un médico de la tabla.");
                return;
            }
            if (MessageBox.Show("¿Eliminar a " + seleccionado.Nombre + " " + seleccionado.Apellido +
                    "? También se borra su cuenta de usuario.",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                admin.GestionarMedicos(Operacion.Eliminar, seleccionado, repo);
                CargarDatos();
            }
            catch (Exception ex)    // "el médico tiene citas registradas", por ejemplo
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ---------- Tabla ----------
        private void CargarDatos()
        {
            medico = repo.ListarTodos();
            MostarTabla();
            LimpiarCampos();
        }

        private void MostarTabla()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            if (filtro == "buscar medico") filtro = "";
            Dictionary<int, string> nombres =
                especialidades.ToDictionary(x => x.id_especialidad, x => x.NombreEspecialidad);

            dgvGestionarMedico.DataSource = medico
                .Where(p => filtro == ""
                    || (p.Nombre + " " + p.Apellido + " " + p.Username).ToLower().Contains(filtro))
                .Select(p => new
                {
                    p.id_Medico,
                    p.Nombre,
                    p.Apellido,
                    Especialidad = nombres.ContainsKey(p.id_especialidad) ? nombres[p.id_especialidad] : "(sin asignar)",
                    Telefonos = string.Join(", ", p.Telefono),
                    Correos = string.Join(", ", p.Correo),
                    Usuario = p.Username
                })
                .ToList();

            if (dgvGestionarMedico.Columns.Contains("id_Medico"))
                dgvGestionarMedico.Columns["id_Medico"].Visible = false;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            MostarTabla();
        }

        private void dgvGestionarMedico_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = (int)dgvGestionarMedico.Rows[e.RowIndex].Cells["id_Medico"].Value;
            seleccionado = medico.FirstOrDefault(p => p.id_Medico == id);
            if (seleccionado == null) return;

            txtNOmbre.Text = seleccionado.Nombre;
            txtApellido.Text = seleccionado.Apellido;
            txtTelefono.Text = string.Join(", ", seleccionado.Telefono);
            txtCorreo.Text = string.Join(", ", seleccionado.Correo);

            if (seleccionado.id_especialidad > 0)
                cmbEspecialidad.SelectedValue = seleccionado.id_especialidad;
            else
                cmbEspecialidad.SelectedIndex = -1;

            txtUsuario.Text = seleccionado.Username;
            txtContraseña.Clear();          // vacío = no cambiar la contraseña
        }

        private void LimpiarCampos()
        {
            seleccionado = null;
            txtNOmbre.Clear();
            txtApellido.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            cmbEspecialidad.SelectedIndex = -1;
            txtUsuario.Clear();
            txtContraseña.Clear();
            txtNOmbre.Focus();
        }

        private void FrmGestionarMedico_Load_1(object sender, EventArgs e)
        {
            admin = (Administrador)Sesion.UsuarioActual;
            CargarDatos();

        }
    }
}
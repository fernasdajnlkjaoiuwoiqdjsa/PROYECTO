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
using Capa_Datos;// ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    public partial class FrmGestionarUsuario : Form
    {
        private readonly UsuarioRepositorio repo = new UsuarioRepositorio();
        private Administrador admin;
        private List<Usuario> usuarios = new List<Usuario>();
        private Usuario seleccionado;
        public FrmGestionarUsuario()
        {
            InitializeComponent();
        }

        private void FrmGestionarUsuario_Load(object sender, EventArgs e)
        {

        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            admin = Sesion.UsuarioActual as Administrador;
            if (admin == null)
            {
                MessageBox.Show("Debes iniciar sesión como administrador.");
                Close();
                return;
            }

            // Configuración de la grilla
            dgvGestionarUsuarios.ReadOnly = true;
            dgvGestionarUsuarios.MultiSelect = false;
            dgvGestionarUsuarios.AllowUserToAddRows = false;
            dgvGestionarUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGestionarUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Eventos enlazados por código: NO los crees también desde el diseñador
            btnBuscar.Click += (s, ev) => MostrarTabla();
            txtBuscar.TextChanged += (s, ev) => MostrarTabla();
            dgvGestionarUsuarios.CellClick += dgvGestionarUsuarios_CellClick;
            btnEditar.Click += btnEditar_Click;
            btnResetarContraseña.Click += btnResetarContraseña_Click;
            btnActivar.Click += (s, ev) => CambiarEstado(true);
            btnDesactivar.Click += (s, ev) => CambiarEstado(false);

            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                usuarios = repo.ListarTodos().Where(u => u != null).ToList();
                MostrarTabla();
                seleccionado = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar los usuarios",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void MostrarTabla()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            if (filtro.StartsWith("buscar")) filtro = "";

            dgvGestionarUsuarios.DataSource = usuarios
                .Where(u => filtro == "" || u.Username.ToLower().Contains(filtro))
                .Select(u => new
                {
                    u.id_usuario,
                    Usuario = u.Username,
                    Nombre = NombreDe(u),
                    u.Rol,
                    Estado = u.SesionActiva ? "Activo" : "Inactivo"
                })
                .ToList();

            if (dgvGestionarUsuarios.Columns.Contains("id_usuario"))
                dgvGestionarUsuarios.Columns["id_usuario"].Visible = false;
        }

        //Metodo estatico privado para mostar el nombre pacint y medico 
        private static string NombreDe(Usuario u)
        {
            Paciente p = u as Paciente;
            if (p != null) return p.Nombre + " " + p.Apellido;

            Medico m = u as Medico;
            if (m != null) return m.Nombre + " " + m.Apellido;

            return "(Administrador)";
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

        private void dgvGestionarUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = (int)dgvGestionarUsuarios.Rows[e.RowIndex].Cells["id_usuario"].Value;
            seleccionado = usuarios.FirstOrDefault(u => u.id_usuario == id);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!HaySeleccion()) return;

            string nuevo = Pedir("Editar usuario", "Nuevo nombre de usuario:", seleccionado.Username, false);
            if (nuevo == null) return;                       // canceló

            string anterior = seleccionado.Username;
            try
            {
                seleccionado.Username = nuevo.Trim();
                admin.GestionarUsuarios(seleccionado, repo, null);   // null = no cambia la contraseña
                MessageBox.Show("Datos actualizados.");
                CargarDatos();
            }
            catch (Exception ex)
            {
                seleccionado.Username = anterior;            // deshace el cambio en memoria
                MessageBox.Show(ex.Message, "No se pudo guardar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnResetarContraseña_Click(object sender, EventArgs e)
        {
            if (!HaySeleccion()) return;

            string nueva = Pedir("Resetear contraseña",
                                 "Nueva contraseña para " + seleccionado.Username + ":", "", true);
            if (nueva == null) return;                       // canceló
            if (nueva == "")
            {
                MessageBox.Show("La contraseña no puede estar vacía.");
                return;
            }

            try
            {
                admin.GestionarUsuarios(seleccionado, repo, nueva);
                MessageBox.Show("Contraseña restablecida.");
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo restablecer",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //Metodo privado para desactivar el acceso sin borrar los datos cambiando el estado activo o inactivo del usuario
        private void CambiarEstado(bool nuevoEstado)
        {
            if (!HaySeleccion()) return;

            if (seleccionado.SesionActiva == nuevoEstado)
            {
                MessageBox.Show("La cuenta ya está " + (nuevoEstado ? "activa." : "inactiva."));
                return;
            }

            if (!nuevoEstado && seleccionado.id_usuario == admin.id_usuario)
            {
                MessageBox.Show("No puedes desactivar tu propia cuenta.");
                return;
            }

            string accion = nuevoEstado ? "activar" : "desactivar";
            if (MessageBox.Show("¿Deseas " + accion + " la cuenta de " + seleccionado.Username + "?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                admin.CambiarEstadoCuenta(seleccionado, nuevoEstado, repo);
                CargarDatos();
            }
            catch (Exception ex)
            {
                seleccionado.Activo = !nuevoEstado;          // deshace el cambio en memoria
                MessageBox.Show(ex.Message, "No se pudo cambiar el estado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //Metodo privado de auxiliar
        private bool HaySeleccion()
        {
            if (seleccionado == null)
            {
                MessageBox.Show("Selecciona un usuario de la tabla.");
                return false;
            }
            return true;
        }

        //Ventana para pedir un dato devuelve null si el usuario cancela
        private static string Pedir(string titulo, string mensaje, string valorInicial, bool esPassword)
        {
            using (Form f = new Form())
            {
                f.Text = titulo;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.StartPosition = FormStartPosition.CenterParent;
                f.MinimizeBox = false;
                f.MaximizeBox = false;
                f.ClientSize = new Size(360, 130);

                Label lbl = new Label { Text = mensaje, Left = 15, Top = 15, Width = 330 };
                TextBox txt = new TextBox
                {
                    Left = 15,
                    Top = 45,
                    Width = 330,
                    Text = valorInicial,
                    UseSystemPasswordChar = esPassword
                };
                Button ok = new Button { Text = "Aceptar", Left = 175, Top = 85, Width = 80, DialogResult = DialogResult.OK };
                Button cancelar = new Button { Text = "Cancelar", Left = 265, Top = 85, Width = 80, DialogResult = DialogResult.Cancel };

                f.Controls.AddRange(new Control[] { lbl, txt, ok, cancelar });
                f.AcceptButton = ok;
                f.CancelButton = cancelar;

                return f.ShowDialog() == DialogResult.OK ? txt.Text : null;
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Sesion.CerrarYVolverAlLogin();

        }
    }
}

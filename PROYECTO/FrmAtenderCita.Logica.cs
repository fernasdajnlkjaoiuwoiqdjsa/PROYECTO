using System;
using System.Windows.Forms;

namespace PROYECTO
{
    // Clase parcial: se suma a tu FrmAtenderCita sin reemplazarla.
    // Controles que debe tener el formulario (Propiedades -> (Name)):
    //   lblPaciente, lblFechaHora, btnMarcarAtendida, txtReceta, btnGuardar, btnLimpiar
    // En txtReceta, el texto "Consulta y Medicamento..." va en PlaceholderText, no en Text.
    public partial class FrmAtenderCita
    {
        private readonly ServicioCitas servicioCitas = new ServicioCitas();
        private Medico medico;
        private Cita cita;

        // La agenda asigna este valor antes de mostrar el formulario
        public int IdCita { get; set; }

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

            // Eventos enlazados por código: NO los crees también desde el diseñador
            btnMarcarAtendida.Click += btnMarcarAtendida_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnLiminar.Click += (s, ev) =>
            {
                if (txtEmitirReceta.Enabled && !txtEmitirReceta.ReadOnly) txtEmitirReceta.Clear();
            };

            txtEmitirReceta.MaxLength = 500;      // igual que varchar(500) en la tabla Receta

            if (IdCita <= 0)
            {
                // Se entró desde el menú lateral, sin elegir una cita en la agenda
                BloquearTodo();
                MessageBox.Show("Elige una cita en la Agenda");
                return;
            }

            CargarCita();
        }

        // Trae la cita y los datos reales del paciente
        private void CargarCita()
        {
            try
            {
                cita = servicioCitas.ObtenerCitaDeMedico(medico, IdCita);
                Paciente paciente = servicioCitas.ObtenerPaciente(cita.id_paciente);

                lbPaciente.Text = paciente != null
                    ? paciente.Nombre + " " + paciente.Apellido
                    : "(sin datos)";
                lbFechaHora.Text = cita.Fecha.ToString("dd/MM/yyyy") + ", " + cita.Hora.ToString("HH:mm");

                ActualizarControles();
            }
            catch (Exception ex)
            {
                BloquearTodo();
                MessageBox.Show(ex.Message, "No se pudo cargar la cita",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // El orden del flujo lo marca el estado: primero se atiende, después (opcional) se emite la receta
        private void ActualizarControles()
        {
            bool pendiente = cita.Estado == EstadosCita.Pendiente;
            bool atendida = cita.Estado == EstadosCita.Atendida;
            bool tieneReceta = cita.Receta != null;

            btnMarcarAtendida.Enabled = pendiente;
            btnMarcarAtendida.Text = pendiente ? "Marcar Atendida"
                                   : atendida ? "Cita atendida"
                                   : "Cita " + cita.Estado.ToLower();

            txtEmitirReceta.Enabled = atendida;           // solo se receta una cita ya atendida
            txtEmitirReceta.ReadOnly = tieneReceta;       // si ya tiene receta, solo se muestra
            btnGuardar.Enabled = atendida && !tieneReceta;
            btnLiminar.Enabled = atendida && !tieneReceta;

            if (tieneReceta) txtEmitirReceta.Text = cita.Receta.Descripcion;
        }

        private void BloquearTodo()
        {
            lbPaciente.Text = "-";
            lbFechaHora.Text = "-";
            btnMarcarAtendida.Enabled = false;
            txtEmitirReceta.Enabled = false;
            btnGuardar.Enabled = false;
            btnLiminar.Enabled = false;
        }

        private void btnMarcarAtendida_Click(object sender, EventArgs e)
        {
            if (cita == null) return;

            if (MessageBox.Show("¿Marcar la cita como atendida?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                servicioCitas.MarcarAtendida(medico, IdCita);   // la entidad valida que esté Pendiente
                MessageBox.Show("La cita quedó atendida. Ya puedes emitir la receta.");
                CargarCita();                                   // vuelve a leer el estado desde la BD
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo marcar como atendida",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cita == null) return;

            try
            {
                servicioCitas.EmitirReceta(medico, IdCita, txtEmitirReceta.Text);
                MessageBox.Show("Receta guardada.");
                CargarCita();
            }
            catch (Exception ex)    // receta vacía, cita sin atender, receta repetida...
            {
                MessageBox.Show(ex.Message, "No se pudo guardar la receta",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

      

        private void FrmAtenderCita_Load(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmAtenderCita
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "FrmAtenderCita";
            this.Load += new System.EventHandler(this.FrmAtenderCita_Load_1);
            this.ResumeLayout(false);

        }

        private void FrmAtenderCita_Load_1(object sender, EventArgs e)
        {

        }
    }
}

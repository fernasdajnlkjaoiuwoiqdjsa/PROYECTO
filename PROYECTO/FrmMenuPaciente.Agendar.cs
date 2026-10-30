using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;
using Capa_Datos;// ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    // Clase parcial se suma al FrmMenuPaciente sin reemplazarla.
    public partial class FrmMenuPacient
    {
        private readonly ServicioCitas servicioCitas = new ServicioCitas();
        private bool cargandoAgenda;     // evita que un combo dispare al siguiente mientras se llena por código

        // Se ejecuta al abrir el formulario no hace falta crear el evento 
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Eventos enlazados por códig no es necesario crearlos desde el diseñador
            cmbElegirEspecialidad.SelectedIndexChanged += (s, ev) => CargarMedicos();
            cmbElegirMedico.SelectedIndexChanged += (s, ev) => CargarHorarios();
            dtElgirFecha.ValueChanged += (s, ev) => CargarHorarios();
            btnAgendarCita.Click += btnAgendar_Click;

            dtElgirFecha.MinDate = DateTime.Today;      //Fecha real no atrasada
            dtElgirFecha.Value = DateTime.Today;
            CargarEspecialidades();
            CargarDashboardPaciente();
        }
        private void CargarDashboardPaciente()
        {
            Paciente pac = Sesion.UsuarioActual as Paciente;
            if (pac == null) return;
            lblMenuPaciente.Text = "Bienvenido, " + pac.Username;


            try
            {
                ServicioCitas servicio = new ServicioCitas();

                // Tarjetas
                lblTarjetaPendientes.Text = servicio.CantidadCitas(pac, EstadosCita.Pendiente).ToString();
                lblTarjetaAtendidas.Text = servicio.CantidadCitas(pac, EstadosCita.Atendida).ToString();

                Cita proxima = servicio.ProximaCita(pac);
                lblTarjetaProxima.Text = proxima != null ? proxima.Fecha.ToString("dd MMM") : "Ninguna";

                // Bloque "Próxima cita": si no hay pendiente, muestra la última que tuvo
                Cita mostrar = proxima ?? servicio.UltimaCita(pac);

                if (mostrar == null)
                {
                    lblProximaCitaTitulo.Text = "Próxima Cita:";
                    lblProximaCitaMedico.Text = "Todavía no tienes citas. Agenda una abajo.";
                    btnEstadoCita.Visible = false;
                    return;
                }

                lblProximaCitaTitulo.Text = proxima != null ? "Próxima Cita:" : "Última Cita:";
                lblProximaCitaMedico.Text = servicio.DescripcionMedico(mostrar.id_Medico)
                    + " · " + mostrar.Fecha.ToString("dd/MM/yyyy") + " " + mostrar.Hora.ToString("h:mm tt");

                MostrarEstado(mostrar.Estado);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar el dashboard");
            }
        }

        private void MostrarEstado(string estado)
        {
            Color fondo, texto;

            if (estado == EstadosCita.Atendida)
            {
                fondo = Color.FromArgb(200, 240, 220); texto = Color.FromArgb(20, 110, 70);
            }
            else if (estado == EstadosCita.Cancelar)
            {
                fondo = Color.FromArgb(250, 210, 205); texto = Color.FromArgb(160, 40, 30);
            }
            else   // Pendiente
            {
                fondo = Color.FromArgb(255, 228, 181); texto = Color.FromArgb(120, 80, 0);
            }

            btnEstadoCita.Visible = true;
            btnEstadoCita.Text = estado;
            btnEstadoCita.FillColor = fondo;
            btnEstadoCita.ForeColor = texto;
            btnEstadoCita.HoverState.FillColor = fondo;   // que no cambie de color al pasar el mouse
        }
        // 1) Especialidades desde la base de datos
        private void CargarEspecialidades()
        {
            try
            {
                cargandoAgenda = true;
                cmbElegirEspecialidad.DisplayMember = "NombreEspecialidad";
                cmbElegirEspecialidad.ValueMember = "id_especialidad";
                cmbElegirEspecialidad.DataSource = servicioCitas.ObtenerEspecialidades();
                cmbElegirEspecialidad.SelectedIndex = -1;
                cmbElegirMedico.DataSource = null;
                cmbHorariosDisponibls.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudieron cargar las especialidades",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { cargandoAgenda = false; }
        }

        // 2) Médicos de la especialidad elegida
        private void CargarMedicos()
        {
            if (cargandoAgenda || cmbElegirEspecialidad.SelectedIndex < 0) return;

            try
            {
                cargandoAgenda = true;
                int idEspecialidad = (int)cmbElegirEspecialidad.SelectedValue;
                List<Medico> medicos = servicioCitas.ObtenerMedicosPorEspecialidad(idEspecialidad);

                cmbElegirMedico.DisplayMember = "Texto";
                cmbElegirMedico.ValueMember = "Id";
                cmbElegirMedico.DataSource = medicos
                    .Select(m => new { Texto = m.Nombre + " " + m.Apellido, Id = m.id_Medico })
                    .ToList();
                cmbElegirMedico.SelectedIndex = -1;
                cmbHorariosDisponibls.DataSource = null;

                if (medicos.Count == 0)
                    MessageBox.Show("Todavía no hay médicos registrados en esa especialidad.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudieron cargar los médicos",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { cargandoAgenda = false; }
        }

        // 3) Horarios libres del médico en la fecha elegida
        private void CargarHorarios()
        {
            if (cargandoAgenda || cmbElegirMedico.SelectedIndex < 0) return;

            try
            {
                cargandoAgenda = true;
                int idMedico = (int)cmbElegirMedico.SelectedValue;
                List<TimeSpan> libres = servicioCitas.ObtenerHorariosDisponibles(idMedico, dtElgirFecha.Value.Date);

                cmbHorariosDisponibls.DataSource = libres.Select(h => h.ToString(@"hh\:mm")).ToList();
                cmbHorariosDisponibls.SelectedIndex = -1;

                if (libres.Count == 0)
                    MessageBox.Show("No hay horarios disponibles ese día. Prueba con otra fecha.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudieron cargar los horarios",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { cargandoAgenda = false; }
        }

        // 4) Confirma el servicio valida y guarda, la cita queda Pendiente
        private void btnAgendar_Click(object sender, EventArgs e)
        {
            Paciente paciente = Sesion.UsuarioActual as Paciente;
            if (paciente == null)
            {
                MessageBox.Show("Debes iniciar sesión como paciente.");
                return;
            }

            if (cmbElegirEspecialidad.SelectedIndex < 0 || cmbElegirMedico.SelectedIndex < 0 || cmbHorariosDisponibls.SelectedIndex < 0)
            {
                MessageBox.Show("Elige la especialidad, el médico, la fecha y el horario.");
                return;
            }

            try
            {
                int idMedico = (int)cmbElegirMedico.SelectedValue;
                DateTime fecha = dtElgirFecha.Value.Date;
                TimeSpan hora = TimeSpan.Parse((string)cmbHorariosDisponibls.SelectedItem);

                servicioCitas.AgendarCita(paciente, idMedico, fecha, hora);

                MessageBox.Show("Cita agendada para el " + fecha.ToString("dd/MM/yyyy") +
                                " a las " + hora.ToString(@"hh\:mm") + ".", "Cita registrada");
                CargarHorarios();       // el horario recién tomado ya no aparece
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo agendar",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                CargarHorarios();       // por si otro paciente ocupó ese horario
            }
        }

        /*private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmMenuPacient
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "FrmMenuPacient";
            this.Load += new System.EventHandler(this.FrmMenuPacient_Load);
            this.ResumeLayout(false);

        }

       /* private void FrmMenuPacient_Load(object sender, EventArgs e)
        {

        }*/
    }
}

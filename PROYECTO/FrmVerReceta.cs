using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;
using Capa_Datos;// ServicioCitas, ServicioAutenticacion, ServicioReportes

namespace PROYECTO
{
    public partial class FrmVerReceta : Form
    {
        private readonly ServicioCitas servicioCitas = new ServicioCitas();
        private Paciente paciente;
        private string textoReceta;

        
        public int IdCita { get; set; }

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

            btnDscargar.Click += btnDescargar_Click;   // no lo crees también en el diseñador
            MostrarReceta();
        }

        private void MostrarReceta()
        {
            try
            {
                List<Cita> citas = servicioCitas.RecetasDePaciente(paciente);
                Cita cita = null;

                if (IdCita > 0) cita = citas.Find(c => c.id_cita == IdCita);
                else if (citas.Count > 0) cita = citas[0];

                if (cita == null)
                {
                    label10.Text = "";
                    label5.Text = "";
                    label8.Text = "";
                    label11.Text = "Todavía no tienes recetas.";
                    btnDscargar.Enabled = false;
                    return;
                }

                label10.Text = servicioCitas.NombreMedico(cita.id_Medico);
                label5.Text = cita.Fecha.ToString("dd MMM yyyy");
                label8.Text = cita.Hora.ToString("h:mm tt");
                label11.Text = cita.Receta.Descripcion;

                textoReceta = servicioCitas.VerReceta(paciente, cita.id_cita);   // para descargar
                btnDscargar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo cargar la receta");
            }
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textoReceta)) return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Texto (*.txt)|*.txt";
                dlg.FileName = "Receta_" + DateTime.Now.ToString("yyyyMMdd");
                if (dlg.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(dlg.FileName, textoReceta, Encoding.UTF8);
            }
        }
        public FrmVerReceta()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FrmHistorialCitas frmPaciente = new FrmHistorialCitas();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuPacient frmPaciente = new FrmMenuPacient();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmVerReceta frmPaciente = new FrmVerReceta();
            frmPaciente.Show();
            this.Hide();
        }

        private void FrmVerReceta_Load(object sender, EventArgs e)
        {

        }

        private void btnDscargar_Click(object sender, EventArgs e)
        {
            // El botón solo está activo cuando existe una receta emitida
            if (!btnDscargar.Enabled || string.IsNullOrEmpty(label11.Text)) return;

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "PDF (*.pdf)|*.pdf";
                dlg.FileName = "Receta_" + DateTime.Now.ToString("yyyyMMdd");
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    PrintDocument doc = new PrintDocument();
                    doc.DocumentName = "Receta médica";
                    doc.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                    if (!doc.PrinterSettings.IsValid)
                    {
                        MessageBox.Show("Tu Windows no tiene activada la impresora \"Microsoft Print to PDF\".");
                        return;
                    }

                    doc.PrinterSettings.PrintToFile = true;
                    doc.PrinterSettings.PrintFileName = dlg.FileName;
                    doc.PrintPage += DibujarRecetaPdf;
                    doc.Print();

                    MessageBox.Show("Receta guardada en:\n" + dlg.FileName, "PDF generado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "No se pudo generar el PDF");
                }
            }
        }
        // Dibuja la hoja de la receta la unidad de la página es 1/100 de pulgada
       // Dibuja la hoja de la receta (la unidad de la página es 1/100 de pulgada)
            private void DibujarRecetaPdf(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float x = e.MarginBounds.Left;
            float ancho = e.MarginBounds.Width;
            float y = e.MarginBounds.Top;

            using (Font fTitulo = new Font("Segoe UI", 24, FontStyle.Bold))
            using (Font fMarca = new Font("Segoe UI", 20, FontStyle.Bold))
            using (Font fSub = new Font("Segoe UI", 11))
            using (Font fEtiqueta = new Font("Segoe UI", 10, FontStyle.Bold))
            using (Font fTexto = new Font("Segoe UI", 14))
            using (Font fPie = new Font("Segoe UI", 9))
            using (Brush azul = new SolidBrush(Color.MidnightBlue))
            using (Brush gris = new SolidBrush(Color.FromArgb(110, 110, 110)))
            using (Pen linea = new Pen(Color.LightGray, 1))
            {
                // Encabezado con logo
                // Encabezado con logo
                g.FillRectangle(azul, x, y, ancho, 90);

                float xTexto = x + 20;
                Image logo = Properties.Resources.catedrapoo;
                if (logo != null)
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    float caja = 78;                          // círculo blanco (el encabezado mide 90)
                    float xLogo = x + 15;
                    float yLogo = y + 6;
                    g.FillEllipse(Brushes.White, xLogo, yLogo, caja, caja);
                    // El logo se escala para caber entero dentro del círculo, centrado y sin deformarse
                    float interior = caja - 12;
                    float escala = Math.Min(interior / logo.Width, interior / logo.Height);
                    float wLogo = logo.Width * escala;
                    float hLogo = logo.Height * escala;
                    g.DrawImage(logo, xLogo + (caja - wLogo) / 2, yLogo + (caja - hLogo) / 2, wLogo, hLogo);

                    xTexto = xLogo + caja + 15;               // el nombre queda a la par del logo
                }

                g.DrawString("SaludVital", fMarca, Brushes.White, xTexto, y + 12);
                g.DrawString("Sistema de Gestión", fSub, Brushes.White, xTexto + 2, y + 52);
                y += 125;
                // Datos
                string nombrePaciente = paciente != null ? paciente.Nombre + " " + paciente.Apellido : "";
                string[,] datos =
                {
            { "Paciente:", nombrePaciente },
            { "Médico:",   label10.Text },
            { "Fecha:",    label5.Text },
            { "Hora:",     label8.Text }
        };

                for (int i = 0; i < datos.GetLength(0); i++)
                {
                    g.DrawString(datos[i, 0], fEtiqueta, gris, x, y);
                    g.DrawString(datos[i, 1], fTexto, Brushes.Black, x, y + 18);
                    y += 62;
                }

                // Receta (con salto de línea automático)
                g.DrawLine(linea, x, y, x + ancho, y);
                y += 15;
                g.DrawString("Receta:", fEtiqueta, gris, x, y);
                y += 22;
                g.DrawString(label11.Text, fTexto, Brushes.Black,
                             new RectangleF(x, y, ancho, 350));

                // Pie de página
                g.DrawString("Documento generado por SaludVital el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                             fPie, gris, x, e.MarginBounds.Bottom - 15);
            }

            e.HasMorePages = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Sesion.CerrarYVolverAlLogin();

        }
    }
}

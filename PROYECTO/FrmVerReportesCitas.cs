using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Capa_Entidad;   // Medico, Cita, Paciente, Usuario, Administrador, Especialidad
using Capa_Negocio;
using Capa_Datos;// ServicioCitas, ServicioAutenticacion, ServicioReportes



namespace PROYECTO
{
    public partial class FrmVerReportesCitas : Form
    {
        private readonly ServicioReportes servicioReportes = new ServicioReportes();
        private Administrador admin;

        private List<string[]> filasPdf = new List<string[]>();   // las filas que se ven en el grid
        private string filtrosPdf = "";
        private int filaPdf, paginaPdf;

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

            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.Items.Clear();
            cmbEstado.Items.AddRange(new object[] { "Todos", EstadosCita.Pendiente, EstadosCita.Atendida, EstadosCita.Cancelar });
            cmbEstado.SelectedIndex = 0;

            dtpFecha.ShowCheckBox = true;
            dtpFecha.Checked = false;                 // sin filtro de fecha al abrir

            dgvReportesCitasybuscralacita.AutoGenerateColumns = true;
            dgvReportesCitasybuscralacita.ReadOnly = true;
            dgvReportesCitasybuscralacita.AllowUserToAddRows = false;
            dgvReportesCitasybuscralacita.AllowUserToDeleteRows = false;
            dgvReportesCitasybuscralacita.MultiSelect = false;
            dgvReportesCitasybuscralacita.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReportesCitasybuscralacita.RowHeadersVisible = false;
            dgvReportesCitasybuscralacita.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            btnBuscar.Click += btnBuscar_Click;               // no los crees también en el diseñador
            btnExportarPDF.Click += btnExportarPdf_Click;
            cmbAgrupar.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAgrupar.Items.Clear();
            cmbAgrupar.Items.AddRange(new object[] { "Día", "Mes" });
            cmbAgrupar.SelectedIndex = 0;
            cmbAgrupar.SelectedIndexChanged += (s, ev) => DibujarGraficoCitas();

            BuscarCitas();                            // al abrir muestra todas las citas
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarCitas();
        }

        private void BuscarCitas()
        {
            try
            {
                DateTime? fecha = dtpFecha.Checked ? (DateTime?)dtpFecha.Value.Date : null;
                string estado = cmbEstado.SelectedIndex <= 0 ? null : cmbEstado.SelectedItem.ToString();
                string nombre = txtNombre.Text.Trim();

                List<Cita> citas = servicioReportes.BuscarCitas(admin, fecha, estado, nombre);
                Dictionary<int, string> pac = servicioReportes.NombresDePacientes();
                Dictionary<int, string> med = servicioReportes.NombresDeMedicos();

                filasPdf = citas.Select(c => new string[]
                {
            c.Fecha.ToString("dd/MM/yyyy"),
            c.Hora.ToString("h:mm tt"),
            pac.ContainsKey(c.id_paciente) ? pac[c.id_paciente] : "",
            med.ContainsKey(c.id_Medico) ? med[c.id_Medico] : "",
            c.Estado
                }).ToList();

                dgvReportesCitasybuscralacita.DataSource = filasPdf.Select(f => new
                {
                    Fecha = f[0],
                    Hora = f[1],
                    Paciente = f[2],
                    Medico = f[3],
                    Estado = f[4]
                }).ToList();

                // Texto de filtros para el PDF
                List<string> partes = new List<string>();
                if (fecha.HasValue) partes.Add("Fecha: " + fecha.Value.ToString("dd/MM/yyyy"));
                if (estado != null) partes.Add("Estado: " + estado);
                if (nombre != "") partes.Add("Nombre: " + nombre);
                filtrosPdf = partes.Count > 0 ? "Filtros: " + string.Join("  |  ", partes) : "Filtros: ninguno";
                DibujarGraficoCitas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo generar el reporte");
            }
        }
        private void DibujarGraficoCitas()
        {
            if (admin == null || cmbAgrupar.SelectedItem == null) return;

            try
            {
                string agrupar = cmbAgrupar.SelectedItem.ToString();
                DateTime? referencia = dtpFecha.Checked ? (DateTime?)dtpFecha.Value : null;
                string estado = cmbEstado.SelectedIndex <= 0 ? null : cmbEstado.SelectedItem.ToString();

                string titulo;
                Dictionary<string, int> datos = servicioReportes.CitasPorPeriodo(
                    admin, agrupar, referencia, estado, txtNombre.Text.Trim(), out titulo);

                chartCitas.Series.Clear();
                chartCitas.ChartAreas.Clear();
                chartCitas.Legends.Clear();
                chartCitas.Titles.Clear();

                if (datos.Count == 0 || datos.Values.Sum() == 0)
                {
                    chartCitas.Titles.Add("No hay citas para graficar");
                    return;
                }

                ChartArea area = new ChartArea();
                area.AxisX.Interval = 1;
                area.AxisX.MajorGrid.Enabled = false;
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
                area.AxisY.MajorGrid.LineColor = Color.Gainsboro;
                area.AxisY.LabelStyle.Format = "0";                        // solo números enteros
                area.AxisY.Interval = datos.Values.Max() <= 10 ? 1 : 0;    // 0 = automático
                chartCitas.ChartAreas.Add(area);

                Series serie = new Series("Citas");
                serie.ChartType = SeriesChartType.Column;
                serie.Color = Color.FromArgb(59, 139, 224);
                serie["PointWidth"] = "0.7";

                foreach (KeyValuePair<string, int> par in datos)
                {
                    int idx = serie.Points.AddXY(par.Key, par.Value);
                    serie.Points[idx].IsValueShownAsLabel = par.Value > 0;   // la cuenta sobre cada barra
                }
                chartCitas.Series.Add(serie);

                chartCitas.Titles.Add(new Title(titulo, Docking.Top,
                    new Font("Segoe UI", 10, FontStyle.Bold), Color.Black));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo dibujar el gráfico");
            }
        }

        private void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (filasPdf.Count == 0)
            {
                MessageBox.Show("No hay citas para exportar. Cambia los filtros y busca de nuevo.");
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "PDF (*.pdf)|*.pdf";
                dlg.FileName = "ReporteCitas_" + DateTime.Now.ToString("yyyyMMdd");
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    PrintDocument doc = new PrintDocument();
                    doc.DocumentName = "Reporte de citas";
                    doc.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                    if (!doc.PrinterSettings.IsValid)
                    {
                        MessageBox.Show("Tu Windows no tiene activada la impresora \"Microsoft Print to PDF\".");
                        return;
                    }

                    doc.PrinterSettings.PrintToFile = true;
                    doc.PrinterSettings.PrintFileName = dlg.FileName;

                    filaPdf = 0;
                    paginaPdf = 0;
                    doc.PrintPage += DibujarReportePdf;
                    doc.Print();

                    MessageBox.Show("Reporte guardado en:\n" + dlg.FileName, "PDF generado");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "No se pudo generar el PDF");
                }
            }
        }

        // Dibuja una página del reporte; si faltan filas pide otra página
        private void DibujarReportePdf(object sender, PrintPageEventArgs e)
        {
            paginaPdf++;
            e.HasMorePages = false;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            float x = e.MarginBounds.Left;
            float ancho = e.MarginBounds.Width;
            float y = e.MarginBounds.Top;

            float[] anchos = { 85, 75, 185, 185, 90 };
            string[] titulos = { "Fecha", "Hora", "Paciente", "Médico", "Estado" };
            float total = anchos.Sum();
            float alto = 26;

            using (Font fMarca = new Font("Segoe UI", 18, FontStyle.Bold))
            using (Font fTitulo = new Font("Segoe UI", 16, FontStyle.Bold))
            using (Font fSub = new Font("Segoe UI", 10))
            using (Font fCab = new Font("Segoe UI", 10, FontStyle.Bold))
            using (Font fFila = new Font("Segoe UI", 10))
            using (Font fPie = new Font("Segoe UI", 9))
            using (Brush azul = new SolidBrush(Color.MidnightBlue))
            using (Brush gris = new SolidBrush(Color.FromArgb(110, 110, 110)))
            using (Brush zebra = new SolidBrush(Color.FromArgb(243, 245, 250)))
            using (StringFormat sf = new StringFormat())
            {
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.LineAlignment = StringAlignment.Center;

                // Encabezado: solo en la primera página
                if (paginaPdf == 1)
                {
                    g.FillRectangle(azul, x, y, ancho, 80);

                    float xTexto = x + 20;
                    Image logo = Properties.Resources.catedrapoo1;   // cambia por el nombre del recurso con el logo completo
                    if (logo != null)
                    {
                        float caja = 66;
                        float xl = x + 10;
                        float yl = y + 7;
                        g.FillEllipse(Brushes.White, xl, yl, caja, caja);

                        float interior = caja - 10;
                        float escala = Math.Min(interior / logo.Width, interior / logo.Height);
                        float w = logo.Width * escala;
                        float h = logo.Height * escala;
                        g.DrawImage(logo, xl + (caja - w) / 2, yl + (caja - h) / 2, w, h);
                        xTexto = xl + caja + 15;
                    }

                    g.DrawString("SaludVital", fMarca, Brushes.White, xTexto, y + 8);
                    g.DrawString("Sistema de Gestión", fSub, Brushes.White, xTexto + 2, y + 44);
                    y += 100;

                    g.DrawString("Reporte de Citas", fTitulo, Brushes.Black, x, y);
                    y += 34;
                    g.DrawString(filtrosPdf, fSub, gris, x, y);
                    y += 20;
                    g.DrawString("Total de citas: " + filasPdf.Count, fSub, gris, x, y);
                    y += 30;
                }

                // Cabecera de la tabla (se repite en cada página)
                g.FillRectangle(azul, x, y, total, alto);
                float cx = x;
                for (int i = 0; i < titulos.Length; i++)
                {
                    g.DrawString(titulos[i], fCab, Brushes.White, new RectangleF(cx + 6, y, anchos[i] - 8, alto), sf);
                    cx += anchos[i];
                }
                y += alto;

                // Filas
                while (filaPdf < filasPdf.Count)
                {
                    if (y + alto > e.MarginBounds.Bottom - 30)
                    {
                        e.HasMorePages = true;
                        break;
                    }

                    if (filaPdf % 2 == 1) g.FillRectangle(zebra, x, y, total, alto);

                    cx = x;
                    for (int i = 0; i < anchos.Length; i++)
                    {
                        g.DrawString(filasPdf[filaPdf][i], fFila, Brushes.Black,
                                     new RectangleF(cx + 6, y, anchos[i] - 8, alto), sf);
                        cx += anchos[i];
                    }
                    y += alto;
                    filaPdf++;
                }

                g.DrawString("Generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "  -  Página " + paginaPdf,
                             fPie, gris, x, e.MarginBounds.Bottom - 15);
            }
        }
        public FrmVerReportesCitas()
        {
            InitializeComponent();
        }

        private void FrmVerReportesCitas_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

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
    }
}

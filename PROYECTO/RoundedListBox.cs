using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTO
{
    public partial class RoundedListBox : UserControl
    {
        private int _cornerRadius = 15;

        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = value;
                this.Invalidate();
            }
        }

        public RoundedListBox()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.BorderStyle = BorderStyle.None;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = new GraphicsPath())
            {
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int arcSize = _cornerRadius;

                path.AddArc(rect.X, rect.Y, arcSize, arcSize, 180, 90);
                path.AddArc(rect.Right - arcSize, rect.Y, arcSize, arcSize, 270, 90);
                path.AddArc(rect.Right - arcSize, rect.Bottom - arcSize, arcSize, arcSize, 0, 90);
                path.AddArc(rect.X, rect.Bottom - arcSize, arcSize, arcSize, 90, 90);
                path.CloseFigure();

                // Asigna la región con bordes redondeados al control
                this.Region = new Region(path);

                // Dibuja el fondo y los elementos de la lista
                base.OnPaint(e);

                // Opcional: Dibuja un borde alrededor
                using (Pen pen = new Pen(Color.DarkGray, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}

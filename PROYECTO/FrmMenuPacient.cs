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
    public partial class FrmMenuPacient : Form
    {

        public FrmMenuPacient()
        {
            InitializeComponent();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            FrmHistorialCitas frmPaciente = new FrmHistorialCitas();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            FrmVerReceta frmPaciente = new FrmVerReceta();
            frmPaciente.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            FrmMenuPacient frmPaciente = new FrmMenuPacient();
            frmPaciente.Show();
            this.Hide();
        }

        private void FrmMenuPacient_Load(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

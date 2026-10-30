namespace PROYECTO
{
    partial class FrmGestionarMedico
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvGestionarMedico = new System.Windows.Forms.DataGridView();
            this.txtBuscar = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnLimpiar = new Guna.UI2.WinForms.Guna2Button();
            this.btnEliminar = new Guna.UI2.WinForms.Guna2Button();
            this.btnGuardar = new Guna.UI2.WinForms.Guna2Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCorreo = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtTelefono = new Guna.UI2.WinForms.Guna2TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtApellido = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtNOmbre = new Guna.UI2.WinForms.Guna2TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.guna2GradientPanel2 = new Guna.UI2.WinForms.Guna2GradientPanel();
            this.guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2PictureBox1 = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Button9 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button3 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button4 = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Button5 = new Guna.UI2.WinForms.Guna2Button();
            this.cmbEspecialidad = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtContraseña = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtUsuario = new Guna.UI2.WinForms.Guna2TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGestionarMedico)).BeginInit();
            this.guna2GradientPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvGestionarMedico
            // 
            this.dgvGestionarMedico.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvGestionarMedico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGestionarMedico.Location = new System.Drawing.Point(472, 702);
            this.dgvGestionarMedico.Name = "dgvGestionarMedico";
            this.dgvGestionarMedico.RowHeadersWidth = 62;
            this.dgvGestionarMedico.RowTemplate.Height = 28;
            this.dgvGestionarMedico.Size = new System.Drawing.Size(765, 270);
            this.dgvGestionarMedico.TabIndex = 46;
            this.dgvGestionarMedico.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGestionarMedico_CellClick);
            // 
            // txtBuscar
            // 
            this.txtBuscar.AutoRoundedCorners = true;
            this.txtBuscar.BorderColor = System.Drawing.Color.Black;
            this.txtBuscar.BorderRadius = 22;
            this.txtBuscar.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBuscar.DefaultText = "";
            this.txtBuscar.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtBuscar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtBuscar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBuscar.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBuscar.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBuscar.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBuscar.Location = new System.Drawing.Point(472, 626);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.PlaceholderText = "";
            this.txtBuscar.SelectedText = "";
            this.txtBuscar.Size = new System.Drawing.Size(765, 46);
            this.txtBuscar.TabIndex = 45;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.AutoRoundedCorners = true;
            this.btnLimpiar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLimpiar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLimpiar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLimpiar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLimpiar.FillColor = System.Drawing.Color.Gray;
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(973, 530);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.PressedColor = System.Drawing.Color.BurlyWood;
            this.btnLimpiar.Size = new System.Drawing.Size(156, 57);
            this.btnLimpiar.TabIndex = 44;
            this.btnLimpiar.Text = "Limpiar";
            // 
            // btnEliminar
            // 
            this.btnEliminar.AutoRoundedCorners = true;
            this.btnEliminar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnEliminar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnEliminar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnEliminar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnEliminar.FillColor = System.Drawing.Color.Red;
            this.btnEliminar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(782, 530);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(156, 57);
            this.btnEliminar.TabIndex = 43;
            this.btnEliminar.Text = "Eliminar";
            // 
            // btnGuardar
            // 
            this.btnGuardar.AutoRoundedCorners = true;
            this.btnGuardar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGuardar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGuardar.FillColor = System.Drawing.Color.DarkGreen;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(588, 530);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(156, 57);
            this.btnGuardar.TabIndex = 41;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(477, 427);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 28);
            this.label8.TabIndex = 39;
            this.label8.Text = "Especialidad";
            // 
            // txtCorreo
            // 
            this.txtCorreo.AutoRoundedCorners = true;
            this.txtCorreo.BorderColor = System.Drawing.Color.Black;
            this.txtCorreo.BorderRadius = 22;
            this.txtCorreo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCorreo.DefaultText = "";
            this.txtCorreo.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtCorreo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtCorreo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCorreo.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtCorreo.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCorreo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCorreo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtCorreo.Location = new System.Drawing.Point(895, 254);
            this.txtCorreo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCorreo.Name = "txtCorreo";
            this.txtCorreo.PlaceholderText = "";
            this.txtCorreo.SelectedText = "";
            this.txtCorreo.Size = new System.Drawing.Size(342, 46);
            this.txtCorreo.TabIndex = 38;
            // 
            // txtTelefono
            // 
            this.txtTelefono.AutoRoundedCorners = true;
            this.txtTelefono.BorderColor = System.Drawing.Color.Black;
            this.txtTelefono.BorderRadius = 22;
            this.txtTelefono.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTelefono.DefaultText = "";
            this.txtTelefono.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtTelefono.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtTelefono.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTelefono.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtTelefono.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTelefono.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTelefono.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtTelefono.Location = new System.Drawing.Point(472, 254);
            this.txtTelefono.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.PlaceholderText = "";
            this.txtTelefono.SelectedText = "";
            this.txtTelefono.Size = new System.Drawing.Size(342, 46);
            this.txtTelefono.TabIndex = 37;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(906, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 28);
            this.label5.TabIndex = 36;
            this.label5.Text = "Correo";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(477, 211);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 28);
            this.label7.TabIndex = 35;
            this.label7.Text = "Telefono";
            // 
            // txtApellido
            // 
            this.txtApellido.AutoRoundedCorners = true;
            this.txtApellido.BorderColor = System.Drawing.Color.Black;
            this.txtApellido.BorderRadius = 22;
            this.txtApellido.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtApellido.DefaultText = "";
            this.txtApellido.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtApellido.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtApellido.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtApellido.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtApellido.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtApellido.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtApellido.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtApellido.Location = new System.Drawing.Point(895, 146);
            this.txtApellido.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtApellido.Name = "txtApellido";
            this.txtApellido.PlaceholderText = "";
            this.txtApellido.SelectedText = "";
            this.txtApellido.Size = new System.Drawing.Size(342, 46);
            this.txtApellido.TabIndex = 34;
            // 
            // txtNOmbre
            // 
            this.txtNOmbre.AutoRoundedCorners = true;
            this.txtNOmbre.BorderColor = System.Drawing.Color.Black;
            this.txtNOmbre.BorderRadius = 22;
            this.txtNOmbre.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNOmbre.DefaultText = "";
            this.txtNOmbre.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtNOmbre.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtNOmbre.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNOmbre.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtNOmbre.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtNOmbre.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNOmbre.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtNOmbre.Location = new System.Drawing.Point(472, 146);
            this.txtNOmbre.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtNOmbre.Name = "txtNOmbre";
            this.txtNOmbre.PlaceholderText = "";
            this.txtNOmbre.SelectedText = "";
            this.txtNOmbre.Size = new System.Drawing.Size(342, 46);
            this.txtNOmbre.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(906, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 28);
            this.label4.TabIndex = 32;
            this.label4.Text = "Apellido";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(477, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 28);
            this.label3.TabIndex = 31;
            this.label3.Text = "Nombre";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(423, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 45);
            this.label2.TabIndex = 30;
            this.label2.Text = "Gestionar Medico";
            // 
            // guna2GradientPanel2
            // 
            this.guna2GradientPanel2.BackColor = System.Drawing.Color.White;
            this.guna2GradientPanel2.Controls.Add(this.guna2Button1);
            this.guna2GradientPanel2.Controls.Add(this.label6);
            this.guna2GradientPanel2.Controls.Add(this.label1);
            this.guna2GradientPanel2.Controls.Add(this.guna2PictureBox1);
            this.guna2GradientPanel2.Controls.Add(this.guna2Button9);
            this.guna2GradientPanel2.Controls.Add(this.guna2Button2);
            this.guna2GradientPanel2.Controls.Add(this.guna2Button3);
            this.guna2GradientPanel2.Controls.Add(this.guna2Button4);
            this.guna2GradientPanel2.Controls.Add(this.guna2Button5);
            this.guna2GradientPanel2.Location = new System.Drawing.Point(1, -2);
            this.guna2GradientPanel2.Name = "guna2GradientPanel2";
            this.guna2GradientPanel2.Size = new System.Drawing.Size(365, 1003);
            this.guna2GradientPanel2.TabIndex = 47;
            // 
            // guna2Button1
            // 
            this.guna2Button1.BackColor = System.Drawing.Color.MintCream;
            this.guna2Button1.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button1.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button1.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button1.FillColor = System.Drawing.Color.White;
            this.guna2Button1.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.guna2Button1.ForeColor = System.Drawing.Color.Black;
            this.guna2Button1.Image = global::PROYECTO.Properties.Resources.usuarios;
            this.guna2Button1.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button1.Location = new System.Drawing.Point(11, 521);
            this.guna2Button1.Name = "guna2Button1";
            this.guna2Button1.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.guna2Button1.Size = new System.Drawing.Size(351, 68);
            this.guna2Button1.TabIndex = 9;
            this.guna2Button1.Text = "Usuarios";
            this.guna2Button1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button1.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Location = new System.Drawing.Point(170, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(193, 28);
            this.label6.TabIndex = 8;
            this.label6.Text = "Sistema de Gestion";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(181, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 45);
            this.label1.TabIndex = 6;
            this.label1.Text = "SaludVital";
            // 
            // guna2PictureBox1
            // 
            this.guna2PictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2PictureBox1.Image = global::PROYECTO.Properties.Resources.catedrapoo1;
            this.guna2PictureBox1.ImageRotate = 0F;
            this.guna2PictureBox1.Location = new System.Drawing.Point(3, 0);
            this.guna2PictureBox1.Name = "guna2PictureBox1";
            this.guna2PictureBox1.Size = new System.Drawing.Size(172, 172);
            this.guna2PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2PictureBox1.TabIndex = 7;
            this.guna2PictureBox1.TabStop = false;
            this.guna2PictureBox1.UseTransparentBackground = true;
            // 
            // guna2Button9
            // 
            this.guna2Button9.BackColor = System.Drawing.Color.MintCream;
            this.guna2Button9.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button9.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button9.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button9.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button9.FillColor = System.Drawing.Color.White;
            this.guna2Button9.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.guna2Button9.ForeColor = System.Drawing.Color.Black;
            this.guna2Button9.Image = global::PROYECTO.Properties.Resources.especialidades;
            this.guna2Button9.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button9.Location = new System.Drawing.Point(11, 393);
            this.guna2Button9.Name = "guna2Button9";
            this.guna2Button9.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.guna2Button9.Size = new System.Drawing.Size(351, 57);
            this.guna2Button9.TabIndex = 6;
            this.guna2Button9.Text = "Especialidades";
            this.guna2Button9.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button9.Click += new System.EventHandler(this.guna2Button9_Click);
            // 
            // guna2Button2
            // 
            this.guna2Button2.BackColor = System.Drawing.Color.MintCream;
            this.guna2Button2.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button2.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button2.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button2.FillColor = System.Drawing.Color.White;
            this.guna2Button2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.guna2Button2.ForeColor = System.Drawing.Color.Black;
            this.guna2Button2.Image = global::PROYECTO.Properties.Resources.reportes;
            this.guna2Button2.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button2.Location = new System.Drawing.Point(11, 456);
            this.guna2Button2.Name = "guna2Button2";
            this.guna2Button2.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.guna2Button2.Size = new System.Drawing.Size(351, 68);
            this.guna2Button2.TabIndex = 4;
            this.guna2Button2.Text = "Reportes";
            this.guna2Button2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button2.Click += new System.EventHandler(this.guna2Button2_Click);
            // 
            // guna2Button3
            // 
            this.guna2Button3.BackColor = System.Drawing.Color.MintCream;
            this.guna2Button3.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button3.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button3.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button3.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button3.FillColor = System.Drawing.Color.White;
            this.guna2Button3.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.guna2Button3.ForeColor = System.Drawing.Color.Black;
            this.guna2Button3.Image = global::PROYECTO.Properties.Resources.medicos;
            this.guna2Button3.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button3.Location = new System.Drawing.Point(11, 319);
            this.guna2Button3.Name = "guna2Button3";
            this.guna2Button3.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.guna2Button3.Size = new System.Drawing.Size(351, 68);
            this.guna2Button3.TabIndex = 3;
            this.guna2Button3.Text = "Medicos";
            this.guna2Button3.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button3.Click += new System.EventHandler(this.guna2Button3_Click);
            // 
            // guna2Button4
            // 
            this.guna2Button4.BackColor = System.Drawing.Color.MintCream;
            this.guna2Button4.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button4.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button4.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button4.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button4.FillColor = System.Drawing.Color.White;
            this.guna2Button4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.guna2Button4.ForeColor = System.Drawing.Color.Black;
            this.guna2Button4.Image = global::PROYECTO.Properties.Resources.pacientes;
            this.guna2Button4.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button4.Location = new System.Drawing.Point(11, 256);
            this.guna2Button4.Name = "guna2Button4";
            this.guna2Button4.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.guna2Button4.Size = new System.Drawing.Size(351, 57);
            this.guna2Button4.TabIndex = 2;
            this.guna2Button4.Text = "Pacientes";
            this.guna2Button4.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button4.Click += new System.EventHandler(this.guna2Button4_Click);
            // 
            // guna2Button5
            // 
            this.guna2Button5.BackColor = System.Drawing.Color.MediumBlue;
            this.guna2Button5.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button5.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.guna2Button5.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.guna2Button5.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.guna2Button5.FillColor = System.Drawing.Color.White;
            this.guna2Button5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.guna2Button5.ForeColor = System.Drawing.Color.Black;
            this.guna2Button5.Image = global::PROYECTO.Properties.Resources.dashboard__2_;
            this.guna2Button5.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button5.Location = new System.Drawing.Point(11, 193);
            this.guna2Button5.Name = "guna2Button5";
            this.guna2Button5.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.guna2Button5.Size = new System.Drawing.Size(351, 57);
            this.guna2Button5.TabIndex = 1;
            this.guna2Button5.Text = "Dashboard";
            this.guna2Button5.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.guna2Button5.Click += new System.EventHandler(this.guna2Button5_Click);
            // 
            // cmbEspecialidad
            // 
            this.cmbEspecialidad.AutoRoundedCorners = true;
            this.cmbEspecialidad.BackColor = System.Drawing.Color.Transparent;
            this.cmbEspecialidad.BorderColor = System.Drawing.Color.Black;
            this.cmbEspecialidad.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEspecialidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEspecialidad.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbEspecialidad.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbEspecialidad.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbEspecialidad.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbEspecialidad.ItemHeight = 30;
            this.cmbEspecialidad.Location = new System.Drawing.Point(482, 458);
            this.cmbEspecialidad.Name = "cmbEspecialidad";
            this.cmbEspecialidad.Size = new System.Drawing.Size(755, 36);
            this.cmbEspecialidad.TabIndex = 48;
            // 
            // txtContraseña
            // 
            this.txtContraseña.AutoRoundedCorners = true;
            this.txtContraseña.BorderColor = System.Drawing.Color.Black;
            this.txtContraseña.BorderRadius = 22;
            this.txtContraseña.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContraseña.DefaultText = "";
            this.txtContraseña.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtContraseña.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtContraseña.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtContraseña.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtContraseña.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtContraseña.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtContraseña.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtContraseña.Location = new System.Drawing.Point(895, 364);
            this.txtContraseña.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtContraseña.Name = "txtContraseña";
            this.txtContraseña.PlaceholderText = "";
            this.txtContraseña.SelectedText = "";
            this.txtContraseña.Size = new System.Drawing.Size(342, 46);
            this.txtContraseña.TabIndex = 52;
            // 
            // txtUsuario
            // 
            this.txtUsuario.AutoRoundedCorners = true;
            this.txtUsuario.BorderColor = System.Drawing.Color.Black;
            this.txtUsuario.BorderRadius = 22;
            this.txtUsuario.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsuario.DefaultText = "";
            this.txtUsuario.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtUsuario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtUsuario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtUsuario.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtUsuario.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUsuario.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtUsuario.Location = new System.Drawing.Point(472, 364);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.PlaceholderText = "";
            this.txtUsuario.SelectedText = "";
            this.txtUsuario.Size = new System.Drawing.Size(342, 46);
            this.txtUsuario.TabIndex = 51;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(906, 321);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 28);
            this.label9.TabIndex = 50;
            this.label9.Text = "Contraseña";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(477, 321);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 28);
            this.label10.TabIndex = 49;
            this.label10.Text = "Usuario";
            // 
            // FrmGestionarMedico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1292, 1002);
            this.Controls.Add(this.txtContraseña);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cmbEspecialidad);
            this.Controls.Add(this.guna2GradientPanel2);
            this.Controls.Add(this.dgvGestionarMedico);
            this.Controls.Add(this.txtBuscar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtCorreo);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtApellido);
            this.Controls.Add(this.txtNOmbre);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "FrmGestionarMedico";
            this.Text = "FrmGestionarMedico";
            this.Load += new System.EventHandler(this.FrmGestionarMedico_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGestionarMedico)).EndInit();
            this.guna2GradientPanel2.ResumeLayout(false);
            this.guna2GradientPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvGestionarMedico;
        private Guna.UI2.WinForms.Guna2TextBox txtBuscar;
        private Guna.UI2.WinForms.Guna2Button btnLimpiar;
        private Guna.UI2.WinForms.Guna2Button btnEliminar;
        private Guna.UI2.WinForms.Guna2Button btnGuardar;
        private System.Windows.Forms.Label label8;
        private Guna.UI2.WinForms.Guna2TextBox txtCorreo;
        private Guna.UI2.WinForms.Guna2TextBox txtTelefono;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private Guna.UI2.WinForms.Guna2TextBox txtApellido;
        private Guna.UI2.WinForms.Guna2TextBox txtNOmbre;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2GradientPanel guna2GradientPanel2;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2PictureBox guna2PictureBox1;
        private Guna.UI2.WinForms.Guna2Button guna2Button9;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private Guna.UI2.WinForms.Guna2Button guna2Button3;
        private Guna.UI2.WinForms.Guna2Button guna2Button4;
        private Guna.UI2.WinForms.Guna2Button guna2Button5;
        private Guna.UI2.WinForms.Guna2ComboBox cmbEspecialidad;
        private Guna.UI2.WinForms.Guna2TextBox txtContraseña;
        private Guna.UI2.WinForms.Guna2TextBox txtUsuario;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}
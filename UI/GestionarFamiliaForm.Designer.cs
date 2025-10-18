
namespace UI
{
    partial class GestionarFamiliaForm
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
            this.btnFinalizar = new System.Windows.Forms.Button();
            this.lblDisponibles = new System.Windows.Forms.Label();
            this.dgvDisponibles = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAsignadas = new System.Windows.Forms.Label();
            this.dgvAsignadas = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.lblId = new System.Windows.Forms.Label();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponibles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignadas)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFinalizar
            // 
            this.btnFinalizar.Location = new System.Drawing.Point(307, 292);
            this.btnFinalizar.Name = "btnFinalizar";
            this.btnFinalizar.Size = new System.Drawing.Size(92, 30);
            this.btnFinalizar.TabIndex = 19;
            this.btnFinalizar.Text = "Modificar";
            this.btnFinalizar.Click += new System.EventHandler(this.btnFinalizar_Click);
            // 
            // lblDisponibles
            // 
            this.lblDisponibles.Location = new System.Drawing.Point(15, 94);
            this.lblDisponibles.Name = "lblDisponibles";
            this.lblDisponibles.Size = new System.Drawing.Size(100, 23);
            this.lblDisponibles.TabIndex = 11;
            this.lblDisponibles.Text = "No asignadas:";
            // 
            // dgvDisponibles
            // 
            this.dgvDisponibles.AllowUserToAddRows = false;
            this.dgvDisponibles.AllowUserToDeleteRows = false;
            this.dgvDisponibles.AllowUserToResizeRows = false;
            this.dgvDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDisponibles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3});
            this.dgvDisponibles.Location = new System.Drawing.Point(15, 120);
            this.dgvDisponibles.Name = "dgvDisponibles";
            this.dgvDisponibles.ReadOnly = true;
            this.dgvDisponibles.Size = new System.Drawing.Size(308, 150);
            this.dgvDisponibles.TabIndex = 12;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Familia";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // lblAsignadas
            // 
            this.lblAsignadas.Location = new System.Drawing.Point(382, 94);
            this.lblAsignadas.Name = "lblAsignadas";
            this.lblAsignadas.Size = new System.Drawing.Size(100, 23);
            this.lblAsignadas.TabIndex = 13;
            this.lblAsignadas.Text = "Asignadas:";
            // 
            // dgvAsignadas
            // 
            this.dgvAsignadas.AllowUserToAddRows = false;
            this.dgvAsignadas.AllowUserToDeleteRows = false;
            this.dgvAsignadas.AllowUserToResizeRows = false;
            this.dgvAsignadas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsignadas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4});
            this.dgvAsignadas.Location = new System.Drawing.Point(385, 120);
            this.dgvAsignadas.Name = "dgvAsignadas";
            this.dgvAsignadas.ReadOnly = true;
            this.dgvAsignadas.Size = new System.Drawing.Size(303, 150);
            this.dgvAsignadas.TabIndex = 14;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Familia";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(329, 163);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(50, 30);
            this.btnAgregar.TabIndex = 15;
            this.btnAgregar.Text = "→";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(329, 203);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(50, 30);
            this.btnEliminar.TabIndex = 16;
            this.btnEliminar.Text = "←";
            // 
            // lblId
            // 
            this.lblId.Location = new System.Drawing.Point(16, 9);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(79, 20);
            this.lblId.TabIndex = 20;
            this.lblId.Text = "ID:";
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(122, 9);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(100, 20);
            this.txtId.TabIndex = 21;
            // 
            // lblNombre
            // 
            this.lblNombre.Location = new System.Drawing.Point(13, 35);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(82, 20);
            this.lblNombre.TabIndex = 22;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(122, 35);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(361, 20);
            this.txtNombre.TabIndex = 23;
            // 
            // lblDesc
            // 
            this.lblDesc.Location = new System.Drawing.Point(12, 64);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(83, 20);
            this.lblDesc.TabIndex = 24;
            this.lblDesc.Text = "Descripcion:";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(122, 61);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(361, 20);
            this.txtDesc.TabIndex = 25;
            // 
            // GestionarFamiliaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 334);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblDesc);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.btnFinalizar);
            this.Controls.Add(this.lblDisponibles);
            this.Controls.Add(this.dgvDisponibles);
            this.Controls.Add(this.lblAsignadas);
            this.Controls.Add(this.dgvAsignadas);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.btnEliminar);
            this.Name = "GestionarFamiliaForm";
            this.Text = "GestionarFamiliaForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponibles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignadas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFinalizar;
        private System.Windows.Forms.Label lblDisponibles;
        private System.Windows.Forms.DataGridView dgvDisponibles;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Label lblAsignadas;
        private System.Windows.Forms.DataGridView dgvAsignadas;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Button btnAgregar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.TextBox txtDesc;
    }
}
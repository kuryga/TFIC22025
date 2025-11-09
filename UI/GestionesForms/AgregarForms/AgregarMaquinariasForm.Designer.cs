using System;
using System.Windows.Forms;

namespace WinApp
{
    partial class AgregarMaquinariasForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitulo;
        private DataGridView dgvMaquinarias;
        private Button btnAgregar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dgvMaquinarias = new System.Windows.Forms.DataGridView();
            this.btnAgregar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaquinarias)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(91, 19);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Maquinarias";
            // 
            // dgvMaquinarias
            // 
            this.dgvMaquinarias.AllowUserToAddRows = false;
            this.dgvMaquinarias.AllowUserToDeleteRows = false;
            this.dgvMaquinarias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaquinarias.Location = new System.Drawing.Point(20, 45);
            this.dgvMaquinarias.Name = "dgvMaquinarias";
            this.dgvMaquinarias.ReadOnly = true;
            this.dgvMaquinarias.Size = new System.Drawing.Size(740, 300);
            this.dgvMaquinarias.TabIndex = 1;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(20, 360);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(180, 30);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "Agregar Maquinaria";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // AgregarMaquinariasForm
            // 
            this.ClientSize = new System.Drawing.Size(770, 399);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.dgvMaquinarias);
            this.Controls.Add(this.btnAgregar);
            this.Name = "AgregarMaquinariasForm";
            this.Text = "Agregar Maquinarias";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaquinarias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

using System;
using System.Windows.Forms;

namespace WinApp
{
    partial class AgregarMaterialesForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitulo;
        private DataGridView dgvMateriales;
        private Button btnAgregar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dgvMateriales = new System.Windows.Forms.DataGridView();
            this.btnAgregar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMateriales)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(79, 19);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Materiales";
            // 
            // dgvMateriales
            // 
            this.dgvMateriales.AllowUserToAddRows = false;
            this.dgvMateriales.AllowUserToDeleteRows = false;
            this.dgvMateriales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMateriales.Location = new System.Drawing.Point(20, 45);
            this.dgvMateriales.Name = "dgvMateriales";
            this.dgvMateriales.ReadOnly = true;
            this.dgvMateriales.Size = new System.Drawing.Size(740, 300);
            this.dgvMateriales.TabIndex = 1;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(20, 360);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(150, 30);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "Agregar Material";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // AgregarMaterialesForm
            // 
            this.ClientSize = new System.Drawing.Size(774, 399);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.dgvMateriales);
            this.Controls.Add(this.btnAgregar);
            this.Name = "AgregarMaterialesForm";
            this.Text = "Agregar Materiales";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMateriales)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

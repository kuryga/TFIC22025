using System;
using System.Windows.Forms;

namespace WinApp
{
    partial class AgregarServiciosAdicionalesForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitulo;
        private DataGridView dgvServicios;
        private Button btnAgregar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dgvServicios = new System.Windows.Forms.DataGridView();
            this.btnAgregar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(151, 19);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Servicios Adicionales";
            // 
            // dgvServicios
            // 
            this.dgvServicios.AllowUserToAddRows = false;
            this.dgvServicios.AllowUserToDeleteRows = false;
            this.dgvServicios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServicios.Location = new System.Drawing.Point(20, 45);
            this.dgvServicios.Name = "dgvServicios";
            this.dgvServicios.ReadOnly = true;
            this.dgvServicios.Size = new System.Drawing.Size(740, 300);
            this.dgvServicios.TabIndex = 1;
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(20, 360);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(180, 30);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "Agregar Servicio";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // AgregarServiciosAdicionalesForm
            // 
            this.ClientSize = new System.Drawing.Size(777, 400);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.dgvServicios);
            this.Controls.Add(this.btnAgregar);
            this.Name = "AgregarServiciosAdicionalesForm";
            this.Text = "Agregar Servicios Adicionales";
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

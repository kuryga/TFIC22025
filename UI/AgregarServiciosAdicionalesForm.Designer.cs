using System;
using System.Windows.Forms;

namespace UI
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
            this.lblTitulo = new Label();
            this.dgvServicios = new DataGridView();
            this.btnAgregar = new Button();

            this.SuspendLayout();

            this.lblTitulo.Text = "Servicios Adicionales";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.AutoSize = true;

            this.dgvServicios.Location = new System.Drawing.Point(20, 45);
            this.dgvServicios.Size = new System.Drawing.Size(740, 300);
            this.dgvServicios.ReadOnly = true;
            this.dgvServicios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServicios.AllowUserToAddRows = false;
            this.dgvServicios.AllowUserToDeleteRows = false;

            this.btnAgregar.Text = "Agregar Servicio";
            this.btnAgregar.Location = new System.Drawing.Point(20, 360);
            this.btnAgregar.Size = new System.Drawing.Size(180, 30);
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);

            this.ClientSize = new System.Drawing.Size(800, 420);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.dgvServicios);
            this.Controls.Add(this.btnAgregar);
            this.Name = "AgregarServiciosAdicionalesForm";
            this.Text = "Agregar Servicios Adicionales";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

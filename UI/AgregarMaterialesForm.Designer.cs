using System;
using System.Windows.Forms;

namespace UI
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
            this.lblTitulo = new Label();
            this.dgvMateriales = new DataGridView();
            this.btnAgregar = new Button();

            this.SuspendLayout();

            // Label Título
            this.lblTitulo.Text = "Materiales";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.AutoSize = true;

            // DataGridView
            this.dgvMateriales.Location = new System.Drawing.Point(20, 45);
            this.dgvMateriales.Size = new System.Drawing.Size(740, 300);
            this.dgvMateriales.ReadOnly = true;
            this.dgvMateriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMateriales.AllowUserToAddRows = false;
            this.dgvMateriales.AllowUserToDeleteRows = false;

            // Botón Agregar
            this.btnAgregar.Text = "Agregar Material";
            this.btnAgregar.Location = new System.Drawing.Point(20, 360);
            this.btnAgregar.Size = new System.Drawing.Size(150, 30);
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);

            // Formulario
            this.ClientSize = new System.Drawing.Size(800, 420);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.dgvMateriales);
            this.Controls.Add(this.btnAgregar);
            this.Name = "AgregarMaterialesForm";
            this.Text = "Agregar Materiales";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

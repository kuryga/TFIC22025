using System;
using System.Windows.Forms;

namespace UI.Forms
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
            this.lblTitulo = new Label();
            this.dgvMaquinarias = new DataGridView();
            this.btnAgregar = new Button();

            this.SuspendLayout();

            this.lblTitulo.Text = "Maquinarias";
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.AutoSize = true;

            this.dgvMaquinarias.Location = new System.Drawing.Point(20, 45);
            this.dgvMaquinarias.Size = new System.Drawing.Size(740, 300);
            this.dgvMaquinarias.ReadOnly = true;
            this.dgvMaquinarias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaquinarias.AllowUserToAddRows = false;
            this.dgvMaquinarias.AllowUserToDeleteRows = false;

            this.btnAgregar.Text = "Agregar Maquinaria";
            this.btnAgregar.Location = new System.Drawing.Point(20, 360);
            this.btnAgregar.Size = new System.Drawing.Size(180, 30);
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);

            this.ClientSize = new System.Drawing.Size(800, 420);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.dgvMaquinarias);
            this.Controls.Add(this.btnAgregar);
            this.Name = "AgregarMaquinariasForm";
            this.Text = "Agregar Maquinarias";

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

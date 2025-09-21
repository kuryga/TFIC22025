using System;
using System.Windows.Forms;

namespace UI
{
    partial class NuevaCotizacionForm
    {
        private System.ComponentModel.IContainer components = null;
        private ComboBox cmbTipoEdificacion;
        private ComboBox cmbMoneda;
        private DataGridView dgvMateriales;
        private DataGridView dgvMaquinarias;
        private DataGridView dgvServicios;
        private Button btnAgregarMaterial;
        private Button btnAgregarMaquinaria;
        private Button btnAgregarServicio;
        private Label lblTotal;
        private Button btnGuardar;
        private Label lblTipo;
        private Label lblMoneda;
        private Label lblMateriales;
        private Label lblMaquinarias;
        private Label lblServicios;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cmbTipoEdificacion = new ComboBox();
            this.cmbMoneda = new ComboBox();
            this.dgvMateriales = new DataGridView();
            this.dgvMaquinarias = new DataGridView();
            this.dgvServicios = new DataGridView();
            this.btnAgregarMaterial = new Button();
            this.btnAgregarMaquinaria = new Button();
            this.btnAgregarServicio = new Button();
            this.lblTotal = new Label();
            this.btnGuardar = new Button();
            this.lblTipo = new Label();
            this.lblMoneda = new Label();
            this.lblMateriales = new Label();
            this.lblMaquinarias = new Label();
            this.lblServicios = new Label();

            this.SuspendLayout();

            // Label Tipo Edificación
            this.lblTipo.Text = "Tipo de Edificación:";
            this.lblTipo.Location = new System.Drawing.Point(20, 20);
            this.lblTipo.AutoSize = true;

            // Combo Tipo Edificación
            this.cmbTipoEdificacion.Location = new System.Drawing.Point(160, 18);
            this.cmbTipoEdificacion.Size = new System.Drawing.Size(200, 21);
            this.cmbTipoEdificacion.Items.AddRange(new object[] { "Vivienda unifamiliar", "Edificio comercial", "Edificio Residencial" });

            // Label Moneda
            this.lblMoneda.Text = "Moneda:";
            this.lblMoneda.Location = new System.Drawing.Point(400, 20);
            this.lblMoneda.AutoSize = true;

            // Combo Moneda
            this.cmbMoneda.Location = new System.Drawing.Point(470, 18);
            this.cmbMoneda.Size = new System.Drawing.Size(150, 21);
            this.cmbMoneda.Items.AddRange(new object[] { "USD", "EUR", "ARS" });

            // Label Materiales
            this.lblMateriales.Text = "Materiales:";
            this.lblMateriales.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            this.lblMateriales.Location = new System.Drawing.Point(20, 50);
            this.lblMateriales.AutoSize = true;

            // DataGrid Materiales
            this.dgvMateriales.Location = new System.Drawing.Point(20, 70);
            this.dgvMateriales.Size = new System.Drawing.Size(740, 100);
            this.dgvMateriales.ReadOnly = true;
            this.dgvMateriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Botón agregar Material
            this.btnAgregarMaterial.Text = "Agregar Materiales";
            this.btnAgregarMaterial.Location = new System.Drawing.Point(20, 175);
            this.btnAgregarMaterial.Click += new System.EventHandler(this.btnAgregarMaterial_Click);

            // Label Maquinarias
            this.lblMaquinarias.Text = "Maquinarias:";
            this.lblMaquinarias.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            this.lblMaquinarias.Location = new System.Drawing.Point(20, 210);
            this.lblMaquinarias.AutoSize = true;

            // DataGrid Maquinarias
            this.dgvMaquinarias.Location = new System.Drawing.Point(20, 230);
            this.dgvMaquinarias.Size = new System.Drawing.Size(740, 100);
            this.dgvMaquinarias.ReadOnly = true;
            this.dgvMaquinarias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Botón agregar Maquinaria
            this.btnAgregarMaquinaria.Text = "Agregar Maquinarias";
            this.btnAgregarMaquinaria.Location = new System.Drawing.Point(20, 335);
            this.btnAgregarMaquinaria.Click += new System.EventHandler(this.btnAgregarMaquinaria_Click);

            // Label Servicios
            this.lblServicios.Text = "Servicios Adicionales:";
            this.lblServicios.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            this.lblServicios.Location = new System.Drawing.Point(20, 370);
            this.lblServicios.AutoSize = true;

            // DataGrid Servicios
            this.dgvServicios.Location = new System.Drawing.Point(20, 390);
            this.dgvServicios.Size = new System.Drawing.Size(740, 100);
            this.dgvServicios.ReadOnly = true;
            this.dgvServicios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Botón agregar Servicio
            this.btnAgregarServicio.Text = "Agregar Servicios";
            this.btnAgregarServicio.Location = new System.Drawing.Point(20, 495);
            this.btnAgregarServicio.Click += new System.EventHandler(this.btnAgregarServicio_Click);

            // Label total
            this.lblTotal.Text = "Costo Total: $123,456.00";
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(600, 500);
            this.lblTotal.AutoSize = true;

            // Botón guardar
            this.btnGuardar.Text = "Guardar Cotización";
            this.btnGuardar.Location = new System.Drawing.Point(600, 530);
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);

            // Formulario
            this.ClientSize = new System.Drawing.Size(800, 580);
            this.Controls.AddRange(new Control[] {
                this.lblTipo, this.cmbTipoEdificacion,
                this.lblMoneda, this.cmbMoneda,
                this.lblMateriales, this.dgvMateriales, this.btnAgregarMaterial,
                this.lblMaquinarias, this.dgvMaquinarias, this.btnAgregarMaquinaria,
                this.lblServicios, this.dgvServicios, this.btnAgregarServicio,
                this.lblTotal, this.btnGuardar
            });

            this.Name = "NuevaCotizacionForm";
            this.Text = "Nueva Cotización";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

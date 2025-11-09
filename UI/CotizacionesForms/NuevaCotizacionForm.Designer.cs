using System;
using System.Windows.Forms;

namespace WinApp
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
            this.cmbTipoEdificacion = new System.Windows.Forms.ComboBox();
            this.cmbMoneda = new System.Windows.Forms.ComboBox();
            this.dgvMateriales = new System.Windows.Forms.DataGridView();
            this.dgvMaquinarias = new System.Windows.Forms.DataGridView();
            this.dgvServicios = new System.Windows.Forms.DataGridView();
            this.btnAgregarMaterial = new System.Windows.Forms.Button();
            this.btnAgregarMaquinaria = new System.Windows.Forms.Button();
            this.btnAgregarServicio = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.lblTipo = new System.Windows.Forms.Label();
            this.lblMoneda = new System.Windows.Forms.Label();
            this.lblMateriales = new System.Windows.Forms.Label();
            this.lblMaquinarias = new System.Windows.Forms.Label();
            this.lblServicios = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMateriales)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaquinarias)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbTipoEdificacion
            // 
            this.cmbTipoEdificacion.Items.AddRange(new object[] {
            "Vivienda unifamiliar",
            "Edificio comercial",
            "Edificio Residencial"});
            this.cmbTipoEdificacion.Location = new System.Drawing.Point(160, 18);
            this.cmbTipoEdificacion.Name = "cmbTipoEdificacion";
            this.cmbTipoEdificacion.Size = new System.Drawing.Size(200, 21);
            this.cmbTipoEdificacion.TabIndex = 1;
            // 
            // cmbMoneda
            // 
            this.cmbMoneda.Items.AddRange(new object[] {
            "USD",
            "EUR",
            "ARS"});
            this.cmbMoneda.Location = new System.Drawing.Point(470, 18);
            this.cmbMoneda.Name = "cmbMoneda";
            this.cmbMoneda.Size = new System.Drawing.Size(150, 21);
            this.cmbMoneda.TabIndex = 3;
            // 
            // dgvMateriales
            // 
            this.dgvMateriales.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMateriales.Location = new System.Drawing.Point(20, 70);
            this.dgvMateriales.Name = "dgvMateriales";
            this.dgvMateriales.ReadOnly = true;
            this.dgvMateriales.Size = new System.Drawing.Size(740, 100);
            this.dgvMateriales.TabIndex = 5;
            // 
            // dgvMaquinarias
            // 
            this.dgvMaquinarias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaquinarias.Location = new System.Drawing.Point(20, 230);
            this.dgvMaquinarias.Name = "dgvMaquinarias";
            this.dgvMaquinarias.ReadOnly = true;
            this.dgvMaquinarias.Size = new System.Drawing.Size(740, 100);
            this.dgvMaquinarias.TabIndex = 8;
            // 
            // dgvServicios
            // 
            this.dgvServicios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvServicios.Location = new System.Drawing.Point(20, 390);
            this.dgvServicios.Name = "dgvServicios";
            this.dgvServicios.ReadOnly = true;
            this.dgvServicios.Size = new System.Drawing.Size(740, 100);
            this.dgvServicios.TabIndex = 11;
            // 
            // btnAgregarMaterial
            // 
            this.btnAgregarMaterial.Location = new System.Drawing.Point(20, 175);
            this.btnAgregarMaterial.Name = "btnAgregarMaterial";
            this.btnAgregarMaterial.Size = new System.Drawing.Size(75, 23);
            this.btnAgregarMaterial.TabIndex = 6;
            this.btnAgregarMaterial.Text = "Agregar Materiales";
            this.btnAgregarMaterial.Click += new System.EventHandler(this.btnAgregarMaterial_Click);
            // 
            // btnAgregarMaquinaria
            // 
            this.btnAgregarMaquinaria.Location = new System.Drawing.Point(20, 335);
            this.btnAgregarMaquinaria.Name = "btnAgregarMaquinaria";
            this.btnAgregarMaquinaria.Size = new System.Drawing.Size(75, 23);
            this.btnAgregarMaquinaria.TabIndex = 9;
            this.btnAgregarMaquinaria.Text = "Agregar Maquinarias";
            this.btnAgregarMaquinaria.Click += new System.EventHandler(this.btnAgregarMaquinaria_Click);
            // 
            // btnAgregarServicio
            // 
            this.btnAgregarServicio.Location = new System.Drawing.Point(20, 495);
            this.btnAgregarServicio.Name = "btnAgregarServicio";
            this.btnAgregarServicio.Size = new System.Drawing.Size(75, 23);
            this.btnAgregarServicio.TabIndex = 12;
            this.btnAgregarServicio.Text = "Agregar Servicios";
            this.btnAgregarServicio.Click += new System.EventHandler(this.btnAgregarServicio_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(600, 500);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(172, 19);
            this.lblTotal.TabIndex = 13;
            this.lblTotal.Text = "Costo Total: $123,456.00";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(600, 530);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 14;
            this.btnGuardar.Text = "Guardar Cotización";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // lblTipo
            // 
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(20, 20);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(101, 13);
            this.lblTipo.TabIndex = 0;
            this.lblTipo.Text = "Tipo de Edificación:";
            // 
            // lblMoneda
            // 
            this.lblMoneda.AutoSize = true;
            this.lblMoneda.Location = new System.Drawing.Point(400, 20);
            this.lblMoneda.Name = "lblMoneda";
            this.lblMoneda.Size = new System.Drawing.Size(49, 13);
            this.lblMoneda.TabIndex = 2;
            this.lblMoneda.Text = "Moneda:";
            // 
            // lblMateriales
            // 
            this.lblMateriales.AutoSize = true;
            this.lblMateriales.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMateriales.Location = new System.Drawing.Point(20, 50);
            this.lblMateriales.Name = "lblMateriales";
            this.lblMateriales.Size = new System.Drawing.Size(68, 15);
            this.lblMateriales.TabIndex = 4;
            this.lblMateriales.Text = "Materiales:";
            // 
            // lblMaquinarias
            // 
            this.lblMaquinarias.AutoSize = true;
            this.lblMaquinarias.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMaquinarias.Location = new System.Drawing.Point(20, 210);
            this.lblMaquinarias.Name = "lblMaquinarias";
            this.lblMaquinarias.Size = new System.Drawing.Size(76, 15);
            this.lblMaquinarias.TabIndex = 7;
            this.lblMaquinarias.Text = "Maquinarias:";
            // 
            // lblServicios
            // 
            this.lblServicios.AutoSize = true;
            this.lblServicios.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblServicios.Location = new System.Drawing.Point(20, 370);
            this.lblServicios.Name = "lblServicios";
            this.lblServicios.Size = new System.Drawing.Size(125, 15);
            this.lblServicios.TabIndex = 10;
            this.lblServicios.Text = "Servicios Adicionales:";
            // 
            // NuevaCotizacionForm
            // 
            this.ClientSize = new System.Drawing.Size(782, 566);
            this.Controls.Add(this.lblTipo);
            this.Controls.Add(this.cmbTipoEdificacion);
            this.Controls.Add(this.lblMoneda);
            this.Controls.Add(this.cmbMoneda);
            this.Controls.Add(this.lblMateriales);
            this.Controls.Add(this.dgvMateriales);
            this.Controls.Add(this.btnAgregarMaterial);
            this.Controls.Add(this.lblMaquinarias);
            this.Controls.Add(this.dgvMaquinarias);
            this.Controls.Add(this.btnAgregarMaquinaria);
            this.Controls.Add(this.lblServicios);
            this.Controls.Add(this.dgvServicios);
            this.Controls.Add(this.btnAgregarServicio);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnGuardar);
            this.Name = "NuevaCotizacionForm";
            this.Text = "Nueva Cotización";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMateriales)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaquinarias)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

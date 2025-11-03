using System.Windows.Forms;

namespace WinApp
{
    partial class GestionarMonedaForm
    {
        private DataGridView dgvMoneda;
        private TextBox txtId, txtNombre, txtValor, txtSimbolo;
        private Label lblId, lblNombre, lblValor, lblSimbolo;
        private Button btnCrear, btnModificar, btnDeshabilitar;

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvMoneda = new System.Windows.Forms.DataGridView();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblValor = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnDeshabilitar = new System.Windows.Forms.Button();
            this.lblSimbolo = new System.Windows.Forms.Label();
            this.txtSimbolo = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoneda)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMoneda
            // 
            this.dgvMoneda.AllowUserToAddRows = false;
            this.dgvMoneda.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMoneda.Location = new System.Drawing.Point(20, 45);
            this.dgvMoneda.MultiSelect = false;
            this.dgvMoneda.Name = "dgvMoneda";
            this.dgvMoneda.ReadOnly = true;
            this.dgvMoneda.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMoneda.Size = new System.Drawing.Size(571, 150);
            this.dgvMoneda.TabIndex = 0;
            this.dgvMoneda.SelectionChanged += new System.EventHandler(this.dgvMoneda_SelectionChanged);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(98, 205);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(182, 20);
            this.txtId.TabIndex = 2;
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(98, 235);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(182, 20);
            this.txtNombre.TabIndex = 4;
            // 
            // txtValor
            // 
            this.txtValor.Location = new System.Drawing.Point(415, 205);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(176, 20);
            this.txtValor.TabIndex = 6;
            // 
            // lblId
            // 
            this.lblId.Location = new System.Drawing.Point(20, 210);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(72, 23);
            this.lblId.TabIndex = 1;
            this.lblId.Text = "ID:";
            // 
            // lblNombre
            // 
            this.lblNombre.Location = new System.Drawing.Point(20, 240);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(72, 23);
            this.lblNombre.TabIndex = 3;
            this.lblNombre.Text = "Nombre:";
            // 
            // lblValor
            // 
            this.lblValor.Location = new System.Drawing.Point(306, 208);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(90, 23);
            this.lblValor.TabIndex = 5;
            this.lblValor.Text = "Valor de cambio:";
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(23, 274);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(110, 23);
            this.btnCrear.TabIndex = 7;
            this.btnCrear.Text = "Crear moneda";
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(269, 274);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(87, 23);
            this.btnModificar.TabIndex = 8;
            this.btnModificar.Text = "Modificar moneda";
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnDeshabilitar
            // 
            this.btnDeshabilitar.Location = new System.Drawing.Point(491, 274);
            this.btnDeshabilitar.Name = "btnDeshabilitar";
            this.btnDeshabilitar.Size = new System.Drawing.Size(100, 23);
            this.btnDeshabilitar.TabIndex = 9;
            this.btnDeshabilitar.Text = "Deshabilitar";
            this.btnDeshabilitar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // lblSimbolo
            // 
            this.lblSimbolo.Location = new System.Drawing.Point(306, 236);
            this.lblSimbolo.Name = "lblSimbolo";
            this.lblSimbolo.Size = new System.Drawing.Size(90, 23);
            this.lblSimbolo.TabIndex = 10;
            this.lblSimbolo.Text = "Simbolo:";
            // 
            // txtSimbolo
            // 
            this.txtSimbolo.Location = new System.Drawing.Point(415, 234);
            this.txtSimbolo.Name = "txtSimbolo";
            this.txtSimbolo.Size = new System.Drawing.Size(176, 20);
            this.txtSimbolo.TabIndex = 11;
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(17, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(545, 23);
            this.lblTitle.TabIndex = 12;
            this.lblTitle.Text = "Descripción:";
            // 
            // GestionarMonedaForm
            // 
            this.ClientSize = new System.Drawing.Size(611, 310);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSimbolo);
            this.Controls.Add(this.txtSimbolo);
            this.Controls.Add(this.dgvMoneda);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblValor);
            this.Controls.Add(this.txtValor);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnDeshabilitar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GestionarMonedaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Moneda";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoneda)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label lblTitle;
    }
}

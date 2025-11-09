using System.Windows.Forms;

namespace WinApp
{
    partial class GestionarTipoEdificacionForm
    {
        private DataGridView dgvTipos;
        private TextBox txtId, txtDescripcion;
        private Label lblId, lblDescripcion;
        private Button btnCrear, btnModificar, btnDeshabilitar;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvTipos = new System.Windows.Forms.DataGridView();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnDeshabilitar = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTipos)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTipos
            // 
            this.dgvTipos.AllowUserToAddRows = false;
            this.dgvTipos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTipos.Location = new System.Drawing.Point(20, 36);
            this.dgvTipos.MultiSelect = false;
            this.dgvTipos.Name = "dgvTipos";
            this.dgvTipos.ReadOnly = true;
            this.dgvTipos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTipos.Size = new System.Drawing.Size(594, 150);
            this.dgvTipos.TabIndex = 0;
            this.dgvTipos.SelectionChanged += new System.EventHandler(this.dgvTipos_SelectionChanged);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(130, 196);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(150, 20);
            this.txtId.TabIndex = 2;
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(130, 226);
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(300, 20);
            this.txtDescripcion.TabIndex = 4;
            // 
            // lblId
            // 
            this.lblId.Location = new System.Drawing.Point(20, 201);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(100, 23);
            this.lblId.TabIndex = 1;
            this.lblId.Text = "ID:";
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.Location = new System.Drawing.Point(20, 231);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(100, 23);
            this.lblDescripcion.TabIndex = 3;
            this.lblDescripcion.Text = "Descripción:";
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(20, 276);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(75, 23);
            this.btnCrear.TabIndex = 5;
            this.btnCrear.Text = "Crear tipo";
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(281, 276);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 6;
            this.btnModificar.Text = "Modificar tipo";
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnDeshabilitar
            // 
            this.btnDeshabilitar.Location = new System.Drawing.Point(539, 276);
            this.btnDeshabilitar.Name = "btnDeshabilitar";
            this.btnDeshabilitar.Size = new System.Drawing.Size(75, 23);
            this.btnDeshabilitar.TabIndex = 7;
            this.btnDeshabilitar.Text = "Deshabilitar";
            this.btnDeshabilitar.Click += new System.EventHandler(this.btnDeshabilitar_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(17, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(597, 23);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "Descripción:";
            // 
            // GestionarTipoEdificacionForm
            // 
            this.ClientSize = new System.Drawing.Size(636, 309);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dgvTipos);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnDeshabilitar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GestionarTipoEdificacionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Tipo de Edificación";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTipos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Label lblTitle;
    }
}

using System.Windows.Forms;

namespace UI
{
    partial class GestionarMaquinariaForm
    {
        private DataGridView dgvMaquinaria;
        private TextBox txtId;
        private TextBox txtNombre;
        private TextBox txtCosto;
        private Label lblId;
        private Label lblNombre;
        private Label lblCosto;
        private Button btnCrear;
        private Button btnModificar;
        private Button btnBorrar;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvMaquinaria = new System.Windows.Forms.DataGridView();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtCosto = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblCosto = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnBorrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaquinaria)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMaquinaria
            // 
            this.dgvMaquinaria.AllowUserToAddRows = false;
            this.dgvMaquinaria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMaquinaria.Location = new System.Drawing.Point(20, 20);
            this.dgvMaquinaria.MultiSelect = false;
            this.dgvMaquinaria.Name = "dgvMaquinaria";
            this.dgvMaquinaria.ReadOnly = true;
            this.dgvMaquinaria.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaquinaria.Size = new System.Drawing.Size(344, 150);
            this.dgvMaquinaria.TabIndex = 0;
            this.dgvMaquinaria.SelectionChanged += new System.EventHandler(this.dgvMaquinaria_SelectionChanged);
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(126, 182);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(150, 20);
            this.txtId.TabIndex = 2;
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(126, 212);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(150, 20);
            this.txtNombre.TabIndex = 4;
            // 
            // txtCosto
            // 
            this.txtCosto.Location = new System.Drawing.Point(126, 239);
            this.txtCosto.Name = "txtCosto";
            this.txtCosto.Size = new System.Drawing.Size(150, 20);
            this.txtCosto.TabIndex = 6;
            // 
            // lblId
            // 
            this.lblId.Location = new System.Drawing.Point(20, 185);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(100, 23);
            this.lblId.TabIndex = 1;
            this.lblId.Text = "ID:";
            // 
            // lblNombre
            // 
            this.lblNombre.Location = new System.Drawing.Point(20, 215);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(100, 23);
            this.lblNombre.TabIndex = 3;
            this.lblNombre.Text = "Nombre:";
            // 
            // lblCosto
            // 
            this.lblCosto.Location = new System.Drawing.Point(20, 245);
            this.lblCosto.Name = "lblCosto";
            this.lblCosto.Size = new System.Drawing.Size(100, 23);
            this.lblCosto.TabIndex = 5;
            this.lblCosto.Text = "Costo/Hora:";
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(289, 181);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(75, 23);
            this.btnCrear.TabIndex = 7;
            this.btnCrear.Text = "Crear maquinaria";
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(289, 211);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 8;
            this.btnModificar.Text = "Modificar maquinaria";
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnBorrar
            // 
            this.btnBorrar.Location = new System.Drawing.Point(289, 239);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(75, 23);
            this.btnBorrar.TabIndex = 9;
            this.btnBorrar.Text = "Borrar maquinaria";
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // GestionarMaquinariaForm
            // 
            this.ClientSize = new System.Drawing.Size(382, 283);
            this.Controls.Add(this.dgvMaquinaria);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.lblNombre);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.lblCosto);
            this.Controls.Add(this.txtCosto);
            this.Controls.Add(this.btnCrear);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnBorrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GestionarMaquinariaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Maquinaria";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaquinaria)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}

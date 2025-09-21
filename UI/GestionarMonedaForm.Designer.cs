using System.Windows.Forms;

namespace UI
{
    partial class GestionarMonedaForm
    {
        private DataGridView dgvMoneda;
        private TextBox txtId, txtNombre, txtValor, txtSimbolo;
        private Label lblId, lblNombre, lblValor, lblSimbolo;
        private Button btnCrear, btnModificar, btnBorrar;

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
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.lblId = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblValor = new System.Windows.Forms.Label();
            this.btnCrear = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnBorrar = new System.Windows.Forms.Button();
            this.lblSimbolo = new System.Windows.Forms.Label();
            this.txtSimbolo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoneda)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMoneda
            // 
            this.dgvMoneda.AllowUserToAddRows = false;
            this.dgvMoneda.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMoneda.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.dgvMoneda.Location = new System.Drawing.Point(20, 20);
            this.dgvMoneda.MultiSelect = false;
            this.dgvMoneda.Name = "dgvMoneda";
            this.dgvMoneda.ReadOnly = true;
            this.dgvMoneda.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMoneda.Size = new System.Drawing.Size(460, 150);
            this.dgvMoneda.TabIndex = 0;
            this.dgvMoneda.SelectionChanged += new System.EventHandler(this.dgvMoneda_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "ID";
            this.dataGridViewTextBoxColumn1.Name = "idMoneda";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Nombre";
            this.dataGridViewTextBoxColumn2.Name = "nombreMoneda";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "valorCambio";
            this.dataGridViewTextBoxColumn3.Name = "valorCambio";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Simbolo";
            this.dataGridViewTextBoxColumn4.Name = "simbolo";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(130, 180);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(150, 23);
            this.txtId.TabIndex = 2;
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(130, 210);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(150, 23);
            this.txtNombre.TabIndex = 4;
            // 
            // txtValor
            // 
            this.txtValor.Location = new System.Drawing.Point(130, 240);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(150, 23);
            this.txtValor.TabIndex = 6;
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
            // lblValor
            // 
            this.lblValor.Location = new System.Drawing.Point(20, 245);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(100, 23);
            this.lblValor.TabIndex = 5;
            this.lblValor.Text = "Valor de cambio:";
            // 
            // btnCrear
            // 
            this.btnCrear.Location = new System.Drawing.Point(365, 181);
            this.btnCrear.Name = "btnCrear";
            this.btnCrear.Size = new System.Drawing.Size(75, 23);
            this.btnCrear.TabIndex = 7;
            this.btnCrear.Text = "Crear moneda";
            this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(365, 211);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 8;
            this.btnModificar.Text = "Modificar moneda";
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnBorrar
            // 
            this.btnBorrar.Location = new System.Drawing.Point(365, 241);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(75, 23);
            this.btnBorrar.TabIndex = 9;
            this.btnBorrar.Text = "Borrar moneda";
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // lblSimbolo
            // 
            this.lblSimbolo.Location = new System.Drawing.Point(20, 273);
            this.lblSimbolo.Name = "lblSimbolo";
            this.lblSimbolo.Size = new System.Drawing.Size(100, 23);
            this.lblSimbolo.TabIndex = 10;
            this.lblSimbolo.Text = "Simbolo:";
            // 
            // textBox1
            // 
            this.txtSimbolo.Location = new System.Drawing.Point(130, 269);
            this.txtSimbolo.Name = "textBox1";
            this.txtSimbolo.Size = new System.Drawing.Size(150, 23);
            this.txtSimbolo.TabIndex = 11;
            // 
            // GestionarMonedaForm
            // 
            this.ClientSize = new System.Drawing.Size(484, 302);
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
            this.Controls.Add(this.btnBorrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GestionarMonedaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Moneda";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoneda)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}

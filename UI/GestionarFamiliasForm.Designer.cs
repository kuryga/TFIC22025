using System.Windows.Forms;
using System;

namespace UI
{
    partial class GestionarFamiliasForm
    {
        private DataGridView dgvUsuarios, dgvDisponibles, dgvAsignadas;
        private Label lblUsuarios, lblDisponibles, lblAsignadas;
        private Button btnAgregar, btnEliminar;
        private void InitializeComponent()
        {
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvDisponibles = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvAsignadas = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblUsuarios = new System.Windows.Forms.Label();
            this.lblDisponibles = new System.Windows.Forms.Label();
            this.lblAsignadas = new System.Windows.Forms.Label();
            this.btnAgregar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.brnGuardar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponibles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignadas)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvUsuarios.Location = new System.Drawing.Point(20, 37);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(480, 100);
            this.dgvUsuarios.TabIndex = 1;
            this.dgvUsuarios.SelectionChanged += new System.EventHandler(this.dgvUsuarios_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Nombre completo";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dgvDisponibles
            // 
            this.dgvDisponibles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDisponibles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3});
            this.dgvDisponibles.Location = new System.Drawing.Point(20, 169);
            this.dgvDisponibles.Name = "dgvDisponibles";
            this.dgvDisponibles.ReadOnly = true;
            this.dgvDisponibles.Size = new System.Drawing.Size(200, 150);
            this.dgvDisponibles.TabIndex = 3;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Familia";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dgvAsignadas
            // 
            this.dgvAsignadas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAsignadas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn4});
            this.dgvAsignadas.Location = new System.Drawing.Point(300, 169);
            this.dgvAsignadas.Name = "dgvAsignadas";
            this.dgvAsignadas.ReadOnly = true;
            this.dgvAsignadas.Size = new System.Drawing.Size(200, 150);
            this.dgvAsignadas.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Familia";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // lblUsuarios
            // 
            this.lblUsuarios.Location = new System.Drawing.Point(20, 10);
            this.lblUsuarios.Name = "lblUsuarios";
            this.lblUsuarios.Size = new System.Drawing.Size(100, 23);
            this.lblUsuarios.TabIndex = 0;
            this.lblUsuarios.Text = "Usuarios:";
            // 
            // lblDisponibles
            // 
            this.lblDisponibles.Location = new System.Drawing.Point(20, 143);
            this.lblDisponibles.Name = "lblDisponibles";
            this.lblDisponibles.Size = new System.Drawing.Size(100, 23);
            this.lblDisponibles.TabIndex = 2;
            this.lblDisponibles.Text = "No asignadas:";
            // 
            // lblAsignadas
            // 
            this.lblAsignadas.Location = new System.Drawing.Point(300, 143);
            this.lblAsignadas.Name = "lblAsignadas";
            this.lblAsignadas.Size = new System.Drawing.Size(100, 23);
            this.lblAsignadas.TabIndex = 4;
            this.lblAsignadas.Text = "Asignadas:";
            // 
            // btnAgregar
            // 
            this.btnAgregar.Location = new System.Drawing.Point(235, 205);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(50, 30);
            this.btnAgregar.TabIndex = 6;
            this.btnAgregar.Text = "→";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(235, 245);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(50, 30);
            this.btnEliminar.TabIndex = 7;
            this.btnEliminar.Text = "←";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // brnGuardar
            // 
            this.brnGuardar.Location = new System.Drawing.Point(408, 325);
            this.brnGuardar.Name = "brnGuardar";
            this.brnGuardar.Size = new System.Drawing.Size(92, 30);
            this.brnGuardar.TabIndex = 8;
            this.brnGuardar.Text = "Guardar";
            this.brnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(44, 325);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 30);
            this.button1.TabIndex = 9;
            this.button1.Text = "Crear";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(214, 324);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 30);
            this.button2.TabIndex = 10;
            this.button2.Text = "Modificar";
            // 
            // GestionarFamiliasForm
            // 
            this.ClientSize = new System.Drawing.Size(524, 366);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.brnGuardar);
            this.Controls.Add(this.lblUsuarios);
            this.Controls.Add(this.dgvUsuarios);
            this.Controls.Add(this.lblDisponibles);
            this.Controls.Add(this.dgvDisponibles);
            this.Controls.Add(this.lblAsignadas);
            this.Controls.Add(this.dgvAsignadas);
            this.Controls.Add(this.btnAgregar);
            this.Controls.Add(this.btnEliminar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GestionarFamiliasForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestionar Familias";
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponibles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAsignadas)).EndInit();
            this.ResumeLayout(false);

        }

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private Button brnGuardar;
        private Button button1;
        private Button button2;
    }
}

using System.Windows.Forms;

namespace UI
{
    partial class ConsultarCotizacionesForm
    {
        private DataGridView dgvCotizaciones;
        private Button btnExportar, btnVerDetalles, btnModificar, btnBorrar;

        private void InitializeComponent()
        {
            this.dgvCotizaciones = new System.Windows.Forms.DataGridView();
            this.btnExportar = new System.Windows.Forms.Button();
            this.btnVerDetalles = new System.Windows.Forms.Button();
            this.btnModificar = new System.Windows.Forms.Button();
            this.btnBorrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCotizaciones)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCotizaciones
            // 
            this.dgvCotizaciones.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCotizaciones.Location = new System.Drawing.Point(20, 20);
            this.dgvCotizaciones.MultiSelect = false;
            this.dgvCotizaciones.Name = "dgvCotizaciones";
            this.dgvCotizaciones.ReadOnly = true;
            this.dgvCotizaciones.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCotizaciones.Size = new System.Drawing.Size(544, 200);
            this.dgvCotizaciones.TabIndex = 0;
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(20, 240);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(75, 23);
            this.btnExportar.TabIndex = 1;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // btnVerDetalles
            // 
            this.btnVerDetalles.Location = new System.Drawing.Point(20, 287);
            this.btnVerDetalles.Name = "btnVerDetalles";
            this.btnVerDetalles.Size = new System.Drawing.Size(75, 23);
            this.btnVerDetalles.TabIndex = 2;
            this.btnVerDetalles.Text = "Ver detalles";
            this.btnVerDetalles.Click += new System.EventHandler(this.btnVerDetalles_Click);
            // 
            // btnModificar
            // 
            this.btnModificar.Location = new System.Drawing.Point(489, 240);
            this.btnModificar.Name = "btnModificar";
            this.btnModificar.Size = new System.Drawing.Size(75, 23);
            this.btnModificar.TabIndex = 3;
            this.btnModificar.Text = "Modificar";
            this.btnModificar.Click += new System.EventHandler(this.btnModificar_Click);
            // 
            // btnBorrar
            // 
            this.btnBorrar.Location = new System.Drawing.Point(489, 287);
            this.btnBorrar.Name = "btnBorrar";
            this.btnBorrar.Size = new System.Drawing.Size(75, 23);
            this.btnBorrar.TabIndex = 4;
            this.btnBorrar.Text = "Borrar";
            this.btnBorrar.Click += new System.EventHandler(this.btnBorrar_Click);
            // 
            // ConsultarCotizacionesForm
            // 
            this.ClientSize = new System.Drawing.Size(592, 351);
            this.Controls.Add(this.dgvCotizaciones);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnVerDetalles);
            this.Controls.Add(this.btnModificar);
            this.Controls.Add(this.btnBorrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConsultarCotizacionesForm";
            this.Text = "Consultar Cotizaciones";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCotizaciones)).EndInit();
            this.ResumeLayout(false);

        }
    }
}

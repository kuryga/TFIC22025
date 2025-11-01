
namespace UI.AuditoriaForms
{
    partial class GenerarReporteForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUnidad = new System.Windows.Forms.Label();
            this.cboDestino = new System.Windows.Forms.ComboBox();
            this.btnCarpeta = new System.Windows.Forms.Button();
            this.chkPaginaAct = new System.Windows.Forms.CheckBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUnidad
            // 
            this.lblUnidad.Location = new System.Drawing.Point(9, 16);
            this.lblUnidad.Name = "lblUnidad";
            this.lblUnidad.Size = new System.Drawing.Size(160, 23);
            this.lblUnidad.TabIndex = 3;
            this.lblUnidad.Text = "Destino (unidad/carpeta):";
            // 
            // cboDestino
            // 
            this.cboDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDestino.Location = new System.Drawing.Point(12, 42);
            this.cboDestino.Name = "cboDestino";
            this.cboDestino.Size = new System.Drawing.Size(360, 21);
            this.cboDestino.TabIndex = 4;
            // 
            // btnCarpeta
            // 
            this.btnCarpeta.Location = new System.Drawing.Point(378, 40);
            this.btnCarpeta.Name = "btnCarpeta";
            this.btnCarpeta.Size = new System.Drawing.Size(118, 23);
            this.btnCarpeta.TabIndex = 5;
            this.btnCarpeta.Text = "Explorar...";
            // 
            // chkPaginaAct
            // 
            this.chkPaginaAct.Checked = true;
            this.chkPaginaAct.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPaginaAct.Location = new System.Drawing.Point(12, 79);
            this.chkPaginaAct.Name = "chkPaginaAct";
            this.chkPaginaAct.Size = new System.Drawing.Size(220, 24);
            this.chkPaginaAct.TabIndex = 6;
            this.chkPaginaAct.Text = "Solo Pagina Actual";
            // 
            // btnGenerar
            // 
            this.btnGenerar.Location = new System.Drawing.Point(193, 109);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(118, 23);
            this.btnGenerar.TabIndex = 7;
            this.btnGenerar.Text = "Generar reporte";
            // 
            // GenerarReporteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 142);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.chkPaginaAct);
            this.Controls.Add(this.lblUnidad);
            this.Controls.Add(this.cboDestino);
            this.Controls.Add(this.btnCarpeta);
            this.Name = "GenerarReporteForm";
            this.Text = "GenerarReporteForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblUnidad;
        private System.Windows.Forms.ComboBox cboDestino;
        private System.Windows.Forms.Button btnCarpeta;
        private System.Windows.Forms.CheckBox chkPaginaAct;
        private System.Windows.Forms.Button btnGenerar;
    }
}

namespace WinApp.AuditoriaForms
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
            this.chkPaginaAct = new System.Windows.Forms.CheckBox();
            this.btnGenerar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkPaginaAct
            // 
            this.chkPaginaAct.Checked = true;
            this.chkPaginaAct.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPaginaAct.Location = new System.Drawing.Point(12, 26);
            this.chkPaginaAct.Name = "chkPaginaAct";
            this.chkPaginaAct.Size = new System.Drawing.Size(220, 24);
            this.chkPaginaAct.TabIndex = 6;
            this.chkPaginaAct.Text = "Solo Pagina Actual";
            // 
            // btnGenerar
            // 
            this.btnGenerar.Location = new System.Drawing.Point(73, 71);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(118, 23);
            this.btnGenerar.TabIndex = 7;
            this.btnGenerar.Text = "Generar reporte";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // GenerarReporteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 111);
            this.Controls.Add(this.btnGenerar);
            this.Controls.Add(this.chkPaginaAct);
            this.Name = "GenerarReporteForm";
            this.Text = "GenerarReporteForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox chkPaginaAct;
        private System.Windows.Forms.Button btnGenerar;
    }
}
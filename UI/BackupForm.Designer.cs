// BackupForm.Designer.cs
using System.Windows.Forms;

namespace UI
{
    partial class BackupForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblUnidad;
        private ComboBox cboDestino;
        private Button btnCarpeta;
        private Label lblPartes;
        private NumericUpDown nudPartes;
        private Button btnBackup;
        private TextBox txtResultado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblUnidad = new System.Windows.Forms.Label();
            this.cboDestino = new System.Windows.Forms.ComboBox();
            this.btnCarpeta = new System.Windows.Forms.Button();
            this.lblPartes = new System.Windows.Forms.Label();
            this.nudPartes = new System.Windows.Forms.NumericUpDown();
            this.btnBackup = new System.Windows.Forms.Button();
            this.txtResultado = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudPartes)).BeginInit();
            this.SuspendLayout();
            // 
            // lblUnidad
            // 
            this.lblUnidad.Location = new System.Drawing.Point(16, 18);
            this.lblUnidad.Name = "lblUnidad";
            this.lblUnidad.Size = new System.Drawing.Size(160, 23);
            this.lblUnidad.TabIndex = 0;
            this.lblUnidad.Text = "Destino (unidad/carpeta):";
            // 
            // cboDestino
            // 
            this.cboDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDestino.Location = new System.Drawing.Point(180, 14);
            this.cboDestino.Name = "cboDestino";
            this.cboDestino.Size = new System.Drawing.Size(360, 21);
            this.cboDestino.TabIndex = 1;
            // 
            // btnCarpeta
            // 
            this.btnCarpeta.Location = new System.Drawing.Point(548, 13);
            this.btnCarpeta.Name = "btnCarpeta";
            this.btnCarpeta.Size = new System.Drawing.Size(118, 23);
            this.btnCarpeta.TabIndex = 2;
            this.btnCarpeta.Text = "Carpeta";
            // 
            // lblPartes
            // 
            this.lblPartes.Location = new System.Drawing.Point(16, 56);
            this.lblPartes.Name = "lblPartes";
            this.lblPartes.Size = new System.Drawing.Size(160, 23);
            this.lblPartes.TabIndex = 3;
            this.lblPartes.Text = "Partes del backup (1–5):";
            // 
            // nudPartes
            // 
            this.nudPartes.Location = new System.Drawing.Point(180, 52);
            this.nudPartes.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudPartes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPartes.Name = "nudPartes";
            this.nudPartes.Size = new System.Drawing.Size(80, 20);
            this.nudPartes.TabIndex = 4;
            this.nudPartes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(269, 308);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(120, 32);
            this.btnBackup.TabIndex = 5;
            this.btnBackup.Text = "Hacer Backup";
            // 
            // txtResultado
            // 
            this.txtResultado.Location = new System.Drawing.Point(12, 82);
            this.txtResultado.Multiline = true;
            this.txtResultado.Name = "txtResultado";
            this.txtResultado.ReadOnly = true;
            this.txtResultado.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResultado.Size = new System.Drawing.Size(680, 220);
            this.txtResultado.TabIndex = 6;
            // 
            // BackupForm
            // 
            this.ClientSize = new System.Drawing.Size(704, 353);
            this.Controls.Add(this.lblUnidad);
            this.Controls.Add(this.cboDestino);
            this.Controls.Add(this.btnCarpeta);
            this.Controls.Add(this.lblPartes);
            this.Controls.Add(this.nudPartes);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.txtResultado);
            this.Name = "BackupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Backup de Base de Datos";
            ((System.ComponentModel.ISupportInitialize)(this.nudPartes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

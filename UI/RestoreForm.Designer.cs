// RestoreForm.Designer.cs
using System.Windows.Forms;

namespace UI
{
    partial class RestoreForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblArchivos;
        private TextBox txtArchivos;
        private Button btnSeleccionar;
        private CheckBox chkVerify;
        private CheckBox chkReplace;
        private Button btnRestaurar;
        private TextBox txtLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblArchivos = new System.Windows.Forms.Label();
            this.txtArchivos = new System.Windows.Forms.TextBox();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.chkVerify = new System.Windows.Forms.CheckBox();
            this.chkReplace = new System.Windows.Forms.CheckBox();
            this.btnRestaurar = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblArchivos
            // 
            this.lblArchivos.Location = new System.Drawing.Point(16, 18);
            this.lblArchivos.Name = "lblArchivos";
            this.lblArchivos.Size = new System.Drawing.Size(220, 23);
            this.lblArchivos.TabIndex = 0;
            this.lblArchivos.Text = "Archivos .bak (uno o varios):";
            // 
            // txtArchivos
            // 
            this.txtArchivos.Location = new System.Drawing.Point(16, 42);
            this.txtArchivos.Name = "txtArchivos";
            this.txtArchivos.ReadOnly = true;
            this.txtArchivos.Size = new System.Drawing.Size(560, 20);
            this.txtArchivos.TabIndex = 1;
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.Location = new System.Drawing.Point(584, 40);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(112, 23);
            this.btnSeleccionar.TabIndex = 2;
            this.btnSeleccionar.Text = "Seleccionar...";
            // 
            // chkVerify
            // 
            this.chkVerify.Checked = true;
            this.chkVerify.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVerify.Location = new System.Drawing.Point(16, 82);
            this.chkVerify.Name = "chkVerify";
            this.chkVerify.Size = new System.Drawing.Size(220, 24);
            this.chkVerify.TabIndex = 3;
            this.chkVerify.Text = "Verificar antes (VERIFYONLY)";
            // 
            // chkReplace
            // 
            this.chkReplace.Location = new System.Drawing.Point(248, 82);
            this.chkReplace.Name = "chkReplace";
            this.chkReplace.Size = new System.Drawing.Size(120, 24);
            this.chkReplace.TabIndex = 4;
            this.chkReplace.Text = "REPLACE";
            // 
            // btnRestaurar
            // 
            this.btnRestaurar.Location = new System.Drawing.Point(288, 378);
            this.btnRestaurar.Name = "btnRestaurar";
            this.btnRestaurar.Size = new System.Drawing.Size(120, 32);
            this.btnRestaurar.TabIndex = 5;
            this.btnRestaurar.Text = "Restaurar";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(16, 112);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(680, 260);
            this.txtLog.TabIndex = 6;
            // 
            // RestoreForm
            // 
            this.ClientSize = new System.Drawing.Size(704, 428);
            this.Controls.Add(this.lblArchivos);
            this.Controls.Add(this.txtArchivos);
            this.Controls.Add(this.btnSeleccionar);
            this.Controls.Add(this.chkVerify);
            this.Controls.Add(this.chkReplace);
            this.Controls.Add(this.btnRestaurar);
            this.Controls.Add(this.txtLog);
            this.Name = "RestoreForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Restore de Base de Datos";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

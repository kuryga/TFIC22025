using System;
using System.Windows.Forms;

namespace UI
{
    partial class NuevaContrasenaForm
    {
        private Label lblNueva;
        private TextBox txtContra;
        private Button btnConfirmar;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblNueva = new System.Windows.Forms.Label();
            this.txtContra = new System.Windows.Forms.TextBox();
            this.btnConfirmar = new System.Windows.Forms.Button();
            this.lblConfirmar = new System.Windows.Forms.Label();
            this.txtConfirmacion = new System.Windows.Forms.TextBox();
            this.btnVerContra = new System.Windows.Forms.Button();
            this.btnVerConfirmacion = new System.Windows.Forms.Button();
            this.lblRequisitos = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNueva
            // 
            this.lblNueva.Location = new System.Drawing.Point(12, 8);
            this.lblNueva.Name = "lblNueva";
            this.lblNueva.Size = new System.Drawing.Size(195, 12);
            this.lblNueva.TabIndex = 0;
            this.lblNueva.Text = "Nueva contraseña:";
            // 
            // txtContra
            // 
            this.txtContra.Location = new System.Drawing.Point(12, 23);
            this.txtContra.Name = "txtContra";
            this.txtContra.Size = new System.Drawing.Size(227, 20);
            this.txtContra.TabIndex = 1;
            this.txtContra.UseSystemPasswordChar = true;
            // 
            // btnConfirmar
            // 
            this.btnConfirmar.Location = new System.Drawing.Point(95, 258);
            this.btnConfirmar.Name = "btnConfirmar";
            this.btnConfirmar.Size = new System.Drawing.Size(144, 26);
            this.btnConfirmar.TabIndex = 5;
            this.btnConfirmar.Text = "Confirmar";
            this.btnConfirmar.UseVisualStyleBackColor = true;
            this.btnConfirmar.Click += new System.EventHandler(this.btnConfirmar_Click);
            // 
            // lblConfirmar
            // 
            this.lblConfirmar.Location = new System.Drawing.Point(9, 58);
            this.lblConfirmar.Name = "lblConfirmar";
            this.lblConfirmar.Size = new System.Drawing.Size(195, 12);
            this.lblConfirmar.TabIndex = 5;
            this.lblConfirmar.Text = "Confirmar contraseña:";
            // 
            // txtConfirmacion
            // 
            this.txtConfirmacion.Location = new System.Drawing.Point(12, 73);
            this.txtConfirmacion.Name = "txtConfirmacion";
            this.txtConfirmacion.Size = new System.Drawing.Size(227, 20);
            this.txtConfirmacion.TabIndex = 3;
            this.txtConfirmacion.UseSystemPasswordChar = true;
            // 
            // btnVerContra
            // 
            this.btnVerContra.Location = new System.Drawing.Point(276, 23);
            this.btnVerContra.Name = "btnVerContra";
            this.btnVerContra.Size = new System.Drawing.Size(46, 20);
            this.btnVerContra.TabIndex = 2;
            this.btnVerContra.Text = "Ver";
            this.btnVerContra.UseVisualStyleBackColor = true;
            // 
            // btnVerConfirmacion
            // 
            this.btnVerConfirmacion.Location = new System.Drawing.Point(276, 73);
            this.btnVerConfirmacion.Name = "btnVerConfirmacion";
            this.btnVerConfirmacion.Size = new System.Drawing.Size(46, 20);
            this.btnVerConfirmacion.TabIndex = 4;
            this.btnVerConfirmacion.Text = "Ver";
            this.btnVerConfirmacion.UseVisualStyleBackColor = true;
            // 
            // lblRequisitos
            // 
            this.lblRequisitos.Location = new System.Drawing.Point(9, 109);
            this.lblRequisitos.Name = "lblRequisitos";
            this.lblRequisitos.Size = new System.Drawing.Size(313, 135);
            this.lblRequisitos.TabIndex = 7;
            // 
            // NuevaContrasenaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 296);
            this.Controls.Add(this.btnVerConfirmacion);
            this.Controls.Add(this.btnVerContra);
            this.Controls.Add(this.lblRequisitos);
            this.Controls.Add(this.lblConfirmar);
            this.Controls.Add(this.txtConfirmacion);
            this.Controls.Add(this.lblNueva);
            this.Controls.Add(this.txtContra);
            this.Controls.Add(this.btnConfirmar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NuevaContrasenaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ingrese la nueva contrasena";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label lblConfirmar;
        private TextBox txtConfirmacion;
        private Button btnVerContra;
        private Button btnVerConfirmacion;
        private Label lblRequisitos;
    }
}

using System;
using System.Windows.Forms;

namespace UI
{
    partial class LoginForm
    {
        private Label lblUsuario;
        private Label lblContrasena;
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnLogin;
        private ComboBox cmbIdiomaInferior;
        private Label lblIdiomaInferior;
        private Button btnRecuperarContrasena;


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblContrasena = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtContrasena = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblIdiomaInferior = new System.Windows.Forms.Label();
            this.cmbIdiomaInferior = new System.Windows.Forms.ComboBox();
            this.btnRecuperarContrasena = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUsuario
            // 
            this.lblUsuario.Location = new System.Drawing.Point(156, 37);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(155, 13);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Usuario:";
            this.lblUsuario.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblContrasena
            // 
            this.lblContrasena.Location = new System.Drawing.Point(156, 86);
            this.lblContrasena.Name = "lblContrasena";
            this.lblContrasena.Size = new System.Drawing.Size(155, 13);
            this.lblContrasena.TabIndex = 2;
            this.lblContrasena.Text = "Contraseña:";
            this.lblContrasena.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(156, 53);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(155, 20);
            this.txtUsuario.TabIndex = 1;
            // 
            // txtContrasena
            // 
            this.txtContrasena.Location = new System.Drawing.Point(156, 102);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.Size = new System.Drawing.Size(155, 20);
            this.txtContrasena.TabIndex = 3;
            this.txtContrasena.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(191, 142);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(86, 30);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Iniciar sesión";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblIdiomaInferior
            // 
            this.lblIdiomaInferior.Location = new System.Drawing.Point(9, 213);
            this.lblIdiomaInferior.Name = "lblIdiomaInferior";
            this.lblIdiomaInferior.Size = new System.Drawing.Size(100, 13);
            this.lblIdiomaInferior.TabIndex = 7;
            this.lblIdiomaInferior.Text = "Idioma:";
            this.lblIdiomaInferior.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbIdiomaInferior
            // 
            this.cmbIdiomaInferior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIdiomaInferior.Items.AddRange(new object[] {
            "Español",
            "Inglés",
            "Portugués"});
            this.cmbIdiomaInferior.Location = new System.Drawing.Point(12, 229);
            this.cmbIdiomaInferior.Name = "cmbIdiomaInferior";
            this.cmbIdiomaInferior.Size = new System.Drawing.Size(86, 21);
            this.cmbIdiomaInferior.TabIndex = 8;
            // 
            // btnRecuperarContrasena
            // 
            this.btnRecuperarContrasena.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRecuperarContrasena.Location = new System.Drawing.Point(310, 223);
            this.btnRecuperarContrasena.Name = "btnRecuperarContrasena";
            this.btnRecuperarContrasena.Size = new System.Drawing.Size(154, 30);
            this.btnRecuperarContrasena.TabIndex = 9;
            this.btnRecuperarContrasena.Text = "¿Olvidaste tu contraseña?";
            this.btnRecuperarContrasena.UseVisualStyleBackColor = true;
            this.btnRecuperarContrasena.Click += new System.EventHandler(this.btnRecuperarContrasena_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 262);
            this.Controls.Add(this.btnRecuperarContrasena);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblContrasena);
            this.Controls.Add(this.txtContrasena);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblIdiomaInferior);
            this.Controls.Add(this.cmbIdiomaInferior);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inicio de Sesión - Nombre parametrizacion Empresa S.A";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}

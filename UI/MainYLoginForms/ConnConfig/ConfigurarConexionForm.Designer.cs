using System.Windows.Forms;

namespace WinApp
{
    partial class ConfigurarConexionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblServidor = new System.Windows.Forms.Label();
            this.lblBaseDatos = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.txtBaseDatos = new System.Windows.Forms.TextBox();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkTrusted = new System.Windows.Forms.CheckBox();
            this.btnProbar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblServidor
            // 
            this.lblServidor.AutoSize = true;
            this.lblServidor.Location = new System.Drawing.Point(24, 22);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(54, 15);
            this.lblServidor.TabIndex = 0;
            this.lblServidor.Text = "Servidor:";
            // 
            // lblBaseDatos
            // 
            this.lblBaseDatos.AutoSize = true;
            this.lblBaseDatos.Location = new System.Drawing.Point(24, 58);
            this.lblBaseDatos.Name = "lblBaseDatos";
            this.lblBaseDatos.Size = new System.Drawing.Size(88, 15);
            this.lblBaseDatos.TabIndex = 1;
            this.lblBaseDatos.Text = "Base de datos:";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(24, 128);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(54, 15);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Text = "Usuario:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(24, 164);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(67, 15);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            // 
            // txtServidor
            // 
            this.txtServidor.Location = new System.Drawing.Point(132, 19);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(219, 23);
            this.txtServidor.TabIndex = 4;
            // 
            // txtBaseDatos
            // 
            this.txtBaseDatos.Location = new System.Drawing.Point(132, 55);
            this.txtBaseDatos.Name = "txtBaseDatos";
            this.txtBaseDatos.Size = new System.Drawing.Size(219, 23);
            this.txtBaseDatos.TabIndex = 5;
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(132, 125);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(219, 23);
            this.txtUsuario.TabIndex = 6;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(132, 161);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(219, 23);
            this.txtPassword.TabIndex = 7;
            // 
            // chkTrusted
            // 
            this.chkTrusted.AutoSize = true;
            this.chkTrusted.Location = new System.Drawing.Point(132, 92);
            this.chkTrusted.Name = "chkTrusted";
            this.chkTrusted.Size = new System.Drawing.Size(171, 19);
            this.chkTrusted.TabIndex = 8;
            this.chkTrusted.Text = "Usar autenticación integrada";
            this.chkTrusted.UseVisualStyleBackColor = true;
            this.chkTrusted.CheckedChanged += new System.EventHandler(this.chkTrusted_CheckedChanged);
            // 
            // btnProbar
            // 
            this.btnProbar.Location = new System.Drawing.Point(132, 205);
            this.btnProbar.Name = "btnProbar";
            this.btnProbar.Size = new System.Drawing.Size(100, 30);
            this.btnProbar.TabIndex = 9;
            this.btnProbar.Text = "Probar";
            this.btnProbar.UseVisualStyleBackColor = true;
            this.btnProbar.Click += new System.EventHandler(this.btnProbar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(251, 205);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 30);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // ConfigurarConexionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 260);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnProbar);
            this.Controls.Add(this.chkTrusted);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.txtBaseDatos);
            this.Controls.Add(this.txtServidor);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblBaseDatos);
            this.Controls.Add(this.lblServidor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurarConexionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de conexión";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblServidor;
        private Label lblBaseDatos;
        private Label lblUsuario;
        private Label lblPassword;
        private TextBox txtServidor;
        private TextBox txtBaseDatos;
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private CheckBox chkTrusted;
        private Button btnProbar;
        private Button btnGuardar;
    }
}

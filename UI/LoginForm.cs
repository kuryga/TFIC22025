using System;
using System.Windows.Forms;
using BLL.Seguridad;
using CredencialesException = BE.Seguridad.CredencialesInvalidasException;
using BloqueadoException = BE.Seguridad.UsuarioBloqueadoException;

namespace UI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            this.AcceptButton = btnLogin;

            if (txtContrasena != null)
                txtContrasena.UseSystemPasswordChar = true;

            txtUsuario?.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var ok = BLL.Seguridad.LoginBLL.GetInstance()
                    .TryLogin(txtUsuario.Text?.Trim(), txtContrasena.Text);

                if (ok)
                {
                    OpenAfterLogin(); 
                }
                else
                {
                    MostrarLoginError();
                }
            }
            catch (BloqueadoException)
            {
                MostrarLoginError();
            }
            catch (CredencialesException)
            {
                MostrarLoginError();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al intentar iniciar sesión.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
        protected virtual void OpenAfterLogin()
        {

            MessageBox.Show("sesion iniciada.", "Atención",
                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // TODO: abrir mmain
            // using (var main = new MainForm())
            // {
            //     this.Hide();
            //     main.ShowDialog();
            //     this.Close();
            // }
        }

        private void MostrarLoginError()
        {
            MessageBox.Show(
                "Usuario o contraseña incorrectos.",
                "Error de autenticación",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtContrasena.Clear();
            txtContrasena.Focus();
        }
    }
}

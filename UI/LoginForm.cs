using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Seguridad;
using CredencialesException = BE.Seguridad.CredencialesInvalidasException;
using BloqueadoException = BE.Seguridad.UsuarioBloqueadoException;

namespace UI
{
    public partial class LoginForm : Form
    {
        public event Action LoginSucceeded;

        public LoginForm()
        {
            InitializeComponent();

            this.AcceptButton = btnLogin;

            if (txtContrasena != null)
                txtContrasena.UseSystemPasswordChar = true;

            txtUsuario?.Focus();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            SetBusy(true);

            try
            {
                string usuario = txtUsuario.Text?.Trim();
                string pass = txtContrasena.Text;
                bool ok = await Task.Run(() =>
                {
                    return LoginBLL.GetInstance().TryLogin(usuario, pass);
                });

                if (ok)
                {
                    SetBusy(false);
                    OpenAfterLogin();
                }
                else
                {
                    SetBusy(false);
                    MostrarLoginError();
                }
            }
            catch (BloqueadoException)
            {
                SetBusy(false);
                MostrarLoginError();
            }
            catch (CredencialesException)
            {
                SetBusy(false);
                MostrarLoginError();
            }
            catch (Exception)
            {
                SetBusy(false);
                MessageBox.Show(
                    "Ocurrió un error al intentar iniciar sesión.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void SetBusy(bool busy)
        {
            this.UseWaitCursor = busy;        
            btnLogin.Enabled = !busy;
            txtUsuario.Enabled = !busy;
            txtContrasena.Enabled = !busy;

            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;
            Application.DoEvents();
        }

        protected virtual void OpenAfterLogin()
        {
            using (var main = new MainForm())
            {
                LoginSucceeded?.Invoke();
            }
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

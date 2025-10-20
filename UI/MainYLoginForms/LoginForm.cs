using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using BLL.Seguridad;
using CredencialesException = BE.Seguridad.CredencialesInvalidasException;
using BloqueadoException = BE.Seguridad.UsuarioBloqueadoException;
using Parametrizacion = BE.Params.Parametrizacion;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using Idioma = BE.Idioma;

namespace UI
{
    public partial class LoginForm : Form
    {
        public event Action LoginSucceeded;
        
        public LoginForm()
        {
            InitializeComponent();

            Parametrizacion param = ParametrizacionBLL.GetInstance().GetParametrizacion();

            this.Text = $"Inicio de sesion - {param.NombreEmpresa}";

            List<Idioma> idiomas = ParametrizacionBLL.GetInstance().GetIdiomas();
            cmbIdiomaInferior.DataSource = idiomas;
            cmbIdiomaInferior.DisplayMember = "nombre";  
            cmbIdiomaInferior.ValueMember = "nombre"; 
            cmbIdiomaInferior.SelectedValue = idiomas.Find(r => r.IdIdioma == param.IdIdioma).Nombre;

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

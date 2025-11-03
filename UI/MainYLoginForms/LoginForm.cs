using BLL.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;
using BloqueadoException = BE.Seguridad.UsuarioBloqueadoException;
using CredencialesException = BE.Seguridad.CredencialesInvalidasException;
using DeshabilitadoException = BE.Seguridad.UsuarioDeshabilitadoException;
using Idioma = BE.Idioma;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class LoginForm : BaseForm
    {
        public event Action LoginSucceeded;

        public LoginForm()
        {
            InitializeComponent();
            loadParametrizacion();

            this.UpdateTexts();
            List<Idioma> idiomas = ParametrizacionBLL.GetInstance().GetIdiomas();
            int idiomaSeleccionado = ParametrizacionBLL.GetInstance().GetIdIdioma();

            cmbIdiomaInferior.DataSource = idiomas;
            cmbIdiomaInferior.DisplayMember = "nombre";
            cmbIdiomaInferior.ValueMember = "nombre";
            cmbIdiomaInferior.SelectedValue = idiomas.Find(r => r.IdIdioma == idiomaSeleccionado).Nombre;

            cmbIdiomaInferior.SelectedValueChanged += CmbIdiomaInferior_SelectedValueChanged;

            this.AcceptButton = btnLogin;

            if (txtContrasena != null)
                txtContrasena.UseSystemPasswordChar = true;

            txtUsuario?.Focus();
        }

        private void loadParametrizacion()
        {
            try
            {
                ParametrizacionBLL.GetInstance().LoadParametrizacion();
            }
            catch (Exception ex)
            {
                string mensaje = ConfigurationManager.AppSettings["MensajeErrorConexion"];
                string titulo = ConfigurationManager.AppSettings["TituloErrorConexion"];

                MessageBox.Show(
                    mensaje,
                    titulo,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void CmbIdiomaInferior_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbIdiomaInferior.SelectedItem is BE.Idioma idioma)
            {
                ParametrizacionBLL.GetInstance().LoadLocalizablesForIdioma(idioma.IdIdioma);
                this.UpdateTexts();
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            SetBusy(true);

            try
            {
                string usuario = txtUsuario.Text?.Trim().ToLower();
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
            catch (DeshabilitadoException)
            {
                SetBusy(false);
                MostrarDeshabilitadoError();
            }
            catch (BloqueadoException)
            {
                SetBusy(false);
                MostrarBloqueadoError();
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
                    ParametrizacionBLL.GetInstance().GetLocalizable("login_error_message"),
                    ParametrizacionBLL.GetInstance().GetLocalizable("login_error_title"),
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
            btnRecuperarContrasena.Enabled = !busy;
            cmbIdiomaInferior.Enabled = !busy;

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
                ParametrizacionBLL.GetInstance().GetLocalizable("login_invalid_message"),
                ParametrizacionBLL.GetInstance().GetLocalizable("login_invalid_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void MostrarBloqueadoError()
        {
            MessageBox.Show(
                ParametrizacionBLL.GetInstance().GetLocalizable("login_blocked_message"),
                ParametrizacionBLL.GetInstance().GetLocalizable("login_blocked_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void MostrarDeshabilitadoError()
        {
            MessageBox.Show(
                ParametrizacionBLL.GetInstance().GetLocalizable("login_disabled_message"),
                ParametrizacionBLL.GetInstance().GetLocalizable("login_disabled_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void UpdateTexts()
        {
            txtUsuario.Tag = TextBoxTag.MailUrban;
            txtContrasena.Tag = TextBoxTag.Pwd;
            lblUsuario.Text = ParametrizacionBLL.GetInstance().GetLocalizable("login_username_label");
            lblContrasena.Text = ParametrizacionBLL.GetInstance().GetLocalizable("login_password_label");
            lblIdiomaInferior.Text = ParametrizacionBLL.GetInstance().GetLocalizable("login_language_label");
            btnLogin.Text = ParametrizacionBLL.GetInstance().GetLocalizable("login_button");
            btnRecuperarContrasena.Text = ParametrizacionBLL.GetInstance().GetLocalizable("login_forgot_password");
            string titleText = ParametrizacionBLL.GetInstance().GetLocalizable("login_title");
            string NombreEmpresa = ParametrizacionBLL.GetInstance().GetNombreEmpresa();

            this.Text = $"{titleText} - {NombreEmpresa}";
        }

        private void btnRecuperarContrasena_Click(object sender, EventArgs e)
        {
            using (var frm = new RecuperarContraForm())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
        }
    }
}

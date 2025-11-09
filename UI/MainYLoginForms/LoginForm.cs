using BLL.Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
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

        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();
        private readonly string secretPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                         "UrbanSoft", "conn.secret");

        private bool _showPass = false;

        public LoginForm()
        {
            InitializeComponent();

            if (!EnsureConnectionConfiguredAndHealthy())
            {
                Application.Exit();
                return;
            }

            LoadLogo();
            loadParametrizacion();
            this.UpdateTexts();

            List<Idioma> idiomas = param.GetIdiomas();
            int idiomaSeleccionado = param.GetIdIdioma();

            cmbIdiomaInferior.DataSource = idiomas;
            cmbIdiomaInferior.DisplayMember = "nombre";
            cmbIdiomaInferior.ValueMember = "nombre";
            cmbIdiomaInferior.SelectedValue = idiomas.Find(r => r.IdIdioma == idiomaSeleccionado).Nombre;

            cmbIdiomaInferior.SelectedValueChanged += CmbIdiomaInferior_SelectedValueChanged;

            this.AcceptButton = btnLogin;

            if (txtContrasena != null)
            {
                txtContrasena.UseSystemPasswordChar = true;
                txtContrasena.TextChanged += TxtContrasena_TextChanged;
            }

            if (btnVer != null)
                btnVer.Enabled = false; 

            txtUsuario?.Focus();
        }
        private void TxtContrasena_TextChanged(object sender, EventArgs e)
        {
            if (btnVer == null || txtContrasena == null)
                return;

            btnVer.Enabled = !string.IsNullOrWhiteSpace(txtContrasena.Text);
        }

        private bool EnsureConnectionConfiguredAndHealthy()
        {
            if (TryGetConnectionStringFromSecret(out var cs) && ProbarConexion(cs))
                return true;

            while (true)
            {
                using (var dlg = new ConexionErrorForm())
                {
                    var dr = dlg.ShowDialog(this);
                    if (dr != DialogResult.OK || !dlg.ReconfigureSelected)
                        return false;

                    using (var cfg = new ConfigurarConexionForm(dlg.SelectedLanguage))
                    {
                        var r2 = cfg.ShowDialog(this);
                        if (r2 != DialogResult.OK)
                            continue;
                    }

                    if (TryGetConnectionStringFromSecret(out cs) && ProbarConexion(cs))
                    {
                        var lang = dlg.SelectedLanguage ?? "es";
                        string tituloInfo = ConfigurationManager.AppSettings[$"TituloInfo.{lang}"] ?? "Información";
                        string msgOk = ConfigurationManager.AppSettings[$"MsgConfigActualizada.{lang}"] ?? "Configuración actualizada correctamente. Se reintentará la conexión.";
                        MessageBox.Show(msgOk, tituloInfo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
            }
        }

        private bool TryGetConnectionStringFromSecret(out string connectionString)
        {
            connectionString = null;
            try
            {
                if (!File.Exists(secretPath))
                    return false;

                string encrypted = File.ReadAllText(secretPath, Encoding.UTF8)?.Trim();
                if (string.IsNullOrWhiteSpace(encrypted))
                    return false;

                connectionString = EncriptacionBLL.GetInstance().DesencriptarReversible(encrypted);
                return !string.IsNullOrWhiteSpace(connectionString);
            }
            catch
            {
                return false;
            }
        }

        private bool ProbarConexion(string connectionString)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void LoadLogo()
        {
            try
            {
                string logoPath = ConfigurationManager.AppSettings["ReportLogoPath"];
                if (string.IsNullOrWhiteSpace(logoPath))
                    return;

                if (!System.IO.Path.IsPathRooted(logoPath))
                    logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logoPath);

                if (System.IO.File.Exists(logoPath))
                {
                    ptbLogo.SizeMode = PictureBoxSizeMode.Zoom;
                    ptbLogo.Image = System.Drawing.Image.FromFile(logoPath);
                }
            }
            catch { }
        }

        private void loadParametrizacion()
        {
            try
            {
                param.LoadParametrizacion();
            }
            catch (Exception)
            {
                string mensaje = ConfigurationManager.AppSettings["MensajeErrorConexion"]
                                 ?? "Ocurrió un error inesperado al intentar conectar con el servicio. Por favor, comuníquese con el soporte técnico de la empresa para obtener asistencia.";
                string titulo = ConfigurationManager.AppSettings["TituloErrorConexion"]
                                ?? "Error de conexión";

                var dr = MessageBox.Show(mensaje + "\n\n¿Desea reconfigurar la conexión ahora?", titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (dr == DialogResult.Yes)
                {
                    if (EnsureConnectionConfiguredAndHealthy())
                    {
                        try
                        {
                            param.LoadParametrizacion();
                        }
                        catch
                        {
                            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void CmbIdiomaInferior_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbIdiomaInferior.SelectedItem is BE.Idioma idioma)
            {
                param.LoadLocalizablesForIdioma(idioma.IdIdioma);
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
                bool ok = await Task.Run(() => LoginBLL.GetInstance().TryLogin(usuario, pass));

                SetBusy(false);

                if (ok)
                    OpenAfterLogin();
                else
                    MostrarLoginError();
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
                MessageBox.Show(param.GetLocalizable("login_error_message"),
                                param.GetLocalizable("login_error_title"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
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
            btnVer.Enabled = !busy;
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
            MessageBox.Show(param.GetLocalizable("login_invalid_message"),
                            param.GetLocalizable("login_invalid_title"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void MostrarBloqueadoError()
        {
            MessageBox.Show(param.GetLocalizable("login_blocked_message"),
                            param.GetLocalizable("login_blocked_title"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void MostrarDeshabilitadoError()
        {
            MessageBox.Show(param.GetLocalizable("login_disabled_message"),
                            param.GetLocalizable("login_disabled_title"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            txtContrasena.Clear();
            txtContrasena.Focus();
        }

        private void UpdateTexts()
        {
            txtUsuario.Tag = TextBoxTag.MailUrban;
            txtContrasena.Tag = TextBoxTag.Pwd;

            lblUsuario.Text = param.GetLocalizable("login_username_label");
            lblContrasena.Text = param.GetLocalizable("login_password_label");
            lblIdiomaInferior.Text = param.GetLocalizable("login_language_label");
            btnLogin.Text = param.GetLocalizable("login_button");
            btnRecuperarContrasena.Text = param.GetLocalizable("login_forgot_password");

            string titleText = param.GetLocalizable("login_title");
            string nombreEmpresa = param.GetNombreEmpresa();
            this.Text = $"{titleText} - {nombreEmpresa}";

            btnVer.Text = param.GetLocalizable(_showPass ? "password_hide_button" : "password_show_button");

            SetHelpContext(param.GetLocalizable("login_help_title"),
                           param.GetLocalizable("login_help_body"));
        }

        private void btnRecuperarContrasena_Click(object sender, EventArgs e)
        {
            using (var frm = new RecuperarContraForm())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.ShowDialog(this);
            }
        }

        private void btnVer_Click(object sender, EventArgs e)
        {
            _showPass = !_showPass;
            if (txtContrasena != null)
                txtContrasena.UseSystemPasswordChar = !_showPass;
        }
    }
}

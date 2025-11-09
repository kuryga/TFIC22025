using BLL.Seguridad;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WinApp
{
    public partial class ConfigurarConexionForm : BaseForm
    {
        private readonly string secretPath;
        private readonly string _lang;

        public ConfigurarConexionForm(string lang)
        {
            InitializeComponent();

            secretPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "UrbanSoft", "conn.secret");

            _lang = string.IsNullOrWhiteSpace(lang) ? "es" : lang;

            UpdateTexts();

            txtServidor.Text = ".";
            txtBaseDatos.Text = "UrbanSoft";
            chkTrusted.Checked = true;
            ActualizarEstadoCampos();
        }

        private void UpdateTexts()
        {
            this.Text = T("Cfg_Title");
            lblServidor.Text = T("Cfg_LabelServidor");
            lblBaseDatos.Text = T("Cfg_LabelBaseDatos");
            lblUsuario.Text = T("Cfg_LabelUsuario");
            lblPassword.Text = T("Cfg_LabelPassword");
            chkTrusted.Text = T("Cfg_CheckTrusted");
            btnProbar.Text = T("Cfg_BtnProbar");
            btnGuardar.Text = T("Cfg_BtnGuardar");

            SetHelpContext(T("Cfg_HelpTitle"), T("Cfg_HelpBody"));
        }

        private string T(string baseKey)
        {
            try
            {
                var v = ConfigurationManager.AppSettings[$"{baseKey}.{_lang}"];
                if (!string.IsNullOrWhiteSpace(v))
                    return v;

                v = ConfigurationManager.AppSettings[baseKey];
                return string.IsNullOrWhiteSpace(v) ? baseKey : v;
            }
            catch
            {
                return baseKey;
            }
        }

        private void btnProbar_Click(object sender, EventArgs e)
        {
            try
            {
                string cs = ConstruirConnectionString();
                using (var conn = new SqlConnection(cs))
                {
                    conn.Open();
                    MessageBox.Show(T("Cfg_ProbarOk"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(T("Cfg_ProbarErrorPrefix") + " " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string cs = ConstruirConnectionString();
                string encrypted = EncriptacionBLL.GetInstance().EncriptarReversible(cs);

                Directory.CreateDirectory(Path.GetDirectoryName(secretPath));
                File.WriteAllText(secretPath, encrypted, Encoding.UTF8);

                MessageBox.Show(T("Cfg_GuardarOkPrefix") + "\n\n" + secretPath,
                                T("Cfg_GuardarOkTitle"),
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(T("Cfg_GuardarErrorPrefix") + " " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkTrusted_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoCampos();
        }

        private void ActualizarEstadoCampos()
        {
            bool enabled = !chkTrusted.Checked;
            txtUsuario.Enabled = enabled;
            txtPassword.Enabled = enabled;
        }

        private string ConstruirConnectionString()
        {
            if (chkTrusted.Checked)
                return $"Server={txtServidor.Text.Trim()};Database={txtBaseDatos.Text.Trim()};Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
            else
                return $"Server={txtServidor.Text.Trim()};Database={txtBaseDatos.Text.Trim()};User ID={txtUsuario.Text.Trim()};Password={txtPassword.Text};Encrypt=True;TrustServerCertificate=True;";
        }
    }
}

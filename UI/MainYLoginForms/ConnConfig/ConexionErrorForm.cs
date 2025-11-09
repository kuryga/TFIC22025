using System;
using System.Configuration;
using System.Windows.Forms;

namespace WinApp
{
    public partial class ConexionErrorForm : Form
    {
        public string SelectedLanguage { get; private set; } = "es";
        public bool ReconfigureSelected { get; private set; }

        public ConexionErrorForm()
        {
            InitializeComponent();
            cmbLang.Items.Clear();
            cmbLang.Items.Add(new LangItem("es", "Español (ES)"));
            cmbLang.Items.Add(new LangItem("en", "English (EN)"));
            cmbLang.SelectedIndex = 0;
            ApplyTexts();
        }

        private void ApplyTexts()
        {
            string lang = ((LangItem)cmbLang.SelectedItem).Code;
            SelectedLanguage = lang;

            this.Text = T("TituloErrorConexion", lang);
            lblMessage.Text = T("MsgConexionNoEstablecida", lang);
            lblIdioma.Text = T("LblIdioma", lang);
            btnConfigurar.Text = T("BtnConfigurar", lang);
            btnCancelar.Text = T("BtnCancelar", lang);
        }

        private static string T(string baseKey, string lang)
        {
            var v = ConfigurationManager.AppSettings[$"{baseKey}.{lang}"];
            if (!string.IsNullOrWhiteSpace(v)) return v;
            v = ConfigurationManager.AppSettings[baseKey];
            return string.IsNullOrWhiteSpace(v) ? baseKey : v;
        }

        private void cmbLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyTexts();
        }

        private void btnConfigurar_Click(object sender, EventArgs e)
        {
            ReconfigureSelected = true;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ReconfigureSelected = false;

            string msg = T("MsgCerrarPorFaltaConexion", SelectedLanguage);
            string title = T("TituloCerrarApp", SelectedLanguage);

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Exit();
        }

        private sealed class LangItem
        {
            public string Code { get; }
            public string Display { get; }
            public LangItem(string code, string display)
            {
                Code = code;
                Display = display;
            }
            public override string ToString() => Display;
        }
    }
}

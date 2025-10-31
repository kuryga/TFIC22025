using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class NuevaContrasenaForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        private bool _showNueva = false;
        private bool _showConfirm = false;

        public NuevaContrasenaForm()
        {
            InitializeComponent();

            this.Load += NuevaContrasenaForm_Load;

            if (btnVerContra != null) btnVerContra.Click += BtnVerNueva_Click;
            if (btnVerConfirmacion != null) btnVerConfirmacion.Click += BtnVerConfirmar_Click;
            if (btnConfirmar != null) btnConfirmar.Click += btnConfirmar_Click;

            if (btnConfirmar != null) this.AcceptButton = btnConfirmar;
        }

        private void NuevaContrasenaForm_Load(object sender, EventArgs e)
        {
            if (txtContra != null) txtContra.UseSystemPasswordChar = true;
            if (txtConfirmacion != null) txtConfirmacion.UseSystemPasswordChar = true;

            UpdateTexts();

            lblRequisitos.AutoSize = false;
            txtContra?.Focus();
        }

        private void BtnVerNueva_Click(object sender, EventArgs e)
        {
            _showNueva = !_showNueva;
            if (txtContra != null) txtContra.UseSystemPasswordChar = !_showNueva;

            if (btnVerContra != null)
                btnVerContra.Text = param.GetLocalizable(_showNueva ? "password_hide_button" : "password_show_button");
        }

        private void BtnVerConfirmar_Click(object sender, EventArgs e)
        {
            _showConfirm = !_showConfirm;
            if (txtConfirmacion != null) txtConfirmacion.UseSystemPasswordChar = !_showConfirm;

            if (btnVerConfirmacion != null)
                btnVerConfirmacion.Text = param.GetLocalizable(_showConfirm ? "password_hide_button" : "password_show_button");
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            var p1 = txtContra?.Text ?? string.Empty;
            var p2 = txtConfirmacion?.Text ?? string.Empty;

            if (!string.Equals(p1, p2, StringComparison.Ordinal))
            {
                MessageBox.Show(
                    param.GetLocalizable("password_requirements_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmacion?.Focus();
                return;
            }

            var ok = ValidatePasswordRules(p1);
            if (!ok)
            {
                MessageBox.Show(
                    param.GetLocalizable("password_requirements_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtContra?.Focus();
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private static bool ValidatePasswordRules(string pass)
        {
            if (string.IsNullOrEmpty(pass)) return false;
            if (pass.Length < 8) return false;
            if (pass.Contains("\r") || pass.Contains("\n")) return false;

            if (!Regex.IsMatch(pass, "[A-Z]")) return false;

            int digits = 0; foreach (char c in pass) if (char.IsDigit(c)) digits++;
            if (digits < 2) return false;

            var symbolSet = @"!@#$^&?_+<>.:";
            bool hasSymbol = false;
            foreach (var ch in pass) if (symbolSet.IndexOf(ch) >= 0) { hasSymbol = true; break; }
            if (!hasSymbol) return false;

            return true;
        }

        private void UpdateTexts()
        {
            txtContra.Tag = "";
            this.Text = param.GetLocalizable("reset_password_title");
            lblNueva.Text = param.GetLocalizable("reset_password_new_label");
            lblConfirmar.Text = param.GetLocalizable("reset_password_confirm_label");
            lblRequisitos.Text = param.GetLocalizable("password_requirements_message");
            btnVerContra.Text = param.GetLocalizable("password_show_button");
            btnVerConfirmacion.Text = param.GetLocalizable("password_show_button");
            btnConfirmar.Text = param.GetLocalizable("reset_password_confirm_button");
        }
    }
}

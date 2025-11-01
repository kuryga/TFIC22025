using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using UsuarioBLL = BLL.Seguridad.UsuarioBLL;

namespace UI
{
    public partial class NuevaContrasenaForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        private bool _showNueva = false;
        private bool _showConfirm = false;

        private readonly int _idUsuario;
        private readonly string _correoUsuario;

        public NuevaContrasenaForm(int idUsuario, string correo)
        {
            InitializeComponent();

            _idUsuario = idUsuario;
            _correoUsuario = correo;

            this.Load += NuevaContrasenaForm_Load;

            btnVerContra.Click += BtnVerNueva_Click;
            btnVerConfirmacion.Click += BtnVerConfirmar_Click;

            this.AcceptButton = btnConfirmar;
        }

        private void NuevaContrasenaForm_Load(object sender, EventArgs e)
        {
            txtContra.UseSystemPasswordChar = true;
            txtConfirmacion.UseSystemPasswordChar = true;

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
                    param.GetLocalizable("password_mismatch_message") ?? param.GetLocalizable("password_requirements_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmacion?.Focus();
                return;
            }

            var ok = InputSanitizer.IsValidNewPassword(p2);
            if (!ok)
            {
                MessageBox.Show(
                    param.GetLocalizable("password_requirements_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtContra?.Focus();
                return;
            }

            ToggleBusy(true);
            try
            {
                UsuarioBLL.GetInstance().CambiarContrasenaConToken(_idUsuario, p2);

                MessageBox.Show(
                    param.GetLocalizable("reset_password_success_message"),
                    param.GetLocalizable("ok_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    (param.GetLocalizable("unexpected_error_prefix") + " " + ex.Message),
                    param.GetLocalizable("warning_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("unexpected_error_prefix") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void UpdateTexts()
        {
            txtContra.Tag = "VERIFY_PASS";
            this.Text = param.GetLocalizable("reset_password_title");
            lblNueva.Text = param.GetLocalizable("reset_password_new_label");
            lblConfirmar.Text = param.GetLocalizable("reset_password_confirm_label");
            lblRequisitos.Text = param.GetLocalizable("password_requirements_message");
            btnVerContra.Text = param.GetLocalizable("password_show_button");
            btnVerConfirmacion.Text = param.GetLocalizable("password_show_button");
            btnConfirmar.Text = param.GetLocalizable("reset_password_confirm_button");
        }

        private void ToggleBusy(bool busy)
        {
            UseWaitCursor = busy;
            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;

            btnConfirmar.Enabled = !busy;
            btnVerContra.Enabled = !busy;
            btnVerConfirmacion.Enabled = !busy;
            txtContra.Enabled = !busy;
            txtConfirmacion.Enabled = !busy;

            Application.DoEvents();
        }
    }
}

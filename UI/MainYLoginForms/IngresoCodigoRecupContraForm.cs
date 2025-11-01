using System;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using UsuarioBLL = BLL.Seguridad.UsuarioBLL;

namespace UI
{
    public partial class IngresoCodigoRecupContraForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public string CodigoIngresado => txtCodigo.Text.Trim().ToUpperInvariant();

        private readonly string correoUsuario;

        public IngresoCodigoRecupContraForm(string correo = null)
        {
            InitializeComponent();

            correoUsuario = correo;

            txtCodigo.KeyPress += TxtCodigo_KeyPress;
            txtCodigo.CharacterCasing = CharacterCasing.Upper;
            btnVerificar.Click += BtnVerificar_Click;

            this.Load += IngresoCodigoRecupContraForm_Load;
        }

        private void IngresoCodigoRecupContraForm_Load(object sender, EventArgs e)
        {
            UpdateTexts();
            txtCodigo.Focus();
        }

        private void TxtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }

            if (!char.IsControl(e.KeyChar) && txtCodigo.Text.Length >= 6)
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void BtnVerificar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Length != 6)
            {
                MessageBox.Show(
                    param.GetLocalizable("security_code_required_message"),
                    param.GetLocalizable("notice_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int idUsuario = UsuarioBLL.GetInstance()
                                          .VerificarCodigoRecuperacion(correoUsuario, CodigoIngresado);

                MessageBox.Show(
                    param.GetLocalizable("security_code_valid_message"),
                    param.GetLocalizable("ok_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                using (var formNueva = new NuevaContrasenaForm(idUsuario, correoUsuario))
                {
                    formNueva.ShowDialog(this);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                string mensaje = ex.Message.ToLowerInvariant().Contains("expirado")
                    ? param.GetLocalizable("security_code_expired_message")
                    : param.GetLocalizable("security_code_invalid_message");

                MessageBox.Show(
                    mensaje,
                    param.GetLocalizable("warning_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("unexpected_error_prefix") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("security_verification_title");
            lblTitulo.Text = param.GetLocalizable("security_code_label");
            btnVerificar.Text = param.GetLocalizable("security_verify_button");
        }
    }
}

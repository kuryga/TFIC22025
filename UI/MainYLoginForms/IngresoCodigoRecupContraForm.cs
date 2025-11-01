using System;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class IngresoCodigoRecupContraForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public string CodigoIngresado => txtCodigo.Text.Trim().ToUpperInvariant();

        public IngresoCodigoRecupContraForm()
        {
            InitializeComponent();

            txtCodigo.KeyPress += TxtCodigo_KeyPress;
            txtCodigo.CharacterCasing = CharacterCasing.Upper;

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

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (txtCodigo.Text.Length != 6)
            {
                MessageBox.Show(
                    param.GetLocalizable("security_code_required_message"),
                    param.GetLocalizable("notice_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // validación 

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("security_verification_title");
            lblTitulo.Text = param.GetLocalizable("security_code_label");
            btnVerificar.Text = param.GetLocalizable("security_verify_button");
        }
    }
}

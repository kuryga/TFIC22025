using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using BLL.Seguridad;

namespace UI
{
    public partial class RecuperarContraForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public RecuperarContraForm()
        {
            InitializeComponent();

            btnEnviar.Click += BtnEnviar_Click;
            this.AcceptButton = (IButtonControl)this.btnEnviar;

            UpdateTexts();
        }

        private void UpdateTexts()
        {
            this.txtEmail.Tag = "MAIL_URBANSOFT";

            this.Text = param.GetLocalizable("recover_title");

            lblInstruccion.Text = param.GetLocalizable("recover_email_label");

            btnEnviar.Text = param.GetLocalizable("recover_submit_button");
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {
            var txt = this.Controls.ContainsKey("txtEmail") ? this.Controls["txtEmail"] as TextBox : null;
            var email = txt?.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show(
                    param.GetLocalizable("recover_email_required_message"),
                    param.GetLocalizable("warning_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt?.Focus();
                return;
            }


            ToggleBusy(true);

            try
            {
                // Llama a tu BLL (ajusta el método si tu firma es distinta)
               // await Task.Run(() => LoginBLL.GetInstance().StartPasswordRecovery(email));

                MessageBox.Show(
                    param.GetLocalizable("recover_email_sent_message"),
                    param.GetLocalizable("ok_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("recover_email_error_prefix_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void ToggleBusy(bool busy)
        {
            UseWaitCursor = busy;
            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;

            if (this.Controls.ContainsKey("btnEnviar"))
                this.Controls["btnEnviar"].Enabled = !busy;

            if (this.Controls.ContainsKey("txtEmail"))
                this.Controls["txtEmail"].Enabled = !busy;

            Application.DoEvents();
        }
    }
}

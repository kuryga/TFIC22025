using System;
using System.Globalization;
using System.Windows.Forms;
using MonedaBLL = BLL.Genericos.MonedaBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class CrearMonedaForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public CrearMonedaForm()
        {
            InitializeComponent();
            HookEventos();
            UpdateTexts();

            if (txtNombre != null) txtNombre.Tag = TextBoxTag.SqlSafe;
            if (txtSimbolo != null) txtSimbolo.Tag = TextBoxTag.SqlSafe;
            if (txtValor != null) txtValor.Tag = TextBoxTag.Price;

            this.AcceptButton = btnCrear;
        }

        private void HookEventos()
        {
            if (btnCrear != null) btnCrear.Click += btnCrear_Click;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("moneda_create_title");

            if (lblTitle != null)
                lblTitle.Text = param.GetLocalizable("moneda_create_header");

            if (lblNombre != null)
                lblNombre.Text = param.GetLocalizable("moneda_name_label");

            if (lblSimbolo != null)
                lblSimbolo.Text = param.GetLocalizable("moneda_symbol_label");

            if (lblValor != null)
                lblValor.Text = param.GetLocalizable("moneda_rate_label");

            if (btnCrear != null)
                btnCrear.Text = param.GetLocalizable("create_button");

            // Ayuda contextual
            string helpTitle = param.GetLocalizable("moneda_create_help_title");
            string helpBody = param.GetLocalizable("moneda_create_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private static bool TryParseDecimalAny(string input, out decimal value)
        {
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                return true;
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                var nombre = (txtNombre?.Text ?? string.Empty).Trim();
                var simbolo = (txtSimbolo?.Text ?? string.Empty).Trim();
                var valorTx = (txtValor?.Text ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_create_required_name_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre?.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(simbolo))
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_create_required_symbol_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSimbolo?.Focus();
                    return;
                }

                decimal tasa;
                if (!TryParseDecimalAny(valorTx, out tasa))
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_rate_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtValor?.Focus();
                    return;
                }

                tasa = Math.Round(tasa, 2, MidpointRounding.AwayFromZero);

                var nuevo = new BE.Moneda
                {
                    NombreMoneda = nombre,
                    Simbolo = simbolo,
                    ValorCambio = tasa,
                    Deshabilitado = false
                };

                bool ok = MonedaBLL.GetInstance().Create(nuevo);
                if (ok)
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_create_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show(
                    param.GetLocalizable("moneda_create_error_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("moneda_create_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

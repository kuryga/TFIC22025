using System;
using System.Globalization;
using System.Windows.Forms;
using MaquinariaBLL = BLL.Genericos.MaquinariaBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class CrearMaquinariaForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public CrearMaquinariaForm()
        {
            InitializeComponent();
            HookEventos();
            UpdateTexts();

            // Tags según convención
            if (txtNombre != null) txtNombre.Tag = TextBoxTag.SqlSafe;
            if (txtCosto != null) txtCosto.Tag = TextBoxTag.Price;

            this.AcceptButton = btnCrear;
        }

        private void HookEventos()
        {
            if (btnCrear != null) btnCrear.Click += btnCrear_Click;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("maquinaria_create_title");

            if (lblTitle != null) lblTitle.Text = param.GetLocalizable("maquinaria_create_header");
            if (lblNombre != null) lblNombre.Text = param.GetLocalizable("maquinaria_name_label");
            if (lblCosto != null) lblCosto.Text = param.GetLocalizable("maquinaria_cost_label");
            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("create_button");

            // Ayuda contextual
            string helpTitle = param.GetLocalizable("maquinaria_create_help_title");
            string helpBody = param.GetLocalizable("maquinaria_create_help_body");
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
                var costoTx = (txtCosto?.Text ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_create_required_name_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre?.Focus();
                    return;
                }

                decimal costo;
                if (!TryParseDecimalAny(costoTx, out costo))
                {
                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_cost_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtCosto?.Focus();
                    return;
                }

                costo = Math.Round(costo, 2, MidpointRounding.AwayFromZero);

                var nuevo = new BE.Maquinaria
                {
                    Nombre = nombre,
                    CostoPorHora = costo,
                    Deshabilitado = false
                };

                bool ok = MaquinariaBLL.GetInstance().Create(nuevo);
                if (ok)
                {
                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_create_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show(
                    param.GetLocalizable("maquinaria_create_error_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("maquinaria_create_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

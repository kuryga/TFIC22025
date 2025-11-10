using System;
using System.Globalization;
using System.Windows.Forms;
using MaterialBLL = BLL.Genericos.MaterialBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class CrearMaterialForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public CrearMaterialForm()
        {
            InitializeComponent();
            HookEventos();
            UpdateTexts();

            if (txtNombre != null) txtNombre.Tag = TextBoxTag.SqlSafe;
            if (txtUnidad != null) txtUnidad.Tag = TextBoxTag.SqlSafe;
            if (txtPrecio != null) txtPrecio.Tag = TextBoxTag.Price;
            if (txtUso != null) txtUso.Tag = TextBoxTag.Price;

            this.AcceptButton = btnCrear;
        }

        private void HookEventos()
        {
            if (btnCrear != null) btnCrear.Click += btnCrear_Click;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("material_create_title");

            if (lblTitle != null)
                lblTitle.Text = param.GetLocalizable("material_create_header");

            if (lblNombre != null)
                lblNombre.Text = param.GetLocalizable("material_name_label");

            if (lblUnidad != null)
                lblUnidad.Text = param.GetLocalizable("material_unit_label");

            if (lblPrecio != null)
                lblPrecio.Text = param.GetLocalizable("material_price_label");

            if (lblUso != null)
                lblUso.Text = param.GetLocalizable("material_usage_label");

            if (btnCrear != null)
                btnCrear.Text = param.GetLocalizable("create_button");

            string helpTitle = param.GetLocalizable("material_create_help_title");
            string helpBody = param.GetLocalizable("material_create_help_body");
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
                var unidad = (txtUnidad?.Text ?? string.Empty).Trim();
                var precioTx = (txtPrecio?.Text ?? string.Empty).Trim();
                var usoTx = (txtUso?.Text ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_create_required_name_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtNombre?.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(unidad))
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_create_required_unit_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUnidad?.Focus();
                    return;
                }

                decimal precio;
                if (!TryParseDecimalAny(precioTx, out precio))
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_price_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPrecio?.Focus();
                    return;
                }

                decimal uso;
                if (!TryParseDecimalAny(usoTx, out uso))
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_usage_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUso?.Focus();
                    return;
                }

                precio = Math.Round(precio, 2, MidpointRounding.AwayFromZero);
                uso = Math.Round(uso, 4, MidpointRounding.AwayFromZero);

                var nuevo = new BE.Material
                {
                    Nombre = nombre,
                    UnidadMedida = unidad,
                    PrecioUnidad = precio,
                    UsoPorM2 = uso,
                    Deshabilitado = false
                };

                bool ok = MaterialBLL.GetInstance().Create(nuevo);
                if (ok)
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_create_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show(
                    param.GetLocalizable("material_create_error_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("material_create_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using System;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using ServicioAdicionalBLL = BLL.Genericos.ServicioAdicionalBLL;

namespace WinApp
{
    public partial class CrearServicioAdicionalForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public CrearServicioAdicionalForm()
        {
            InitializeComponent();
            HookEventos();
            UpdateTexts();

            if (txtDescripcion != null) txtDescripcion.Tag = TextBoxTag.SqlSafe;
            if (txtPrecio != null) txtPrecio.Tag = TextBoxTag.Price;

            this.AcceptButton = btnCrear;
        }

        private void HookEventos()
        {
            if (btnCrear != null) btnCrear.Click += btnCrear_Click;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("servicio_create_title");

            if (lblTitle != null)
                lblTitle.Text = param.GetLocalizable("servicio_create_header");

            if (lblDescripcion != null)
                lblDescripcion.Text = param.GetLocalizable("servicio_description_label");

            if (lblPrecio != null)
                lblPrecio.Text = param.GetLocalizable("servicio_price_label");

            if (btnCrear != null)
                btnCrear.Text = param.GetLocalizable("create_button");

            string helpTitle = param.GetLocalizable("servicio_create_help_title");
            string helpBody = param.GetLocalizable("servicio_create_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                var desc = (txtDescripcion?.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(desc))
                {
                    MessageBox.Show(
                        param.GetLocalizable("servicio_create_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDescripcion?.Focus();
                    return;
                }

                decimal precio;
                if (!decimal.TryParse(txtPrecio?.Text, out precio))
                {
                    MessageBox.Show(
                        param.GetLocalizable("servicio_price_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPrecio?.Focus();
                    return;
                }

                precio = Math.Round(precio, 2, MidpointRounding.AwayFromZero);

                var nuevo = new BE.ServicioAdicional
                {
                    Descripcion = desc,
                    Precio = precio,
                    Deshabilitado = false
                };

                bool ok = ServicioAdicionalBLL.GetInstance().Create(nuevo);
                if (ok)
                {
                    MessageBox.Show(
                        param.GetLocalizable("servicio_create_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show(
                    param.GetLocalizable("servicio_create_error_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("servicio_create_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

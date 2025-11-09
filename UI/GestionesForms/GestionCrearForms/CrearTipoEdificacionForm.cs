using System;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using TipoEdificacionBLL = BLL.Genericos.TipoEdificacionBLL;

namespace WinApp
{
    public partial class CrearTipoEdificacionForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public CrearTipoEdificacionForm()
        {
            InitializeComponent();
            HookEventos();
            UpdateTexts();

            if (txtDescripcion != null)
                txtDescripcion.Tag = TextBoxTag.SqlSafe;

            this.AcceptButton = btnCrear;
        }

        private void HookEventos()
        {
            if (btnCrear != null) btnCrear.Click += btnCrear_Click;
        }

        private void UpdateTexts()
        {

            this.Text = param.GetLocalizable("tipoedif_create_title");

            if (lblTitle != null)
                lblTitle.Text = param.GetLocalizable("tipoedif_create_header");

            if (lblDescripcion != null)
                lblDescripcion.Text = param.GetLocalizable("tipoedif_description_label");

            if (btnCrear != null)
                btnCrear.Text = param.GetLocalizable("create_button");

            string helpTitle = param.GetLocalizable("tipoedif_create_help_title");
            string helpBody = param.GetLocalizable("tipoedif_create_help_body");
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
                        param.GetLocalizable("tipoedif_create_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDescripcion?.Focus();
                    return;
                }

                var nuevo = new BE.TipoEdificacion
                {
                    Descripcion = desc,
                    Deshabilitado = false
                };

                bool ok = TipoEdificacionBLL.GetInstance().Create(nuevo);
                if (ok)
                {
                    MessageBox.Show(
                        param.GetLocalizable("tipoedif_create_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    return;
                }

                MessageBox.Show(
                    param.GetLocalizable("tipoedif_create_error_message"),
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("tipoedif_create_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

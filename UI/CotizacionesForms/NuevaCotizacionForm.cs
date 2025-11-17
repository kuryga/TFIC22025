using System;
using System.Linq;
using System.Windows.Forms;

using BLL.Genericos;
using BE;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class NuevaCotizacionForm : BaseForm
    {
        private readonly CotizacionBLL _bll = CotizacionBLL.GetInstance();
        private readonly ParametrizacionBLL _param = ParametrizacionBLL.GetInstance();

        public NuevaCotizacionForm()
        {
            InitializeComponent();

            this.Text = _param.GetLocalizable("cotizacion_new_title");

            string helpTitle = _param.GetLocalizable("cotizacion_new_help_title");
            string helpBody = _param.GetLocalizable("cotizacion_new_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private Cotizacion ConstruirCotizacionDesdeVista()
        {
            var ctz = new Cotizacion();

            ctz.FechaCreacion = DateTime.Now;

            ctz.TipoEdificacion = new TipoEdificacion
            {
                IdTipoEdificacion = (cmbTipoEdificacion.SelectedItem as dynamic).Id
            };

            ctz.Moneda = new Moneda
            {
                IdMoneda = (cmbMoneda.SelectedItem as dynamic).Id
            };

            ctz.ListaMateriales = dgvMateriales.Rows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Select(r => new MaterialCotizacion
                {
                    Cantidad = Convert.ToDecimal(r.Cells["Cantidad"].Value ?? 0),
                    Material = new Material
                    {
                        Nombre = r.Cells["Nombre"].Value?.ToString(),
                        UnidadMedida = r.Cells["Unidad"].Value?.ToString(),
                        PrecioUnidad = Convert.ToDecimal(r.Cells["PrecioUnidad"].Value ?? 0),
                        UsoPorM2 = Convert.ToDecimal(r.Cells["UsoPorM2"].Value ?? 0)
                    }
                })
                .ToList();

            ctz.ListaMaquinaria = dgvMaquinarias.Rows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Select(r => new MaquinariaCotizacion
                {
                    HorasUso = Convert.ToDecimal(r.Cells["HorasUso"].Value ?? 0),
                    Maquinaria = new Maquinaria
                    {
                        Nombre = r.Cells["Nombre"].Value?.ToString(),
                        CostoPorHora = Convert.ToDecimal(r.Cells["CostoPorHora"].Value ?? 0)
                    }
                })
                .ToList();

            ctz.ListaServicios = dgvServicios.Rows
                .OfType<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Select(r => new ServicioCotizacion
                {
                    Servicio = new ServicioAdicional
                    {
                        Descripcion = r.Cells["Descripcion"].Value?.ToString(),
                        Precio = Convert.ToDecimal(r.Cells["Precio"].Value ?? 0)
                    }
                })
                .ToList();

            return ctz;
        }

        private void ActualizarTotal()
        {
            var ctz = ConstruirCotizacionDesdeVista();
            decimal total = _bll.CalcularTotal(ctz);

            string totalLabel = _param.GetLocalizable("quotation_total_label") ?? "Costo total: {0}";
            lblTotal.Text = string.Format(totalLabel, total.ToString("N2"));
        }

        private void btnAgregarMaterial_Click(object sender, EventArgs e)
        {
            var form = new AgregarMaterialesForm();
            form.ShowDialog(this);
            ActualizarTotal();
        }

        private void btnAgregarMaquinaria_Click(object sender, EventArgs e)
        {
            var form = new AgregarMaquinariasForm();
            form.ShowDialog(this);
            ActualizarTotal();
        }

        private void btnAgregarServicio_Click(object sender, EventArgs e)
        {
            var form = new AgregarServiciosAdicionalesForm();
            form.ShowDialog(this);
            ActualizarTotal();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbTipoEdificacion.SelectedIndex == -1 || cmbMoneda.SelectedIndex == -1)
            {
                MessageBox.Show(
                    _param.GetLocalizable("quotation_required_fields_message"),
                    _param.GetLocalizable("warning_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            var ctz = ConstruirCotizacionDesdeVista();

            try
            {
                int idNuevo = _bll.CrearCotizacion(ctz);

                string successMsgTemplate =
                    _param.GetLocalizable("quotation_save_success_message")
                    ?? "Cotización guardada correctamente. ID: {0}";

                string successMsg = string.Format(successMsgTemplate, idNuevo);

                MessageBox.Show(
                    successMsg,
                    _param.GetLocalizable("ok_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                string errorPrefix =
                    _param.GetLocalizable("quotation_save_error_message_prefix")
                    ?? "Error al guardar la cotización: ";

                MessageBox.Show(
                    errorPrefix + ex.Message,
                    _param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

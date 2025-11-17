using System;
using System.Diagnostics;
using System.Windows.Forms;

using CotizacionBLL = BLL.Genericos.CotizacionBLL;

namespace WinApp
{
    public partial class ConsultarCotizacionesForm : BaseForm
    {
        public ConsultarCotizacionesForm()
        {
            InitializeComponent();
            CargarDatos();

            var param = BLL.Genericos.ParametrizacionBLL.GetInstance();
            string helpTitle = param.GetLocalizable("cotizacion_list_help_title");
            string helpBody = param.GetLocalizable("cotizacion_list_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private void CargarDatos()
        {
            var datos = CotizacionBLL.GetInstance().GetListaCotizaciones();

            dgvCotizaciones.AutoGenerateColumns = false;
            dgvCotizaciones.Columns.Clear();

            dgvCotizaciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "IdCotizacion",
                DataPropertyName = "IdCotizacion",
                Name = "colId"
            });

            dgvCotizaciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "FechaCreacion",
                DataPropertyName = "FechaCreacion",
                Name = "colFecha",
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvCotizaciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "TipoEdificacion",
                DataPropertyName = "TipoDescripcion",
                Name = "colTipo"
            });
            dgvCotizaciones.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Moneda",
                DataPropertyName = "MonedaNombre",
                Name = "colMoneda"
            });

            dgvCotizaciones.DataSource = datos;
        }

        private void btnVerDetalles_Click(object sender, EventArgs e)
        {
            if (dgvCotizaciones.CurrentRow != null)
            {
                var ctz = dgvCotizaciones.CurrentRow.DataBoundItem as BE.Cotizacion;
                if (ctz == null) return;

                int id = ctz.IdCotizacion;
                using (var form = new DetalleCotizacionForm(id))
                {
                    form.ShowDialog(this);
                }
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgvCotizaciones.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione una cotización de la lista.",
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var ctzResumen = dgvCotizaciones.CurrentRow.DataBoundItem as BE.Cotizacion;
            if (ctzResumen == null) return;

            int id = ctzResumen.IdCotizacion;

            var dr = MessageBox.Show(
                $"¿Seguro que querés exportar la cotización {id}?",
                "Confirmar exportar",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (dr != DialogResult.OK) return;

            BE.Cotizacion ctzCompleta;
            try
            {
                ctzCompleta = CotizacionBLL.GetInstance().GetCotizacionCompleta(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al obtener la cotización: " + ex.Message,
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (ctzCompleta == null)
            {
                MessageBox.Show(
                    "No se encontró la cotización seleccionada.",
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var param = BLL.Genericos.ParametrizacionBLL.GetInstance();
            string empresa = param.GetLocalizable("company_name") ?? AppDomain.CurrentDomain.FriendlyName;
            byte[] logoBytes = null;

            string rutaPdf;
            try
            {
                rutaPdf = Utils.Reporting.FilePDFExporter.GenerarPdfCotizacion(ctzCompleta, empresa, logoBytes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar el PDF: " + ex.Message,
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(rutaPdf))
            {
                MessageBox.Show(
                    "No se pudo generar el PDF.",
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(rutaPdf);
            }
            catch
            {
                MessageBox.Show(
                    "PDF generado en: " + rutaPdf,
                    "Exportar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvCotizaciones.CurrentRow != null)
            {
                var ctz = dgvCotizaciones.CurrentRow.DataBoundItem as BE.Cotizacion;
                if (ctz == null) return;

                int id = ctz.IdCotizacion;
                using (var form = new DetalleCotizacionForm(id))
                {
                    form.ShowDialog(this);
                }
            }
        }
    }
}

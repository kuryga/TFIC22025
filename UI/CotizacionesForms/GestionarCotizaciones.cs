using System;
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
            MessageBox.Show(
                    "¿Seguro que querés exportar la cotizacion 101001011?",
                    "Confirmar exportar",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar cotización (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                    "¿Seguro que querés borrar la cotizacion 101001011?",
                    "Confirmar Borrar",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );
        }
    }
}

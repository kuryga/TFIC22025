using System;
using System.Windows.Forms;

namespace UI
{
    public partial class DetalleCotizacionForm : Form
    {
        private string cotizacionId;

        public DetalleCotizacionForm(string id)
        {
            cotizacionId = id;
            InitializeComponent();
            CargarDetallesMock();
        }

        private void CargarDetallesMock()
        {
            txtId.Text = cotizacionId;
            cmbTipo.SelectedIndex = 0;
            cmbMoneda.SelectedIndex = 1;

            dgvMateriales.Rows.Add("Cemento", "kg", "5.00", "3.0");
            dgvMaquinaria.Rows.Add("Excavadora", "120");
            dgvServicios.Rows.Add("Instalación", "30");
        }
    }
}

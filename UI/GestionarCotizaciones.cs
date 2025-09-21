using System;
using System.Windows.Forms;

namespace UI
{
    public partial class ConsultarCotizacionesForm : Form
    {
        public ConsultarCotizacionesForm()
        {
            InitializeComponent();
            CargarMock();
        }

        private void CargarMock()
        {
            dgvCotizaciones.Rows.Clear();
            dgvCotizaciones.Rows.Add("10101001", "2024-05-01");
            dgvCotizaciones.Rows.Add("20202002", "2024-06-03");
        }

        private void btnVerDetalles_Click(object sender, EventArgs e)
        {
            if (dgvCotizaciones.CurrentRow != null)
            {
                string id = dgvCotizaciones.CurrentRow.Cells[0].Value.ToString();
                using (var form = new DetalleCotizacionForm(id))
                {
                    form.ShowDialog();
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

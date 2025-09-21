using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarServicioAdicionalForm : Form
    {
        public GestionarServicioAdicionalForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvServicios.Rows.Clear();
            dgvServicios.Rows.Add("1", "Instalación de vallado regulatorio", "30000.00");
            dgvServicios.Rows.Add("2", "Transporte de materiales", "15000.00");
        }

        private void dgvServicios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow != null && dgvServicios.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvServicios.CurrentRow.Cells["idServicio"].Value?.ToString() ?? "";
                txtDescripcion.Text = dgvServicios.CurrentRow.Cells["descripcion"].Value?.ToString() ?? "";
                txtPrecio.Text = dgvServicios.CurrentRow.Cells["precio"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear servicio adicional (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar servicio adicional (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar servicio adicional (simulado)");
        }
    }
}

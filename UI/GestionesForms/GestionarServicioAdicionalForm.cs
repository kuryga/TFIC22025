using System;
using System.Windows.Forms;

using ServicioAdicionalBLL = BLL.Genericos.ServicioAdicionalBLL;

namespace UI
{
    public partial class GestionarServicioAdicionalForm : BaseForm
    {
        public GestionarServicioAdicionalForm()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgvServicios.DataSource = null;

            dgvServicios.DataSource = ServicioAdicionalBLL.GetInstance().GetAll();
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

using System;
using System.Windows.Forms;

using MaterialBLL = BLL.Genericos.MaterialBLL;

namespace UI
{
    public partial class GestionarMaterialForm : BaseForm
    {
        public GestionarMaterialForm()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgvMaterial.DataSource = null;

            dgvMaterial.DataSource = MaterialBLL.GetInstance().GetAll();
        }

        private void dgvMaterial_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaterial.CurrentRow != null && dgvMaterial.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvMaterial.CurrentRow.Cells["idMaterial"].Value?.ToString() ?? "";
                txtNombre.Text = dgvMaterial.CurrentRow.Cells["nombre"].Value?.ToString() ?? "";
                txtUnidad.Text = dgvMaterial.CurrentRow.Cells["unidadMedida"].Value?.ToString() ?? "";
                txtPrecio.Text = dgvMaterial.CurrentRow.Cells["precioUnidad"].Value?.ToString() ?? "";
                txtUso.Text = dgvMaterial.CurrentRow.Cells["usoPorM2"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear material (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar material (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar material (simulado)");
        }
    }
}

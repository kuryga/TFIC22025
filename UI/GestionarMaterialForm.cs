using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarMaterialForm : Form
    {
        public GestionarMaterialForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvMaterial.Rows.Clear();

            dgvMaterial.Rows.Add("1", "Cemento", "kg", "5.00", "3.5");
            dgvMaterial.Rows.Add("2", "Arena", "m³", "20.00", "0.6");
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

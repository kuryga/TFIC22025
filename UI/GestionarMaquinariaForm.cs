using System;
using System.Windows.Forms;

using MaquinariaBLL = BLL.Genericos.MaquinariaBLL;

namespace UI
{
    public partial class GestionarMaquinariaForm : Form
    {
        public GestionarMaquinariaForm()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgvMaquinaria.DataSource = null;

            dgvMaquinaria.DataSource = MaquinariaBLL.GetInstance().GetAll();
        }

        private void dgvMaquinaria_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaquinaria.CurrentRow != null && dgvMaquinaria.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvMaquinaria.CurrentRow.Cells["idMaquinaria"].Value?.ToString() ?? "";
                txtNombre.Text = dgvMaquinaria.CurrentRow.Cells["nombre"].Value?.ToString() ?? "";
                txtCosto.Text = $"{dgvMaquinaria.CurrentRow.Cells["costoPorHora"].Value?.ToString() ?? ""}$";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear maquinaria (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar maquinaria (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar maquinaria (simulado)");
        }

        private void GestionarMaquinariaForm_Load(object sender, EventArgs e)
        {

        }
    }
}

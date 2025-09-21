using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarMaquinariaForm : Form
    {
        public GestionarMaquinariaForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvMaquinaria.Rows.Clear();

            dgvMaquinaria.Rows.Add("1", "Excavadora", "120");
            dgvMaquinaria.Rows.Add("2", "Camión Volcador", "95");
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

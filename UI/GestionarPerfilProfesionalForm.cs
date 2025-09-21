using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarPerfilProfesionalForm : Form
    {
        public GestionarPerfilProfesionalForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvPerfiles.Rows.Clear();
            dgvPerfiles.Rows.Add("1", "Arquitecto", "150");
            dgvPerfiles.Rows.Add("2", "Ingeniero Civil", "180");
        }

        private void dgvPerfiles_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPerfiles.CurrentRow != null && dgvPerfiles.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvPerfiles.CurrentRow.Cells["idPerfil"].Value?.ToString() ?? "";
                txtDescripcion.Text = dgvPerfiles.CurrentRow.Cells["descripcion"].Value?.ToString() ?? "";
                txtCosto.Text = dgvPerfiles.CurrentRow.Cells["costoPorDia"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear perfil profesional (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar perfil profesional (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar perfil profesional (simulado)");
        }
    }
}

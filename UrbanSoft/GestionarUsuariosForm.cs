using System;
using System.Windows.Forms;

namespace UrbanSoft
{
    public partial class GestionarUsuariosForm : Form
    {
        public GestionarUsuariosForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvUsuarios.Rows.Clear();
            dgvUsuarios.Rows.Add("1", "Ana", "Pérez", "ana@email.com", "1512345678", "Calle sarasa y saranga", "12345678");
            dgvUsuarios.Rows.Add("2", "Carlos", "Ramírez", "carlos@email.com", "1198765432", "Av. Sarlingui 1500", "87654321");
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow != null && dgvUsuarios.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvUsuarios.CurrentRow.Cells["idUsuario"].Value?.ToString() ?? "";
                txtNombre.Text = dgvUsuarios.CurrentRow.Cells["nombreUsuario"].Value?.ToString() ?? "";
                txtApellido.Text = dgvUsuarios.CurrentRow.Cells["apellidoUsuario"].Value?.ToString() ?? "";
                txtCorreo.Text = dgvUsuarios.CurrentRow.Cells["correoElectronico"].Value?.ToString() ?? "";
                txtTelefono.Text = dgvUsuarios.CurrentRow.Cells["telefonoContacto"].Value?.ToString() ?? "";
                txtDireccion.Text = dgvUsuarios.CurrentRow.Cells["direccionUsuario"].Value?.ToString() ?? "";
                txtDocumento.Text = dgvUsuarios.CurrentRow.Cells["numeroDocumento"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear usuario (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar usuario (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar usuario (simulado)");
        }
    }
}

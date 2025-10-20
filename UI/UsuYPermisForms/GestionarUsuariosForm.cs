using System;
using System.Windows.Forms;

using UsuarioBLL = BLL.Seguridad.UsuarioBLL;

namespace UI
{
    public partial class GestionarUsuariosForm : Form
    {
        public GestionarUsuariosForm()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgvUsuarios.DataSource = null;

            dgvUsuarios.DataSource = UsuarioBLL.GetInstance().GetAll();
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

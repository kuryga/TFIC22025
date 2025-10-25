using System;
using System.Linq;
using System.Windows.Forms;

using UsuarioBLL = BLL.Seguridad.UsuarioBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class GestionarUsuariosForm : BaseForm
    {
        public GestionarUsuariosForm()
        {
            InitializeComponent();

            UpdateTexts();
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
            try
            {
                using (var form = new GestionarUsuarioForm())
                {
                    var dr = form.ShowDialog(this);
                    if (dr == DialogResult.OK)
                    {
                        CargarDatos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error abriendo formulario de alta: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.Index < 0)
                {
                    MessageBox.Show("Seleccione un usuario de la lista.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!int.TryParse(txtId.Text, out int id) || id <= 0)
                {
                    MessageBox.Show("Seleccione un usuario válido.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var row = dgvUsuarios.CurrentRow;
                string origNombre = row.Cells["nombreUsuario"].Value?.ToString()?.Trim() ?? "";
                string origApellido = row.Cells["apellidoUsuario"].Value?.ToString()?.Trim() ?? "";
                string origTelefono = row.Cells["telefonoContacto"].Value?.ToString()?.Trim() ?? "";
                string origDireccion = row.Cells["direccionUsuario"].Value?.ToString()?.Trim() ?? "";

                string newNombre = txtNombre.Text.Trim();
                string newApellido = txtApellido.Text.Trim();
                string newTelefono = txtTelefono.Text.Trim();
                string newDireccion = txtDireccion.Text.Trim();

                bool hayCambios =
                    !string.Equals(origNombre, newNombre, StringComparison.Ordinal) ||
                    !string.Equals(origApellido, newApellido, StringComparison.Ordinal) ||
                    !string.Equals(origTelefono, newTelefono, StringComparison.Ordinal) ||
                    !string.Equals(origDireccion, newDireccion, StringComparison.Ordinal);

                if (!hayCambios)
                {
                    MessageBox.Show("No hay cambios para guardar.", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var faltantes = new[]
                {
                    (Campo:"Nombre", Valor:newNombre),
                    (Campo:"Apellido", Valor:newApellido),
                    (Campo:"Teléfono", Valor:newTelefono),
                    (Campo:"Dirección", Valor:newDireccion)
                }
                .Where(x => string.IsNullOrWhiteSpace(x.Valor))
                .Select(x => x.Campo)
                .ToList();

                if (faltantes.Any())
                {
                    MessageBox.Show("Complete los campos obligatorios: " + string.Join(", ", faltantes), "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var u = new BE.Usuario
                {
                    IdUsuario = id,
                    NombreUsuario = newNombre,
                    ApellidoUsuario = newApellido,
                    CorreoElectronico = txtCorreo.Text.Trim(),
                    TelefonoContacto = newTelefono,
                    DireccionUsuario = newDireccion,
                    NumeroDocumento = txtDocumento.Text.Trim()
                };

                UsuarioBLL.GetInstance().Update(u);

                CargarDatos();

                foreach (DataGridViewRow r in dgvUsuarios.Rows)
                {
                    if (r.Cells["idUsuario"].Value is int rid && rid == id)
                    {
                        r.Selected = true;
                        dgvUsuarios.CurrentCell = r.Cells[0];
                        break;
                    }
                }

                MessageBox.Show("Usuario modificado correctamente.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error modificando usuario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar usuario (simulado)");
        }

        private void UpdateTexts()
        {
            txtTelefono.Tag = "AR_PHONE";
            txtCorreo.Tag = "MAIL_URBANSOFT";
            txtDocumento.Tag = "NUM_12";
            lblId.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_id_label");
            lblNombre.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_firstname_label");
            lblApellido.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_lastname_label");
            lblDocumento.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_document_label");
            lblCorreo.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_email_label");
            lblTelefono.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_phone_label");
            lblDireccion.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_address_label");

            btnCrear.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_create_button");
            btnModificar.Text = ParametrizacionBLL.GetInstance().GetLocalizable("user_modify_button");

            string titleText = ParametrizacionBLL.GetInstance().GetLocalizable("something_title");
            string NombreEmpresa = ParametrizacionBLL.GetInstance().GetNombreEmpresa();

            this.Text = $"{titleText} - {NombreEmpresa}";
            //TODO: esto no anda ^^^
        }
    }
}

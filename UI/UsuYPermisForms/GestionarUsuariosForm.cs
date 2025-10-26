using System;
using System.Linq;
using System.Windows.Forms;

using UsuarioBLL = BLL.Seguridad.UsuarioBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class GestionarUsuariosForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        // Estructura para mantener la línea base del usuario seleccionado
        private class Baseline
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = "";
            public string Apellido { get; set; } = "";
            public string Telefono { get; set; } = "";
            public string Direccion { get; set; } = "";
            public string Correo { get; set; } = "";
            public string Documento { get; set; } = "";
            public bool Bloqueado { get; set; }
            public bool Deshabilitado { get; set; }
        }

        private Baseline _baseline = new Baseline();

        public GestionarUsuariosForm()
        {
            InitializeComponent();

            txtNombre.TextChanged += InputsChanged;
            txtApellido.TextChanged += InputsChanged;
            txtTelefono.TextChanged += InputsChanged;
            txtDireccion.TextChanged += InputsChanged;
            txtCorreo.TextChanged += InputsChanged;
            txtDocumento.TextChanged += InputsChanged;

            dgvUsuarios.CellValueChanged += DgvUsuarios_CellValueChanged;
            dgvUsuarios.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvUsuarios.IsCurrentCellDirty)
                    dgvUsuarios.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            UpdateTexts();
            CargarDatos();

            btnModificar.Enabled = false;
        }

        private void CargarDatos()
        {
            var data = UsuarioBLL.GetInstance().GetAll();

            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.Columns.Clear();

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colId",
                DataPropertyName = "IdUsuario",
                HeaderText = param.GetLocalizable("user_id_label"),
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                DataPropertyName = "NombreUsuario",
                HeaderText = param.GetLocalizable("user_firstname_label"),
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colApellido",
                DataPropertyName = "ApellidoUsuario",
                HeaderText = param.GetLocalizable("user_lastname_label"),
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCorreo",
                DataPropertyName = "CorreoElectronico",
                HeaderText = param.GetLocalizable("user_email_label"),
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvUsuarios.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colBloqueado",
                DataPropertyName = "Bloqueado",
                HeaderText = param.GetLocalizable("user_blocked_label"),
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });
            dgvUsuarios.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colDeshabilitado",
                DataPropertyName = "Deshabilitado",
                HeaderText = param.GetLocalizable("user_disabled_label"),
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });

            dgvUsuarios.DataSource = data;
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            var row = dgvUsuarios.CurrentRow;
            if (row == null || row.Index < 0) return;

            var u = row.DataBoundItem as BE.Usuario;
            if (u == null) return;

            txtId.Text = u.IdUsuario.ToString();
            txtNombre.Text = u.NombreUsuario ?? "";
            txtApellido.Text = u.ApellidoUsuario ?? "";
            txtCorreo.Text = u.CorreoElectronico ?? "";
            txtTelefono.Text = u.TelefonoContacto ?? "";
            txtDireccion.Text = u.DireccionUsuario ?? "";
            txtDocumento.Text = u.NumeroDocumento ?? "";

            SetBaselineFrom(u);
            btnModificar.Enabled = false;
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
                MessageBox.Show(
                    param.GetLocalizable("user_open_create_error_message") + ex.Message,
                    param.GetLocalizable("user_modify_error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.Index < 0)
                {
                    MessageBox.Show(
                        param.GetLocalizable("user_select_from_list_message"),
                        param.GetLocalizable("user_select_from_list_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!int.TryParse(txtId.Text, out int id) || id <= 0)
                    return;

                var uOrig = dgvUsuarios.CurrentRow.DataBoundItem as BE.Usuario;
                if (uOrig == null) return;

                string newNombre = (txtNombre.Text ?? "").Trim();
                string newApellido = (txtApellido.Text ?? "").Trim();
                string newTelefono = (txtTelefono.Text ?? "").Trim();
                string newDireccion = (txtDireccion.Text ?? "").Trim();
                string newCorreo = (txtCorreo.Text ?? "").Trim();
                string newDocumento = (txtDocumento.Text ?? "").Trim();

                bool newBloqueado = dgvUsuarios.CurrentRow.Cells["colBloqueado"].Value is bool b1 && b1;
                bool newDeshabilitado = dgvUsuarios.CurrentRow.Cells["colDeshabilitado"].Value is bool b2 && b2;

                bool hayCambios =
                    !string.Equals(uOrig.NombreUsuario ?? "", newNombre, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.ApellidoUsuario ?? "", newApellido, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.TelefonoContacto ?? "", newTelefono, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.DireccionUsuario ?? "", newDireccion, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.CorreoElectronico ?? "", newCorreo, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.NumeroDocumento ?? "", newDocumento, StringComparison.Ordinal) ||
                    newBloqueado != uOrig.Bloqueado ||
                    newDeshabilitado != uOrig.Deshabilitado;

                if (!hayCambios)
                {
                    MessageBox.Show(
                        param.GetLocalizable("user_no_changes_message"),
                        param.GetLocalizable("user_no_changes_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnModificar.Enabled = false;
                    return;
                }

                var faltantes = new[]
                {
                    (Campo: param.GetLocalizable("user_firstname_label"), Valor: newNombre),
                    (Campo: param.GetLocalizable("user_lastname_label"),  Valor: newApellido),
                    (Campo: param.GetLocalizable("user_phone_label"),     Valor: newTelefono),
                    (Campo: param.GetLocalizable("user_address_label"),   Valor: newDireccion)
                }
                .Where(x => string.IsNullOrWhiteSpace(x.Valor))
                .Select(x => x.Campo)
                .ToList();

                if (faltantes.Any())
                {
                    MessageBox.Show(
                        param.GetLocalizable("user_required_fields_message") + string.Join(", ", faltantes),
                        param.GetLocalizable("user_required_fields_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    return;
                }

                var u = new BE.Usuario
                {
                    IdUsuario = id,
                    NombreUsuario = newNombre,
                    ApellidoUsuario = newApellido,
                    CorreoElectronico = newCorreo,
                    TelefonoContacto = newTelefono,
                    DireccionUsuario = newDireccion,
                    NumeroDocumento = newDocumento,
                    Bloqueado = newBloqueado,
                    Deshabilitado = newDeshabilitado
                };

                UsuarioBLL.GetInstance().Update(u);
                CargarDatos();

                foreach (DataGridViewRow r in dgvUsuarios.Rows)
                {
                    if (r.DataBoundItem is BE.Usuario uRow && uRow.IdUsuario == id)
                    {
                        r.Selected = true;
                        dgvUsuarios.CurrentCell = r.Cells[0];
                        SetBaselineFrom(uRow);
                        break;
                    }
                }

                btnModificar.Enabled = false;

                MessageBox.Show(
                    param.GetLocalizable("user_modified_success"),
                    param.GetLocalizable("user_modified_success_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("user_modify_error_message") + ex.Message,
                    param.GetLocalizable("user_modify_error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTexts()
        {
            txtTelefono.Tag = "AR_PHONE";
            txtDocumento.Tag = "NUM_12";
            txtCorreo.Tag = "MAIL_URBANSOFT";
            txtDireccion.Tag = "SAFE";
            txtNombre.Tag = "SAFE";
            txtApellido.Tag = "SAFE";

            lblId.Text = param.GetLocalizable("user_id_label");
            lblNombre.Text = param.GetLocalizable("user_firstname_label");
            lblApellido.Text = param.GetLocalizable("user_lastname_label");
            lblDocumento.Text = param.GetLocalizable("user_document_label");
            lblCorreo.Text = param.GetLocalizable("user_email_label");
            lblTelefono.Text = param.GetLocalizable("user_phone_label");
            lblDireccion.Text = param.GetLocalizable("user_address_label");

            btnCrear.Text = param.GetLocalizable("user_create_button");
            btnModificar.Text = param.GetLocalizable("user_modify_button");

            string titleText = param.GetLocalizable("something_title");
            string NombreEmpresa = param.GetNombreEmpresa();
            this.Text = $"{titleText} - {NombreEmpresa}";

            if (dgvUsuarios.Columns.Count > 0)
            {
                dgvUsuarios.Columns["colId"].HeaderText = param.GetLocalizable("user_id_label");
                dgvUsuarios.Columns["colNombre"].HeaderText = param.GetLocalizable("user_firstname_label");
                dgvUsuarios.Columns["colApellido"].HeaderText = param.GetLocalizable("user_lastname_label");
                dgvUsuarios.Columns["colCorreo"].HeaderText = param.GetLocalizable("user_email_label");
                dgvUsuarios.Columns["colBloqueado"].HeaderText = param.GetLocalizable("user_blocked_label");
                dgvUsuarios.Columns["colDeshabilitado"].HeaderText = param.GetLocalizable("user_disabled_label");
            }
        }

        private void InputsChanged(object sender, EventArgs e)
        {
            UpdateModifyButton();
        }

        private void DgvUsuarios_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var colName = dgvUsuarios.Columns[e.ColumnIndex].Name;
                if (colName == "colBloqueado" || colName == "colDeshabilitado")
                    btnModificar.Enabled = true;
            }
        }

        private void SetBaselineFrom(BE.Usuario u)
        {
            _baseline = new Baseline
            {
                Id = u.IdUsuario,
                Nombre = u.NombreUsuario ?? "",
                Apellido = u.ApellidoUsuario ?? "",
                Telefono = u.TelefonoContacto ?? "",
                Direccion = u.DireccionUsuario ?? "",
                Correo = u.CorreoElectronico ?? "",
                Documento = u.NumeroDocumento ?? "",
                Bloqueado = u.Bloqueado,
                Deshabilitado = u.Deshabilitado
            };
        }

        private bool HasChanges()
        {
            string T(string s) => (s ?? "").Trim();

            if (!string.Equals(T(txtNombre.Text), T(_baseline.Nombre), StringComparison.Ordinal)) return true;
            if (!string.Equals(T(txtApellido.Text), T(_baseline.Apellido), StringComparison.Ordinal)) return true;
            if (!string.Equals(T(txtTelefono.Text), T(_baseline.Telefono), StringComparison.Ordinal)) return true;
            if (!string.Equals(T(txtDireccion.Text), T(_baseline.Direccion), StringComparison.Ordinal)) return true;
            if (!string.Equals(T(txtCorreo.Text), T(_baseline.Correo), StringComparison.Ordinal)) return true;
            if (!string.Equals(T(txtDocumento.Text), T(_baseline.Documento), StringComparison.Ordinal)) return true;

            return false;
        }

        private void UpdateModifyButton()
        {
            btnModificar.Enabled = HasChanges();
        }
    }
}

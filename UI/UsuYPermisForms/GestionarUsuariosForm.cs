using System;
using System.Linq;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using UsuarioBLL = BLL.Seguridad.UsuarioBLL;

namespace WinApp
{
    public partial class GestionarUsuariosForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        private class Baseline
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = "";
            public string Apellido { get; set; } = "";
            public string Telefono { get; set; } = "";
            public string Direccion { get; set; } = "";
            public string Correo { get; set; } = "";
            public string Documento { get; set; } = "";
        }

        private Baseline _baseline = new Baseline();

        public GestionarUsuariosForm()
        {
            InitializeComponent();

            txtNombre.TextChanged -= InputsChanged; txtNombre.TextChanged += InputsChanged;
            txtApellido.TextChanged -= InputsChanged; txtApellido.TextChanged += InputsChanged;
            txtTelefono.TextChanged -= InputsChanged; txtTelefono.TextChanged += InputsChanged;
            txtDireccion.TextChanged -= InputsChanged; txtDireccion.TextChanged += InputsChanged;
            txtCorreo.TextChanged -= InputsChanged; txtCorreo.TextChanged += InputsChanged;
            txtDocumento.TextChanged -= InputsChanged; txtDocumento.TextChanged += InputsChanged;

            btnBloquear.Click -= btnBloquear_Click; btnBloquear.Click += btnBloquear_Click;
            btnDeshabilitar.Click -= btnDeshabilitar_Click; btnDeshabilitar.Click += btnDeshabilitar_Click;
            btnModificar.Click -= btnModificar_Click; btnModificar.Click += btnModificar_Click;
            btnCrear.Click -= btnCrear_Click; btnCrear.Click += btnCrear_Click;

            dgvUsuarios.SelectionChanged -= dgvUsuarios_SelectionChanged;
            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;

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
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            });
            dgvUsuarios.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colDeshabilitado",
                DataPropertyName = "Deshabilitado",
                HeaderText = param.GetLocalizable("user_disabled_label"),
                ReadOnly = true,
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
            UpdateActionButtonsText(u);
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

                if (!int.TryParse(txtId.Text, out int id) || id <= 0) return;

                var uOrig = dgvUsuarios.CurrentRow.DataBoundItem as BE.Usuario;
                if (uOrig == null) return;

                string newNombre = (txtNombre.Text ?? "").Trim();
                string newApellido = (txtApellido.Text ?? "").Trim();
                string newTelefono = (txtTelefono.Text ?? "").Trim();
                string newDireccion = (txtDireccion.Text ?? "").Trim();
                string newCorreo = (txtCorreo.Text ?? "").Trim();
                string newDocumento = (txtDocumento.Text ?? "").Trim();

                bool hayCambios =
                    !string.Equals(uOrig.NombreUsuario ?? "", newNombre, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.ApellidoUsuario ?? "", newApellido, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.TelefonoContacto ?? "", newTelefono, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.DireccionUsuario ?? "", newDireccion, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.CorreoElectronico ?? "", newCorreo, StringComparison.Ordinal) ||
                    !string.Equals(uOrig.NumeroDocumento ?? "", newDocumento, StringComparison.Ordinal);

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
                    (param.GetLocalizable("user_firstname_label"), newNombre),
                    (param.GetLocalizable("user_lastname_label"),  newApellido),
                    (param.GetLocalizable("user_phone_label"),     newTelefono),
                    (param.GetLocalizable("user_address_label"),   newDireccion)
                }
                .Where(x => string.IsNullOrWhiteSpace(x.Item2))
                .Select(x => x.Item1)
                .ToList();

                if (faltantes.Any())
                {
                    MessageBox.Show(
                        param.GetLocalizable("user_required_fields_message") + string.Join(", ", faltantes),
                        param.GetLocalizable("user_required_fields_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    Bloqueado = uOrig.Bloqueado,
                    Deshabilitado = uOrig.Deshabilitado
                };

                UsuarioBLL.GetInstance().Update(u);
                CargarDatos();
                ReselectAndSync(id);
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

        private void btnBloquear_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.Index < 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("user_select_from_list_message"),
                    param.GetLocalizable("user_select_from_list_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var u = dgvUsuarios.CurrentRow.DataBoundItem as BE.Usuario;
            if (u == null) return;

            bool nuevoEstado = !u.Bloqueado;
            string msg = nuevoEstado ? param.GetLocalizable("user_block_confirm_message")
                                     : param.GetLocalizable("user_unblock_confirm_message");

            if (!ShowConfirm(msg, param.GetLocalizable("confirm_title")))
                return;

            bool cambioRealizado = false;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnBloquear.Enabled = false;
                u.Bloqueado = nuevoEstado;

                cambioRealizado = UsuarioBLL.GetInstance().Update(u);

                if (cambioRealizado)
                {
                    CargarDatos();
                    ReselectAndSync(u.IdUsuario);

                    MessageBox.Show(
                        nuevoEstado ? param.GetLocalizable("user_block_success")
                                    : param.GetLocalizable("user_unblock_success"),
                        param.GetLocalizable("ok_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                u.Bloqueado = !nuevoEstado;

                MessageBox.Show(
                    (nuevoEstado ? param.GetLocalizable("user_block_error_message")
                                 : param.GetLocalizable("user_unblock_error_message")) + " " + ex.Message,
                    param.GetLocalizable("user_modify_error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBloquear.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.Index < 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("user_select_from_list_message"),
                    param.GetLocalizable("user_select_from_list_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var u = dgvUsuarios.CurrentRow.DataBoundItem as BE.Usuario;
            if (u == null) return;

            bool target = !u.Deshabilitado;
            string msg = target ? param.GetLocalizable("user_disable_confirm_message")
                                : param.GetLocalizable("user_enable_confirm_message");
            string title = param.GetLocalizable("confirm_title");

            if (!ShowConfirm(msg, title)) return;

            try
            {
                u.Deshabilitado = target;
                UsuarioBLL.GetInstance().Update(u);
                CargarDatos();
                ReselectAndSync(u.IdUsuario);
                MessageBox.Show(
                    target ? param.GetLocalizable("user_disable_success") : param.GetLocalizable("user_enable_success"),
                    param.GetLocalizable("ok_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    (target ? param.GetLocalizable("user_disable_error_message") : param.GetLocalizable("user_enable_error_message")) + ex.Message,
                    param.GetLocalizable("user_modify_error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InputsChanged(object sender, EventArgs e)
        {
            UpdateModifyButton();
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
                Documento = u.NumeroDocumento ?? ""
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

        private void UpdateActionButtonsText(BE.Usuario u)
        {
            btnBloquear.Text = u.Bloqueado
                ? param.GetLocalizable("user_unblock_button")
                : param.GetLocalizable("user_block_button");

            btnDeshabilitar.Text = u.Deshabilitado
                ? param.GetLocalizable("user_enable_button")
                : param.GetLocalizable("user_disable_button");
        }

        private void ReselectAndSync(int id)
        {
            foreach (DataGridViewRow r in dgvUsuarios.Rows)
            {
                if (r.DataBoundItem is BE.Usuario uRow && uRow.IdUsuario == id)
                {
                    r.Selected = true;
                    dgvUsuarios.CurrentCell = r.Cells[0];
                    txtId.Text = uRow.IdUsuario.ToString();
                    txtNombre.Text = uRow.NombreUsuario ?? "";
                    txtApellido.Text = uRow.ApellidoUsuario ?? "";
                    txtCorreo.Text = uRow.CorreoElectronico ?? "";
                    txtTelefono.Text = uRow.TelefonoContacto ?? "";
                    txtDireccion.Text = uRow.DireccionUsuario ?? "";
                    txtDocumento.Text = uRow.NumeroDocumento ?? "";
                    SetBaselineFrom(uRow);
                    UpdateActionButtonsText(uRow);
                    break;
                }
            }
            btnModificar.Enabled = false;
        }

        private bool ShowConfirm(string message, string title)
        {
            string okText = param.GetLocalizable("confirm_ok_button");
            string cancelText = param.GetLocalizable("confirm_cancel_button");

            using (var f = new Form
            {
                Width = 420,
                Height = 180,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = title
            })
            {
                var lbl = new Label { Left = 15, Top = 20, Width = 380, Height = 60, Text = message };
                var btnOk = new Button { Text = okText, Left = 220, Width = 85, Top = 90, DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = cancelText, Left = 310, Width = 85, Top = 90, DialogResult = DialogResult.Cancel };
                f.Controls.Add(lbl);
                f.Controls.Add(btnOk);
                f.Controls.Add(btnCancel);
                f.AcceptButton = btnOk;
                f.CancelButton = btnCancel;
                return f.ShowDialog(this) == DialogResult.OK;
            }
        }
        private void UpdateTexts()
        {
            txtTelefono.Tag = TextBoxTag.PhoneNumber;
            txtDocumento.Tag = TextBoxTag.Num12;
            txtCorreo.Tag = TextBoxTag.MailUrban;
            txtDireccion.Tag = TextBoxTag.SqlSafe;
            txtNombre.Tag = TextBoxTag.SqlSafe;
            txtApellido.Tag = TextBoxTag.SqlSafe;

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

            string helpTitle = param.GetLocalizable("users_management_help_title");
            string helpBody = param.GetLocalizable("users_management_help_body");
            SetHelpContext(helpTitle, helpBody);
        }
    }
}

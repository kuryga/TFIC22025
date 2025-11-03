using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BLL.Seguridad;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class GestionarFamiliasForm : BaseForm
    {
        private int _usuarioSeleccionadoId = 0;
        private HashSet<int> _familiasAsignadasOriginal = new HashSet<int>();
        private bool _suspendSelectionEvents = false;

        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarFamiliasForm()
        {
            InitializeComponent();
            ConfigurarGrids();
            CargarUsuarios();
            UpdateTexts();

            this.Shown += (s, e) => AplicarPermisosFamilias();
        }

        private void ConfigurarGrids()
        {
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AllowUserToAddRows = false;

            dgvDisponibles.MultiSelect = false;
            dgvDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDisponibles.AllowUserToAddRows = false;

            dgvAsignadas.MultiSelect = false;
            dgvAsignadas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignadas.AllowUserToAddRows = false;

            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.Columns.Clear();
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("user_id_label"),
                Name = "IdUsuario",
                Width = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("full_name_label"),
                Name = "NombreCompleto",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("user_email_label"),
                Name = "correoElectronico",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvDisponibles.AutoGenerateColumns = false;
            dgvDisponibles.Columns.Clear();
            dgvDisponibles.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("user_id_label"),
                Name = "IdFamilia",
                Width = 20,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgvDisponibles.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("familia_label"),
                Name = "NombreFamilia",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvDisponibles.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("descripcion_label"),
                Name = "descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvAsignadas.AutoGenerateColumns = false;
            dgvAsignadas.Columns.Clear();
            dgvAsignadas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("user_id_label"),
                Name = "IdFamilia",
                Width = 20,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgvAsignadas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("familia_label"),
                Name = "NombreFamilia",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvAsignadas.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = param.GetLocalizable("descripcion_label"),
                Name = "descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
            dgvDisponibles.SelectionChanged += DgvDisponibles_SelectionChanged;
            dgvAsignadas.SelectionChanged += DgvAsignadas_SelectionChanged;
        }

        private void CargarUsuarios()
        {
            dgvUsuarios.Rows.Clear();
            var usuarios = UsuarioBLL.GetInstance().GetAll();

            foreach (var u in usuarios)
            {
                var nombreCompleto = $"{u.NombreUsuario} {u.ApellidoUsuario}".Trim();
                dgvUsuarios.Rows.Add(u.IdUsuario.ToString(), nombreCompleto, u.CorreoElectronico);
            }

            if (dgvUsuarios.Rows.Count > 0)
                dgvUsuarios.ClearSelection();
        }

        private void AplicarPermisosFamilias()
        {
            try
            {
                bool puedeCrear = PermisosBLL.GetInstance().PuedeCrearFamilia();
                bool puedeModificar = PermisosBLL.GetInstance().PuedeModificarFamilia();
                bool puedeGuardar = PermisosBLL.GetInstance().PuedeAsignarFamilia();
                btnCrear.Enabled = puedeCrear;
                btnModificar.Enabled = puedeModificar;
                btnGuardar.Enabled = puedeGuardar;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("familia_apply_permissions_error_message") + ex.Message,
                    param.GetLocalizable("permissions_label"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null || dgvUsuarios.CurrentRow.IsNewRow) return;

            if (!int.TryParse(Convert.ToString(dgvUsuarios.CurrentRow.Cells["IdUsuario"].Value), out var idUsuario))
                return;

            _usuarioSeleccionadoId = idUsuario;

            try
            {
                CargarFamiliasParaUsuario(_usuarioSeleccionadoId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("familias_load_error_message") + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarFamiliasParaUsuario(int idUsuario)
        {
            dgvDisponibles.Rows.Clear();
            dgvAsignadas.Rows.Clear();
            _familiasAsignadasOriginal.Clear();

            var todas = PermisosBLL.GetInstance().GetAllFamilias();
            var asignadas = PermisosBLL.GetInstance().GetFamiliasByUsuario(idUsuario) ?? new List<BE.Familia>();

            foreach (var f in asignadas) _familiasAsignadasOriginal.Add(f.IdFamilia);

            var setAsignadas = new HashSet<int>(asignadas.Select(f => f.IdFamilia));

            foreach (var f in todas)
            {
                if (setAsignadas.Contains(f.IdFamilia))
                    dgvAsignadas.Rows.Add(f.IdFamilia, f.NombreFamilia, f.Descripcion);
                else
                    dgvDisponibles.Rows.Add(f.IdFamilia, f.NombreFamilia, f.Descripcion);
            }

            _suspendSelectionEvents = true;
            dgvDisponibles.ClearSelection();
            dgvDisponibles.CurrentCell = null;
            dgvAsignadas.ClearSelection();
            dgvAsignadas.CurrentCell = null;
            _suspendSelectionEvents = false;

            BeginInvoke((Action)(() =>
            {
                _suspendSelectionEvents = true;
                dgvDisponibles.ClearSelection();
                dgvDisponibles.CurrentCell = null;
                dgvAsignadas.ClearSelection();
                dgvAsignadas.CurrentCell = null;
                _suspendSelectionEvents = false;
            }));
        }

        private void DgvDisponibles_SelectionChanged(object sender, EventArgs e)
        {
            if (_suspendSelectionEvents) return;
            _suspendSelectionEvents = true;
            try
            {
                if (dgvDisponibles.Focused && dgvDisponibles.SelectedRows.Count > 0)
                    dgvAsignadas.ClearSelection();
            }
            finally { _suspendSelectionEvents = false; }
        }

        private void DgvAsignadas_SelectionChanged(object sender, EventArgs e)
        {
            if (_suspendSelectionEvents) return;
            _suspendSelectionEvents = true;
            try
            {
                if (dgvAsignadas.Focused && dgvAsignadas.SelectedRows.Count > 0)
                    dgvDisponibles.ClearSelection();
            }
            finally { _suspendSelectionEvents = false; }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvDisponibles.CurrentRow == null) return;

            var id = Convert.ToInt32(dgvDisponibles.CurrentRow.Cells["IdFamilia"].Value);
            var nombre = Convert.ToString(dgvDisponibles.CurrentRow.Cells["NombreFamilia"].Value);
            var desc = Convert.ToString(dgvDisponibles.CurrentRow.Cells["descripcion"].Value);

            dgvDisponibles.Rows.RemoveAt(dgvDisponibles.CurrentRow.Index);
            dgvAsignadas.Rows.Add(id, nombre, desc);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAsignadas.CurrentRow == null) return;

            var id = Convert.ToInt32(dgvAsignadas.CurrentRow.Cells["IdFamilia"].Value);
            var nombre = Convert.ToString(dgvAsignadas.CurrentRow.Cells["NombreFamilia"].Value);
            var desc = Convert.ToString(dgvAsignadas.CurrentRow.Cells["descripcion"].Value);

            dgvAsignadas.Rows.RemoveAt(dgvAsignadas.CurrentRow.Index);
            dgvDisponibles.Rows.Add(id, nombre, desc);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionadoId <= 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("select_user_simple_message"),
                    param.GetLocalizable("notice_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var asignadasAhora = new HashSet<int>();
            foreach (DataGridViewRow r in dgvAsignadas.Rows)
            {
                if (r.IsNewRow) continue;
                if (r.Cells["IdFamilia"].Value == null) continue;
                asignadasAhora.Add(Convert.ToInt32(r.Cells["IdFamilia"].Value));
            }

            var hayCambios = !_familiasAsignadasOriginal.SetEquals(asignadasAhora);
            if (!hayCambios)
            {
                MessageBox.Show(
                    param.GetLocalizable("user_no_changes_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                PermisosBLL.GetInstance().SetFamiliasForUsuario(_usuarioSeleccionadoId, asignadasAhora);
                _familiasAsignadasOriginal = new HashSet<int>(asignadasAhora);
                MessageBox.Show(
                    param.GetLocalizable("familias_assigned_saved_success"),
                    param.GetLocalizable("ok_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("save_error_message") + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private BE.Familia GetFamiliaSeleccionada()
        {
            DataGridViewRow row = null;
            if (dgvDisponibles.SelectedRows.Count > 0)
                row = dgvDisponibles.SelectedRows[0];
            else if (dgvAsignadas.SelectedRows.Count > 0)
                row = dgvAsignadas.SelectedRows[0];

            if (row == null || row.IsNewRow)
                return new BE.Familia { IdFamilia = -1 };

            return new BE.Familia
            {
                IdFamilia = Convert.ToInt32(row.Cells["IdFamilia"].Value),
                NombreFamilia = Convert.ToString(row.Cells["NombreFamilia"].Value) ?? string.Empty,
                Descripcion = Convert.ToString(row.Cells["descripcion"].Value) ?? string.Empty
            };
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new GestionarFamiliaForm(null))
                {
                    var dr = form.ShowDialog(this);
                    if (dr == DialogResult.OK && _usuarioSeleccionadoId > 0)
                        CargarFamiliasParaUsuario(_usuarioSeleccionadoId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("familia_create_error_message") + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var seleccion = GetFamiliaSeleccionada();
            if (seleccion == null || seleccion.IdFamilia <= 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("select_family_from_lists_message"),
                    param.GetLocalizable("notice_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var form = new GestionarFamiliaForm(seleccion))
                {
                    var dr = form.ShowDialog(this);
                    if (dr == DialogResult.OK && _usuarioSeleccionadoId > 0)
                        CargarFamiliasParaUsuario(_usuarioSeleccionadoId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("familia_modify_error_message") + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTexts()
        {
            lblAsignadas.Text = param.GetLocalizable("familias_assigned_label");
            lblDisponibles.Text = param.GetLocalizable("familias_unassigned_label");
            lblUsuarios.Text = param.GetLocalizable("users_label");
            btnCrear.Text = param.GetLocalizable("familia_create_button");
            btnGuardar.Text = param.GetLocalizable("save_button");
            btnModificar.Text = param.GetLocalizable("familia_modify_button");
        }
    }
}

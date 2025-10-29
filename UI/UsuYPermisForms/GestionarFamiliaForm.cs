using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using BLL.Seguridad;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class GestionarFamiliaForm : BaseForm
    {
        private readonly bool _isEdit;
        private readonly int _familiaId;
        private HashSet<int> _patentesAsignadasOriginal = new HashSet<int>();
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarFamiliaForm() : this(null) { }

        public GestionarFamiliaForm(BE.Familia familiaAEditar)
        {
            InitializeComponent();
            ConfigurarControles();
            UpdateTexts();

            _isEdit = familiaAEditar != null;
            _familiaId = familiaAEditar?.IdFamilia ?? 0;

            if (_isEdit)
            {
                txtId.Text = familiaAEditar.IdFamilia.ToString();
                txtNombre.Text = familiaAEditar.NombreFamilia ?? string.Empty;
                txtDesc.Text = familiaAEditar.Descripcion ?? string.Empty;
                txtId.Enabled = false;
                CargarPatentesParaFamilia(_familiaId);
            }
            else
            {
                txtId.Text = string.Empty;
                txtNombre.Text = string.Empty;
                txtDesc.Text = string.Empty;
                txtId.Enabled = false;
                CargarPatentesParaFamilia(0);
            }
        }

        private void ConfigurarControles()
        {
            txtId.ReadOnly = true;
            txtId.TabStop = false;

            ConfigurarGridPatentes(dgvDisponibles);
            ConfigurarGridPatentes(dgvAsignadas);

            dgvDisponibles.MultiSelect = false;
            dgvDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDisponibles.AllowUserToAddRows = false;

            dgvAsignadas.MultiSelect = false;
            dgvAsignadas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsignadas.AllowUserToAddRows = false;
        }

        private static void ConfigurarGridPatentes(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = ParametrizacionBLL.GetInstance().GetLocalizable("user_id_label"),
                Name = "IdPatente",
                Width = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = ParametrizacionBLL.GetInstance().GetLocalizable("patente_label"),
                Name = "NombrePatente",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = ParametrizacionBLL.GetInstance().GetLocalizable("descripcion_label"),
                Name = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        private void CargarPatentesParaFamilia(int idFamilia)
        {
            dgvDisponibles.Rows.Clear();
            dgvAsignadas.Rows.Clear();
            _patentesAsignadasOriginal.Clear();

            var todas = PermisosBLL.GetInstance().GetAllPatentes() ?? new List<BE.Patente>();
            var asignadas = (idFamilia > 0)
                ? (PermisosBLL.GetInstance().GetPatentesByFamilia(idFamilia) ?? new List<BE.Patente>())
                : new List<BE.Patente>();

            foreach (var p in asignadas) _patentesAsignadasOriginal.Add(p.IdPatente);

            var setAsignadas = new HashSet<int>(asignadas.Select(p => p.IdPatente));

            foreach (var p in todas)
            {
                var row = new object[] { p.IdPatente, p.NombrePatente, p.Descripcion };
                if (setAsignadas.Contains(p.IdPatente))
                    dgvAsignadas.Rows.Add(row);
                else
                    dgvDisponibles.Rows.Add(row);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvDisponibles.CurrentRow == null || dgvDisponibles.CurrentRow.IsNewRow) return;

            var id = Convert.ToInt32(dgvDisponibles.CurrentRow.Cells["IdPatente"].Value);
            var nom = Convert.ToString(dgvDisponibles.CurrentRow.Cells["NombrePatente"].Value);
            var des = Convert.ToString(dgvDisponibles.CurrentRow.Cells["Descripcion"].Value);

            int idx = dgvDisponibles.CurrentRow.Index;
            dgvDisponibles.Rows.RemoveAt(idx);

            dgvAsignadas.ClearSelection();
            var newIdx = dgvAsignadas.Rows.Add(id, nom, des);
            dgvAsignadas.Rows[newIdx].Selected = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAsignadas.CurrentRow == null || dgvAsignadas.CurrentRow.IsNewRow) return;

            var id = Convert.ToInt32(dgvAsignadas.CurrentRow.Cells["IdPatente"].Value);
            var nom = Convert.ToString(dgvAsignadas.CurrentRow.Cells["NombrePatente"].Value);
            var des = Convert.ToString(dgvAsignadas.CurrentRow.Cells["Descripcion"].Value);

            int idx = dgvAsignadas.CurrentRow.Index;
            dgvAsignadas.Rows.RemoveAt(idx);

            dgvDisponibles.ClearSelection();
            var newIdx = dgvDisponibles.Rows.Add(id, nom, des);
            dgvDisponibles.Rows[newIdx].Selected = true;
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            var nombre = (txtNombre.Text ?? string.Empty).Trim();
            var descripcion = (txtDesc.Text ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    param.GetLocalizable("family_name_required_message"),
                    param.GetLocalizable("validation_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            var patentesAhora = new HashSet<int>();
            foreach (DataGridViewRow r in dgvAsignadas.Rows)
            {
                if (r.IsNewRow) continue;
                if (r.Cells["IdPatente"].Value == null) continue;
                patentesAhora.Add(Convert.ToInt32(r.Cells["IdPatente"].Value));
            }

            try
            {
                if (_isEdit)
                {
                    var fam = new BE.Familia
                    {
                        IdFamilia = _familiaId,
                        NombreFamilia = nombre,
                        Descripcion = descripcion
                    };

                    PermisosBLL.GetInstance().UpdateFamilia(fam, patentesAhora);

                    MessageBox.Show(
                        param.GetLocalizable("family_updated_success"),
                        param.GetLocalizable("ok_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    var fam = new BE.Familia
                    {
                        NombreFamilia = nombre,
                        Descripcion = descripcion
                    };

                    int nuevoId = PermisosBLL.GetInstance().CreateFamilia(fam, patentesAhora);

                    MessageBox.Show(
                        param.GetLocalizable("family_created_success_prefix") + nuevoId,
                        param.GetLocalizable("ok_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("error_prefix_message") + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTexts()
        {
            lblDesc.Text = param.GetLocalizable("users_label");
            lblNombre.Text = param.GetLocalizable("users_label");
            lblDisponibles.Text = param.GetLocalizable("patentes_unassigned_label");
            lblDisponibles.Text = param.GetLocalizable("patentes_assigned_label");

            btnFinalizar.Text = param.GetLocalizable("save_button");
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using TipoEdificacionBLL = BLL.Genericos.TipoEdificacionBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class GestionarTipoEdificacionForm : BaseForm
    {
        private string _descripcionOriginal = string.Empty;
        private bool _cargandoFila = false;
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarTipoEdificacionForm()
        {
            InitializeComponent();
            ConfigurarGrid();
            HookEventos();
            UpdateTexts();
            CargarDatos();
            btnModificar.Enabled = false;
            txtDescripcion.Tag = TextBoxTag.SqlSafe;
        }

        private void ConfigurarGrid()
        {
            dgvTipos.AutoGenerateColumns = false;
            dgvTipos.Columns.Clear();

            var colId = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = param.GetLocalizable("tipoedif_id_label"),
                DataPropertyName = "IdTipoEdificacion",
                Width = 70,
                ReadOnly = true
            };
            var colDesc = new DataGridViewTextBoxColumn
            {
                Name = "colDesc",
                HeaderText = param.GetLocalizable("tipoedif_description_label"),
                DataPropertyName = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colDeshab = new DataGridViewCheckBoxColumn
            {
                Name = "colDeshab",
                HeaderText = param.GetLocalizable("tipoedif_disabled_label"),
                DataPropertyName = "Deshabilitado",
                Width = 120,
                ReadOnly = true
            };

            dgvTipos.Columns.AddRange(colId, colDesc, colDeshab);
            dgvTipos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTipos.MultiSelect = false;
            dgvTipos.AllowUserToAddRows = false;
            dgvTipos.AllowUserToDeleteRows = false;
            dgvTipos.ReadOnly = true;
        }

        private void HookEventos()
        {
            dgvTipos.SelectionChanged += dgvTipos_SelectionChanged;
            txtDescripcion.TextChanged += txtDescripcion_TextChanged;

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("tipoedif_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("tipoedif_modify_button");
        }

        private void UpdateTexts()
        {
            lblTitle.Text = param.GetLocalizable("tipoedif_title");
            txtId.ReadOnly = true;

            if (dgvTipos.Columns["colId"] != null)
                dgvTipos.Columns["colId"].HeaderText = param.GetLocalizable("tipoedif_id_label");
            if (dgvTipos.Columns["colDesc"] != null)
                dgvTipos.Columns["colDesc"].HeaderText = param.GetLocalizable("tipoedif_description_label");
            if (dgvTipos.Columns["colDeshab"] != null)
                dgvTipos.Columns["colDeshab"].HeaderText = param.GetLocalizable("tipoedif_disabled_label");

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("tipoedif_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("tipoedif_modify_button");
        }

        private void CargarDatos(int? seleccionarId = null)
        {
            dgvTipos.DataSource = null;
            var lista = TipoEdificacionBLL.GetInstance().GetAll();
            dgvTipos.DataSource = lista;

            if (seleccionarId.HasValue && lista != null)
            {
                var row = lista
                    .Select((v, i) => new { v, i })
                    .FirstOrDefault(x => x.v.IdTipoEdificacion == seleccionarId.Value);
                if (row != null)
                {
                    dgvTipos.ClearSelection();
                    dgvTipos.Rows[row.i].Selected = true;
                    dgvTipos.CurrentCell = dgvTipos.Rows[row.i].Cells[0];
                }
            }

            AjustarTextoBotonDeshabilitar();
        }

        private void dgvTipos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTipos.CurrentRow == null || dgvTipos.CurrentRow.DataBoundItem == null) return;

            _cargandoFila = true;

            var item = dgvTipos.CurrentRow.DataBoundItem as BE.TipoEdificacion;
            if (item == null) { _cargandoFila = false; return; }

            txtId.Text = item.IdTipoEdificacion.ToString();
            txtDescripcion.Text = item.Descripcion ?? string.Empty;
            _descripcionOriginal = txtDescripcion.Text;

            btnModificar.Enabled = false;

            _cargandoFila = false;

            AjustarTextoBotonDeshabilitar(item);
        }

        private void AjustarTextoBotonDeshabilitar(BE.TipoEdificacion item = null)
        {
            if (btnDeshabilitar == null) return;

            if (item == null)
                item = dgvTipos.CurrentRow != null ? dgvTipos.CurrentRow.DataBoundItem as BE.TipoEdificacion : null;

            if (item == null)
            {
                btnDeshabilitar.Text = param.GetLocalizable("tipoedif_disable_button");
                btnDeshabilitar.Enabled = false;
                return;
            }

            btnDeshabilitar.Enabled = true;
            btnDeshabilitar.Text = item.Deshabilitado
                ? param.GetLocalizable("tipoedif_enable_button")
                : param.GetLocalizable("tipoedif_disable_button");
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFila) return;
            btnModificar.Enabled = !string.Equals(
                txtDescripcion.Text?.Trim() ?? string.Empty,
                _descripcionOriginal?.Trim() ?? string.Empty,
                StringComparison.Ordinal);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear tipo de edificación (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtId.Text, out var id) || id <= 0)
                {
                    MessageBox.Show(
                        param.GetLocalizable("tipoedif_select_valid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var nuevaDesc = txtDescripcion.Text?.Trim() ?? string.Empty;

                var obj = new BE.TipoEdificacion
                {
                    IdTipoEdificacion = id,
                    Descripcion = nuevaDesc
                };

                if (TipoEdificacionBLL.GetInstance().Update(obj))
                {
                    _descripcionOriginal = nuevaDesc;
                    btnModificar.Enabled = false;
                    CargarDatos(seleccionarId: id);

                    MessageBox.Show(
                        param.GetLocalizable("tipoedif_update_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("tipoedif_update_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTipos.CurrentRow == null || dgvTipos.CurrentRow.DataBoundItem == null)
                {
                    MessageBox.Show(
                        param.GetLocalizable("tipoedif_select_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var item = dgvTipos.CurrentRow.DataBoundItem as BE.TipoEdificacion;
                if (item == null) return;

                var id = item.IdTipoEdificacion;
                var estadoActual = item.Deshabilitado;

                TipoEdificacionBLL.GetInstance().Deshabilitar(id, !estadoActual);

                CargarDatos(seleccionarId: id);

                MessageBox.Show(
                    estadoActual
                        ? param.GetLocalizable("tipoedif_enable_success_message")
                        : param.GetLocalizable("tipoedif_disable_success_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("tipoedif_toggle_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

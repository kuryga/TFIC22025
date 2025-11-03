using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using MaquinariaBLL = BLL.Genericos.MaquinariaBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class GestionarMaquinariaForm : BaseForm
    {
        private string _nombreOriginal = string.Empty;
        private decimal _costoOriginal = 0m;
        private bool _cargandoFila = false;

        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarMaquinariaForm()
        {
            InitializeComponent();
            ConfigurarGrid();
            HookEventos();
            UpdateTexts();
            CargarDatos();
            btnModificar.Enabled = false;
        }

        private void ConfigurarGrid()
        {
            dgvMaquinaria.AutoGenerateColumns = false;
            dgvMaquinaria.Columns.Clear();

            var colId = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = param.GetLocalizable("maquinaria_id_label"),
                DataPropertyName = "IdMaquinaria",
                Width = 70,
                ReadOnly = true
            };
            var colNombre = new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = param.GetLocalizable("maquinaria_name_label"),
                DataPropertyName = "Nombre",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colCosto = new DataGridViewTextBoxColumn
            {
                Name = "colCosto",
                HeaderText = param.GetLocalizable("maquinaria_cost_label"),
                DataPropertyName = "CostoPorHora",
                Width = 130,
                ReadOnly = true
            };
            colCosto.DefaultCellStyle.Format = "N2";

            var colDeshab = new DataGridViewCheckBoxColumn
            {
                Name = "colDeshab",
                HeaderText = param.GetLocalizable("maquinaria_disabled_label"),
                DataPropertyName = "Deshabilitado",
                Width = 120,
                ReadOnly = true
            };

            dgvMaquinaria.Columns.AddRange(colId, colNombre, colCosto, colDeshab);
            dgvMaquinaria.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaquinaria.MultiSelect = false;
            dgvMaquinaria.AllowUserToAddRows = false;
            dgvMaquinaria.AllowUserToDeleteRows = false;
            dgvMaquinaria.ReadOnly = true;
        }

        private void HookEventos()
        {
            dgvMaquinaria.SelectionChanged += dgvMaquinaria_SelectionChanged;
            txtNombre.TextChanged += Campos_TextChanged;
            txtCosto.TextChanged += Campos_TextChanged;
        }

        private void UpdateTexts()
        {
            lblTitle.Text = param.GetLocalizable("maquinaria_title");
            txtNombre.Tag = TextBoxTag.SqlSafe;
            txtCosto.Tag = TextBoxTag.Price;

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("maquinaria_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("maquinaria_modify_button");
            AjustarTextoBotonToggle();
        }

        private void CargarDatos(int? seleccionarId = null)
        {
            dgvMaquinaria.DataSource = null;
            var lista = MaquinariaBLL.GetInstance().GetAll();
            dgvMaquinaria.DataSource = lista;

            if (seleccionarId.HasValue && lista != null)
            {
                var row = lista
                    .Select((v, i) => new { v, i })
                    .FirstOrDefault(x => x.v.IdMaquinaria == seleccionarId.Value);
                if (row != null)
                {
                    dgvMaquinaria.ClearSelection();
                    dgvMaquinaria.Rows[row.i].Selected = true;
                    dgvMaquinaria.CurrentCell = dgvMaquinaria.Rows[row.i].Cells[0];
                }
            }

            AjustarTextoBotonToggle();
        }

        private void dgvMaquinaria_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaquinaria.CurrentRow == null || dgvMaquinaria.CurrentRow.DataBoundItem == null) return;

            _cargandoFila = true;

            var item = dgvMaquinaria.CurrentRow.DataBoundItem as BE.Maquinaria;
            if (item == null) { _cargandoFila = false; return; }

            txtId.Text = item.IdMaquinaria.ToString();
            txtNombre.Text = item.Nombre ?? string.Empty;
            txtCosto.Text = item.CostoPorHora.ToString("0.00");

            _nombreOriginal = txtNombre.Text;
            _costoOriginal = item.CostoPorHora;

            btnModificar.Enabled = false;

            _cargandoFila = false;

            AjustarTextoBotonToggle(item);
        }

        private void Campos_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFila) return;

            bool nombreCambio = !string.Equals(txtNombre.Text ?? string.Empty, _nombreOriginal ?? string.Empty, StringComparison.Ordinal);
            bool costoCambio = TryParseDecimal(txtCosto.Text, out var costoNuevo) && costoNuevo != _costoOriginal;

            btnModificar.Enabled = nombreCambio || costoCambio;
        }

        private static bool TryParseDecimal(string input, out decimal value)
        {
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                return true;
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                param.GetLocalizable("maquinaria_create_pending_message"),
                param.GetLocalizable("info_title"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(txtId.Text, out id) || id <= 0)
                {
                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_select_valid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal costo;
                if (!TryParseDecimal(txtCosto.Text, out costo))
                {
                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_cost_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var obj = new BE.Maquinaria
                {
                    IdMaquinaria = id,
                    Nombre = txtNombre.Text != null ? txtNombre.Text.Trim() : string.Empty,
                    CostoPorHora = costo
                };

                if (MaquinariaBLL.GetInstance().Update(obj))
                {
                    _nombreOriginal = obj.Nombre;
                    _costoOriginal = obj.CostoPorHora;

                    btnModificar.Enabled = false;
                    CargarDatos(seleccionarId: id);

                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_update_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("maquinaria_update_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMaquinaria.CurrentRow == null || dgvMaquinaria.CurrentRow.DataBoundItem == null)
                {
                    MessageBox.Show(
                        param.GetLocalizable("maquinaria_select_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var item = dgvMaquinaria.CurrentRow.DataBoundItem as BE.Maquinaria;
                if (item == null) return;

                var id = item.IdMaquinaria;
                var estadoActual = item.Deshabilitado;

                MaquinariaBLL.GetInstance().Deshabilitar(id, !estadoActual);

                CargarDatos(seleccionarId: id);

                MessageBox.Show(
                    estadoActual
                        ? param.GetLocalizable("maquinaria_enable_success_message")
                        : param.GetLocalizable("maquinaria_disable_success_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("maquinaria_toggle_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AjustarTextoBotonToggle(BE.Maquinaria item = null)
        {
            if (btnDeshabilitar == null) return;

            if (item == null)
                item = dgvMaquinaria.CurrentRow != null ? dgvMaquinaria.CurrentRow.DataBoundItem as BE.Maquinaria : null;

            if (item == null)
            {
                btnDeshabilitar.Text = param.GetLocalizable("maquinaria_disable_button");
                btnDeshabilitar.Enabled = false;
                return;
            }

            btnDeshabilitar.Enabled = true;
            btnDeshabilitar.Text = item.Deshabilitado
                ? param.GetLocalizable("maquinaria_enable_button")
                : param.GetLocalizable("maquinaria_disable_button");
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using MonedaBLL = BLL.Genericos.MonedaBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class GestionarMonedaForm : BaseForm
    {
        private string _nombreOriginal = string.Empty;
        private string _simboloOriginal = string.Empty;
        private decimal _valorOriginal = 0m;
        private bool _cargandoFila = false;

        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarMonedaForm()
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
            dgvMoneda.AutoGenerateColumns = false;
            dgvMoneda.Columns.Clear();

            var colId = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = param.GetLocalizable("moneda_id_label"),
                DataPropertyName = "IdMoneda",
                Width = 70,
                ReadOnly = true
            };
            var colNombre = new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = param.GetLocalizable("moneda_name_label"),
                DataPropertyName = "NombreMoneda",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colSimbolo = new DataGridViewTextBoxColumn
            {
                Name = "colSimbolo",
                HeaderText = param.GetLocalizable("moneda_symbol_label"),
                DataPropertyName = "Simbolo",
                Width = 90,
                ReadOnly = true
            };
            var colValor = new DataGridViewTextBoxColumn
            {
                Name = "colValor",
                HeaderText = param.GetLocalizable("moneda_rate_label"),
                DataPropertyName = "ValorCambio",
                Width = 120,
                ReadOnly = true
            };
            colValor.DefaultCellStyle.Format = "N4";

            var colDeshab = new DataGridViewCheckBoxColumn
            {
                Name = "colDeshab",
                HeaderText = param.GetLocalizable("moneda_disabled_label"),
                DataPropertyName = "Deshabilitado",
                Width = 120,
                ReadOnly = true
            };

            dgvMoneda.Columns.AddRange(colId, colNombre, colSimbolo, colValor, colDeshab);
            dgvMoneda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMoneda.MultiSelect = false;
            dgvMoneda.AllowUserToAddRows = false;
            dgvMoneda.AllowUserToDeleteRows = false;
            dgvMoneda.ReadOnly = true;
        }

        private void HookEventos()
        {
            dgvMoneda.SelectionChanged += dgvMoneda_SelectionChanged;
            txtNombre.TextChanged += Campos_TextChanged;
            txtSimbolo.TextChanged += Campos_TextChanged;
            txtValor.TextChanged += Campos_TextChanged;

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("moneda_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("moneda_modify_button");
            AjustarTextoBotonToggle();
        }

        private void UpdateTexts()
        {
            
            lblTitle.Text = param.GetLocalizable("moneda_title"); 
            lblId.Text = param.GetLocalizable("moneda_id_label");
            lblNombre.Text = param.GetLocalizable("moneda_name_label");
            lblSimbolo.Text = param.GetLocalizable("moneda_symbol_label");
            lblValor.Text = param.GetLocalizable("moneda_rate_label");

            if (dgvMoneda.Columns["colId"] != null)
                dgvMoneda.Columns["colId"].HeaderText = param.GetLocalizable("moneda_id_label");
            if (dgvMoneda.Columns["colNombre"] != null)
                dgvMoneda.Columns["colNombre"].HeaderText = param.GetLocalizable("moneda_name_label");
            if (dgvMoneda.Columns["colSimbolo"] != null)
                dgvMoneda.Columns["colSimbolo"].HeaderText = param.GetLocalizable("moneda_symbol_label");
            if (dgvMoneda.Columns["colValor"] != null)
                dgvMoneda.Columns["colValor"].HeaderText = param.GetLocalizable("moneda_rate_label");
            if (dgvMoneda.Columns["colDeshab"] != null)
                dgvMoneda.Columns["colDeshab"].HeaderText = param.GetLocalizable("moneda_disabled_label");

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("moneda_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("moneda_modify_button");
            AjustarTextoBotonToggle();
        }

        private void CargarDatos(int? seleccionarId = null)
        {
            dgvMoneda.DataSource = null;
            var lista = MonedaBLL.GetInstance().GetAll();
            dgvMoneda.DataSource = lista;

            if (seleccionarId.HasValue && lista != null)
            {
                var row = lista
                    .Select((v, i) => new { v, i })
                    .FirstOrDefault(x => x.v.IdMoneda == seleccionarId.Value);
                if (row != null)
                {
                    dgvMoneda.ClearSelection();
                    dgvMoneda.Rows[row.i].Selected = true;
                    dgvMoneda.CurrentCell = dgvMoneda.Rows[row.i].Cells[0];
                }
            }

            AjustarTextoBotonToggle();
        }

        private void dgvMoneda_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMoneda.CurrentRow == null || dgvMoneda.CurrentRow.DataBoundItem == null) return;

            _cargandoFila = true;

            var item = dgvMoneda.CurrentRow.DataBoundItem as BE.Moneda;
            if (item == null) { _cargandoFila = false; return; }

            txtId.Text = item.IdMoneda.ToString();
            txtNombre.Text = item.NombreMoneda ?? string.Empty;
            txtSimbolo.Text = item.Simbolo ?? string.Empty;
            txtValor.Text = item.ValorCambio.ToString("0.0000");

            _nombreOriginal = txtNombre.Text;
            _simboloOriginal = txtSimbolo.Text;
            _valorOriginal = item.ValorCambio;

            btnModificar.Enabled = false;

            _cargandoFila = false;

            AjustarTextoBotonToggle(item);
        }

        private void Campos_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFila) return;

            bool nombreCambio = !string.Equals(txtNombre.Text ?? string.Empty, _nombreOriginal ?? string.Empty, StringComparison.Ordinal);
            bool simboloCambio = !string.Equals(txtSimbolo.Text ?? string.Empty, _simboloOriginal ?? string.Empty, StringComparison.Ordinal);

            bool valorCambio = false;
            decimal valorNuevo;
            if (TryParseDecimal(txtValor.Text, out valorNuevo))
                valorCambio = valorNuevo != _valorOriginal;

            btnModificar.Enabled = nombreCambio || simboloCambio || valorCambio;
        }

        private static bool TryParseDecimal(string input, out decimal value)
        {
            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                return true;
            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear moneda pendiente de implementación");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(txtId.Text, out id) || id <= 0)
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_select_valid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal valor;
                if (!TryParseDecimal(txtValor.Text, out valor))
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_rate_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var obj = new BE.Moneda
                {
                    IdMoneda = id,
                    NombreMoneda = txtNombre.Text != null ? txtNombre.Text.Trim() : string.Empty,
                    Simbolo = txtSimbolo.Text != null ? txtSimbolo.Text.Trim() : string.Empty,
                    ValorCambio = valor
                };

                if (MonedaBLL.GetInstance().Update(obj))
                {
                    _nombreOriginal = obj.NombreMoneda;
                    _simboloOriginal = obj.Simbolo;
                    _valorOriginal = obj.ValorCambio;

                    btnModificar.Enabled = false;
                    CargarDatos(seleccionarId: id);

                    MessageBox.Show(
                        param.GetLocalizable("moneda_update_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("moneda_update_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMoneda.CurrentRow == null || dgvMoneda.CurrentRow.DataBoundItem == null)
                {
                    MessageBox.Show(
                        param.GetLocalizable("moneda_select_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var item = dgvMoneda.CurrentRow.DataBoundItem as BE.Moneda;
                if (item == null) return;

                var id = item.IdMoneda;
                var estadoActual = item.Deshabilitado;

                MonedaBLL.GetInstance().Deshabilitar(id, !estadoActual);

                CargarDatos(seleccionarId: id);

                MessageBox.Show(
                    estadoActual
                        ? param.GetLocalizable("moneda_enable_success_message")
                        : param.GetLocalizable("moneda_disable_success_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("moneda_toggle_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AjustarTextoBotonToggle(BE.Moneda item = null)
        {
            if (btnDeshabilitar == null) return;

            if (item == null)
                item = dgvMoneda.CurrentRow != null ? dgvMoneda.CurrentRow.DataBoundItem as BE.Moneda : null;

            if (item == null)
            {
                btnDeshabilitar.Text = param.GetLocalizable("moneda_disable_button");
                btnDeshabilitar.Enabled = false;
                return;
            }

            btnDeshabilitar.Enabled = true;
            btnDeshabilitar.Text = item.Deshabilitado
                ? param.GetLocalizable("moneda_enable_button")
                : param.GetLocalizable("moneda_disable_button");
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using MaterialBLL = BLL.Genericos.MaterialBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class GestionarMaterialForm : BaseForm
    {
        private string _nombreOriginal = string.Empty;
        private string _unidadOriginal = string.Empty;
        private decimal _precioOriginal = 0;
        private decimal _usoOriginal = 0;
        private bool _cargandoFila = false;

        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarMaterialForm()
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
            dgvMaterial.AutoGenerateColumns = false;
            dgvMaterial.Columns.Clear();

            var colId = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = param.GetLocalizable("material_id_label"),
                DataPropertyName = "IdMaterial",
                Width = 60,
                ReadOnly = true
            };
            var colNombre = new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = param.GetLocalizable("material_name_label"),
                DataPropertyName = "Nombre",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colUnidad = new DataGridViewTextBoxColumn
            {
                Name = "colUnidad",
                HeaderText = param.GetLocalizable("material_unit_label"),
                DataPropertyName = "UnidadMedida",
                Width = 140,
                ReadOnly = true
            };
            var colPrecio = new DataGridViewTextBoxColumn
            {
                Name = "colPrecio",
                HeaderText = param.GetLocalizable("material_price_label"),
                DataPropertyName = "PrecioUnidad",
                Width = 110,
                ReadOnly = true
            };
            colPrecio.DefaultCellStyle.Format = "N2";

            var colUso = new DataGridViewTextBoxColumn
            {
                Name = "colUso",
                HeaderText = param.GetLocalizable("material_usage_label"),
                DataPropertyName = "UsoPorM2",
                Width = 100,
                ReadOnly = true
            };
            colUso.DefaultCellStyle.Format = "N4";

            var colDeshab = new DataGridViewCheckBoxColumn
            {
                Name = "colDeshab",
                HeaderText = param.GetLocalizable("material_disabled_label"),
                DataPropertyName = "Deshabilitado",
                Width = 110,
                ReadOnly = true
            };

            dgvMaterial.Columns.AddRange(colId, colNombre, colUnidad, colPrecio, colUso, colDeshab);
            dgvMaterial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaterial.MultiSelect = false;
            dgvMaterial.AllowUserToAddRows = false;
            dgvMaterial.AllowUserToDeleteRows = false;
            dgvMaterial.ReadOnly = true;
        }

        private void HookEventos()
        {
            dgvMaterial.SelectionChanged += dgvMaterial_SelectionChanged;
            txtNombre.TextChanged += Campos_TextChanged;
            txtUnidad.TextChanged += Campos_TextChanged;
            txtPrecio.TextChanged += Campos_TextChanged;
            txtUso.TextChanged += Campos_TextChanged;

            btnCrear.Click += btnCrear_Click;
            btnModificar.Click += btnModificar_Click;
            btnDeshabilitar.Click += btnDeshabilitar_Click;
        }

        private void UpdateTexts()
        {
            lblTitle.Text = param.GetLocalizable("material_title");
            txtPrecio.Tag = TextBoxTag.Price;
            txtNombre.Tag = TextBoxTag.SqlSafe;
            txtUso.ReadOnly = true;
            txtId.ReadOnly = true;

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("material_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("material_modify_button");
            AjustarTextoBotonToggle();
        }

        private void CargarDatos(int? seleccionarId = null)
        {
            dgvMaterial.DataSource = null;
            var lista = MaterialBLL.GetInstance().GetAll();
            dgvMaterial.DataSource = lista;

            if (seleccionarId.HasValue && lista != null)
            {
                var row = lista
                    .Select((v, i) => new { v, i })
                    .FirstOrDefault(x => x.v.IdMaterial == seleccionarId.Value);
                if (row != null)
                {
                    dgvMaterial.ClearSelection();
                    dgvMaterial.Rows[row.i].Selected = true;
                    dgvMaterial.CurrentCell = dgvMaterial.Rows[row.i].Cells[0];
                }
            }

            AjustarTextoBotonToggle();
        }

        private void dgvMaterial_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMaterial.CurrentRow == null || dgvMaterial.CurrentRow.DataBoundItem == null) return;

            _cargandoFila = true;

            var item = dgvMaterial.CurrentRow.DataBoundItem as BE.Material;
            if (item == null) { _cargandoFila = false; return; }

            txtId.Text = item.IdMaterial.ToString();
            txtNombre.Text = item.Nombre ?? string.Empty;
            txtUnidad.Text = item.UnidadMedida ?? string.Empty;
            txtPrecio.Text = item.PrecioUnidad.ToString("0.00");
            txtUso.Text = item.UsoPorM2.ToString("0.0000");

            _nombreOriginal = txtNombre.Text;
            _unidadOriginal = txtUnidad.Text;
            _precioOriginal = item.PrecioUnidad;
            _usoOriginal = item.UsoPorM2;

            btnModificar.Enabled = false;

            _cargandoFila = false;

            AjustarTextoBotonToggle(item);
        }

        private void Campos_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFila) return;
            VerificarCambios();
        }

        private void VerificarCambios()
        {
            bool nombreCambio = !string.Equals(txtNombre.Text ?? string.Empty, _nombreOriginal ?? string.Empty, StringComparison.Ordinal);
            bool unidadCambio = !string.Equals(txtUnidad.Text ?? string.Empty, _unidadOriginal ?? string.Empty, StringComparison.Ordinal);

            bool precioCambio = false;
            decimal precioNuevo;
            if (TryParseDecimal(txtPrecio.Text, out precioNuevo))
                precioCambio = precioNuevo != _precioOriginal;

            bool usoCambio = false;
            decimal usoNuevo;
            if (TryParseDecimal(txtUso.Text, out usoNuevo))
                usoCambio = usoNuevo != _usoOriginal;

            btnModificar.Enabled = nombreCambio || unidadCambio || precioCambio || usoCambio;
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
                "Crear material pendiente de implementación",
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
                        param.GetLocalizable("material_select_valid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal precio;
                if (!TryParseDecimal(txtPrecio.Text, out precio))
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_price_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal uso;
                if (!TryParseDecimal(txtUso.Text, out uso))
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_usage_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var obj = new BE.Material
                {
                    IdMaterial = id,
                    Nombre = txtNombre.Text != null ? txtNombre.Text.Trim() : string.Empty,
                    UnidadMedida = txtUnidad.Text != null ? txtUnidad.Text.Trim() : string.Empty,
                    PrecioUnidad = precio,
                    UsoPorM2 = uso
                };

                if (MaterialBLL.GetInstance().Update(obj))
                {
                    _nombreOriginal = obj.Nombre;
                    _unidadOriginal = obj.UnidadMedida;
                    _precioOriginal = obj.PrecioUnidad;
                    _usoOriginal = obj.UsoPorM2;

                    btnModificar.Enabled = false;
                    CargarDatos(seleccionarId: id);

                    MessageBox.Show(
                        param.GetLocalizable("material_update_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("material_update_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeshabilitar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMaterial.CurrentRow == null || dgvMaterial.CurrentRow.DataBoundItem == null)
                {
                    MessageBox.Show(
                        param.GetLocalizable("material_select_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var item = dgvMaterial.CurrentRow.DataBoundItem as BE.Material;
                if (item == null) return;

                var id = item.IdMaterial;
                var estadoActual = item.Deshabilitado;

                MaterialBLL.GetInstance().Deshabilitar(id, !estadoActual);

                CargarDatos(seleccionarId: id);

                MessageBox.Show(
                    estadoActual
                        ? param.GetLocalizable("material_enable_success_message")
                        : param.GetLocalizable("material_disable_success_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("material_toggle_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AjustarTextoBotonToggle(BE.Material item = null)
        {
            if (btnDeshabilitar == null) return;

            if (item == null)
                item = dgvMaterial.CurrentRow != null ? dgvMaterial.CurrentRow.DataBoundItem as BE.Material : null;

            if (item == null)
            {
                btnDeshabilitar.Text = param.GetLocalizable("material_disable_button");
                btnDeshabilitar.Enabled = false;
                return;
            }

            btnDeshabilitar.Enabled = true;
            btnDeshabilitar.Text = item.Deshabilitado
                ? param.GetLocalizable("material_enable_button")
                : param.GetLocalizable("material_disable_button");
        }
    }
}

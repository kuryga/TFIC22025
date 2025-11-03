using System;
using System.Linq;
using System.Windows.Forms;

using ServicioAdicionalBLL = BLL.Genericos.ServicioAdicionalBLL;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class GestionarServicioAdicionalForm : BaseForm
    {
        private string _descOriginal = string.Empty;
        private decimal _precioOriginal = 0m;
        private bool _cargandoFila = false;
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GestionarServicioAdicionalForm()
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
            dgvServicios.AutoGenerateColumns = false;
            dgvServicios.Columns.Clear();

            var colId = new DataGridViewTextBoxColumn
            {
                Name = "colId",
                HeaderText = param.GetLocalizable("servicio_id_label"),
                DataPropertyName = "IdServicio",
                Width = 70,
                ReadOnly = true
            };
            var colDesc = new DataGridViewTextBoxColumn
            {
                Name = "colDesc",
                HeaderText = param.GetLocalizable("servicio_description_label"),
                DataPropertyName = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true
            };
            var colPrecio = new DataGridViewTextBoxColumn
            {
                Name = "colPrecio",
                HeaderText = param.GetLocalizable("servicio_price_label"),
                DataPropertyName = "Precio",
                Width = 100,
                ReadOnly = true
            };
            colPrecio.DefaultCellStyle.Format = "N2";

            var colDeshab = new DataGridViewCheckBoxColumn
            {
                Name = "colDeshab",
                HeaderText = param.GetLocalizable("servicio_disabled_label"),
                DataPropertyName = "Deshabilitado",
                Width = 120,
                ReadOnly = true
            };

            dgvServicios.Columns.AddRange(colId, colDesc, colPrecio, colDeshab);
            dgvServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServicios.MultiSelect = false;
            dgvServicios.AllowUserToAddRows = false;
            dgvServicios.AllowUserToDeleteRows = false;
            dgvServicios.ReadOnly = true;
        }

        private void HookEventos()
        {
            dgvServicios.SelectionChanged += dgvServicios_SelectionChanged;
            txtDescripcion.TextChanged += txtDescripcion_TextChanged;
            txtPrecio.TextChanged += txtPrecio_TextChanged;

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("servicio_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("servicio_modify_button");
        }

        private void UpdateTexts()
        {
            txtDescripcion.Tag = TextBoxTag.SqlSafe;
            txtPrecio.Tag = TextBoxTag.Price;
            txtId.ReadOnly = true;
            lblTitle.Text = param.GetLocalizable("servicio_title");

            if (dgvServicios.Columns["colId"] != null)
                dgvServicios.Columns["colId"].HeaderText = param.GetLocalizable("servicio_id_label");
            if (dgvServicios.Columns["colDesc"] != null)
                dgvServicios.Columns["colDesc"].HeaderText = param.GetLocalizable("servicio_description_label");
            if (dgvServicios.Columns["colPrecio"] != null)
                dgvServicios.Columns["colPrecio"].HeaderText = param.GetLocalizable("servicio_price_label");
            if (dgvServicios.Columns["colDeshab"] != null)
                dgvServicios.Columns["colDeshab"].HeaderText = param.GetLocalizable("servicio_disabled_label");

            if (btnCrear != null) btnCrear.Text = param.GetLocalizable("servicio_create_button");
            if (btnModificar != null) btnModificar.Text = param.GetLocalizable("servicio_modify_button");

            AjustarTextoBotonToggle();
        }

        private void CargarDatos(int? seleccionarId = null)
        {
            dgvServicios.DataSource = null;
            var lista = ServicioAdicionalBLL.GetInstance().GetAll();
            dgvServicios.DataSource = lista;

            if (seleccionarId.HasValue && lista != null)
            {
                var row = lista
                    .Select((v, i) => new { v, i })
                    .FirstOrDefault(x => x.v.IdServicio == seleccionarId.Value);
                if (row != null)
                {
                    dgvServicios.ClearSelection();
                    dgvServicios.Rows[row.i].Selected = true;
                    dgvServicios.CurrentCell = dgvServicios.Rows[row.i].Cells[0];
                }
            }

            AjustarTextoBotonToggle();
        }

        private void dgvServicios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow == null || dgvServicios.CurrentRow.DataBoundItem == null) return;

            _cargandoFila = true;

            var item = dgvServicios.CurrentRow.DataBoundItem as BE.ServicioAdicional;
            if (item == null) { _cargandoFila = false; return; }

            txtId.Text = item.IdServicio.ToString();
            txtDescripcion.Text = item.Descripcion ?? string.Empty;
            txtPrecio.Text = item.Precio.ToString("0.00");

            _descOriginal = txtDescripcion.Text;
            _precioOriginal = item.Precio;

            btnModificar.Enabled = false;
            _cargandoFila = false;

            AjustarTextoBotonToggle(item);
        }

        private void AjustarTextoBotonToggle(BE.ServicioAdicional item = null)
        {
            if (btnDeshabilitar == null) return;

            if (item == null)
                item = dgvServicios.CurrentRow != null ? dgvServicios.CurrentRow.DataBoundItem as BE.ServicioAdicional : null;

            if (item == null)
            {
                btnDeshabilitar.Text = param.GetLocalizable("servicio_disable_button");
                btnDeshabilitar.Enabled = false;
                return;
            }

            btnDeshabilitar.Enabled = true;
            btnDeshabilitar.Text = item.Deshabilitado
                ? param.GetLocalizable("servicio_enable_button")
                : param.GetLocalizable("servicio_disable_button");
        }

        private void txtDescripcion_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFila) return;
            VerificarCambios();
        }

        private void txtPrecio_TextChanged(object sender, EventArgs e)
        {
            if (_cargandoFila) return;
            VerificarCambios();
        }

        private void VerificarCambios()
        {
            bool descCambio = !string.Equals(txtDescripcion.Text ?? string.Empty, _descOriginal ?? string.Empty, StringComparison.Ordinal);

            decimal nuevoPrecio;
            bool precioCambio = decimal.TryParse(txtPrecio.Text, out nuevoPrecio) && nuevoPrecio != _precioOriginal;

            btnModificar.Enabled = descCambio || precioCambio;
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear servicio adicional pendiente de implementación");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                int id;
                if (!int.TryParse(txtId.Text, out id) || id <= 0)
                {
                    MessageBox.Show(
                        param.GetLocalizable("servicio_select_valid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                decimal precio;
                if (!decimal.TryParse(txtPrecio.Text, out precio))
                {
                    MessageBox.Show(
                        param.GetLocalizable("servicio_price_invalid_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var obj = new BE.ServicioAdicional
                {
                    IdServicio = id,
                    Descripcion = txtDescripcion.Text != null ? txtDescripcion.Text.Trim() : string.Empty,
                    Precio = precio
                };

                if (ServicioAdicionalBLL.GetInstance().Update(obj))
                {
                    _descOriginal = obj.Descripcion;
                    _precioOriginal = obj.Precio;
                    btnModificar.Enabled = false;
                    CargarDatos(seleccionarId: id);

                    MessageBox.Show(
                        param.GetLocalizable("servicio_update_success_message"),
                        param.GetLocalizable("info_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("servicio_update_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvServicios.CurrentRow == null || dgvServicios.CurrentRow.DataBoundItem == null)
                {
                    MessageBox.Show(
                        param.GetLocalizable("servicio_select_required_message"),
                        param.GetLocalizable("notice_title"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var item = dgvServicios.CurrentRow.DataBoundItem as BE.ServicioAdicional;
                if (item == null) return;

                var id = item.IdServicio;
                var estadoActual = item.Deshabilitado;

                ServicioAdicionalBLL.GetInstance().Deshabilitar(id, !estadoActual);

                CargarDatos(seleccionarId: id);

                MessageBox.Show(
                    estadoActual
                        ? param.GetLocalizable("servicio_enable_success_message")
                        : param.GetLocalizable("servicio_disable_success_message"),
                    param.GetLocalizable("info_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("servicio_toggle_error_message") + " " + ex.Message,
                    param.GetLocalizable("error_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

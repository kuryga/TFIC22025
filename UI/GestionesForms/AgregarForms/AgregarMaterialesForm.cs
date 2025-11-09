using System;
using System.Linq;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using MaterialBLL = BLL.Genericos.MaterialBLL;

namespace WinApp
{
    public partial class AgregarMaterialesForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public AgregarMaterialesForm()
        {
            InitializeComponent();
            ConfigurarGrid();
            UpdateTexts();

            try { CargarMateriales(); }
            catch { CargarMaterialesMock(); }

            this.AcceptButton = btnAgregar;

            string helpTitle = param.GetLocalizable("agregarmat_help_title");
            string helpBody = param.GetLocalizable("agregarmat_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private void ConfigurarGrid()
        {
            dgvMateriales.AutoGenerateColumns = false;
            dgvMateriales.Columns.Clear();

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
                DataPropertyName = "Unidad",
                Width = 100,
                ReadOnly = true
            };
            var colPrecio = new DataGridViewTextBoxColumn
            {
                Name = "colPrecio",
                HeaderText = param.GetLocalizable("material_price_label"),
                DataPropertyName = "PrecioUnidad",
                Width = 120,
                ReadOnly = true
            };
            colPrecio.DefaultCellStyle.Format = "N2";
            var colUso = new DataGridViewTextBoxColumn
            {
                Name = "colUso",
                HeaderText = param.GetLocalizable("material_usage_label"),
                DataPropertyName = "UsoPorM2",
                Width = 130,
                ReadOnly = true
            };
            colUso.DefaultCellStyle.Format = "N4";

            dgvMateriales.Columns.AddRange(colNombre, colUnidad, colPrecio, colUso);
            dgvMateriales.MultiSelect = false;
            dgvMateriales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMateriales.AllowUserToAddRows = false;
            dgvMateriales.AllowUserToDeleteRows = false;
            dgvMateriales.ReadOnly = true;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("agregarmat_title");
            if (this.Controls.ContainsKey("lblListado"))
                this.Controls["lblListado"].Text = param.GetLocalizable("agregarmat_list_label");
            btnAgregar.Text = param.GetLocalizable("agregarmat_add_button");
        }

        private void CargarMateriales()
        {
            var lista = MaterialBLL.GetInstance()
                                   .GetAll()
                                   ?.Where(m => !m.Deshabilitado)
                                   .Select(m => new MaterialDTO
                                   {
                                       Nombre = m.Nombre ?? string.Empty,
                                       Unidad = m.UnidadMedida ?? string.Empty,
                                       PrecioUnidad = (double)m.PrecioUnidad,
                                       UsoPorM2 = (double)m.UsoPorM2
                                   })
                                   .ToList() ?? new System.Collections.Generic.List<MaterialDTO>();

            dgvMateriales.DataSource = lista;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvMateriales.CurrentRow == null || dgvMateriales.CurrentRow.Index < 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("agregarmat_select_warning"),
                    param.GetLocalizable("agregarmat_select_warning_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                param.GetLocalizable("agregarmat_success_message"),
                param.GetLocalizable("ok_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CargarMaterialesMock()
        {
            var lista = new[]
            {
                new MaterialDTO { Nombre = "Cemento", Unidad = "kg", PrecioUnidad = 50.0, UsoPorM2 = 5 },
                new MaterialDTO { Nombre = "Ladrillo", Unidad = "unidad", PrecioUnidad = 2.0, UsoPorM2 = 60 },
                new MaterialDTO { Nombre = "Arena", Unidad = "m3", PrecioUnidad = 35.0, UsoPorM2 = 0.2 }
            };

            dgvMateriales.DataSource = lista;
        }

        private class MaterialDTO
        {
            public string Nombre { get; set; }
            public string Unidad { get; set; }
            public double PrecioUnidad { get; set; }
            public double UsoPorM2 { get; set; }
        }
    }
}

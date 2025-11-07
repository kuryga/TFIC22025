using System;
using System.Windows.Forms;
using System.Linq;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using MaquinariaBLL = BLL.Genericos.MaquinariaBLL;

namespace WinApp
{
    public partial class AgregarMaquinariasForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public AgregarMaquinariasForm()
        {
            InitializeComponent();
            ConfigurarGrid();
            UpdateTexts();

            try { CargarMaquinarias(); }
            catch { CargarMaquinariasMock(); }

            this.AcceptButton = btnAgregar;

            string helpTitle = param.GetLocalizable("agregarmaq_help_title");
            string helpBody = param.GetLocalizable("agregarmaq_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private void ConfigurarGrid()
        {
            dgvMaquinarias.AutoGenerateColumns = false;
            dgvMaquinarias.Columns.Clear();

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
                Width = 140,
                ReadOnly = true
            };
            colCosto.DefaultCellStyle.Format = "N2";

            dgvMaquinarias.Columns.AddRange(colNombre, colCosto);
            dgvMaquinarias.MultiSelect = false;
            dgvMaquinarias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaquinarias.AllowUserToAddRows = false;
            dgvMaquinarias.AllowUserToDeleteRows = false;
            dgvMaquinarias.ReadOnly = true;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("agregarmaq_title");
            if (this.Controls.ContainsKey("lblListado"))
                this.Controls["lblListado"].Text = param.GetLocalizable("agregarmaq_list_label");
            btnAgregar.Text = param.GetLocalizable("agregarmaq_add_button");
        }

        private void CargarMaquinarias()
        {
            var lista = MaquinariaBLL.GetInstance()
                                     .GetAll()
                                     ?.Where(m => !m.Deshabilitado)
                                     .Select(m => new MaquinariaDTO
                                     {
                                         Nombre = m.Nombre ?? string.Empty,
                                         CostoPorHora = (double)m.CostoPorHora
                                     })
                                     .ToList() ?? new System.Collections.Generic.List<MaquinariaDTO>();

            dgvMaquinarias.DataSource = lista;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvMaquinarias.CurrentRow == null || dgvMaquinarias.CurrentRow.Index < 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("agregarmaq_select_warning"),
                    param.GetLocalizable("agregarmaq_select_warning_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                param.GetLocalizable("agregarmaq_success_message"),
                param.GetLocalizable("ok_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CargarMaquinariasMock()
        {
            var lista = new[]
            {
            new MaquinariaDTO { Nombre = "Excavadora", CostoPorHora = 100.0 },
            new MaquinariaDTO { Nombre = "Grua",       CostoPorHora = 150.0 },
            new MaquinariaDTO { Nombre = "Mezcladora", CostoPorHora = 80.0  }
        };
            dgvMaquinarias.DataSource = lista;
        }

        private class MaquinariaDTO
        {
            public string Nombre { get; set; }
            public double CostoPorHora { get; set; }
        }
    }

}

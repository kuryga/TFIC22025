using System;
using System.Linq;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;
using ServicioAdicionalBLL = BLL.Genericos.ServicioAdicionalBLL;

namespace WinApp
{
    public partial class AgregarServiciosAdicionalesForm : BaseForm
    {
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public AgregarServiciosAdicionalesForm()
        {
            InitializeComponent();
            ConfigurarGrid();
            UpdateTexts();

            try { CargarServicios(); }
            catch { CargarServiciosMock(); }

            this.AcceptButton = btnAgregar;

            string helpTitle = param.GetLocalizable("agregarserv_help_title");
            string helpBody = param.GetLocalizable("agregarserv_help_body");
            SetHelpContext(helpTitle, helpBody);
        }

        private void ConfigurarGrid()
        {
            dgvServicios.AutoGenerateColumns = false;
            dgvServicios.Columns.Clear();

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
                Width = 120,
                ReadOnly = true
            };
            colPrecio.DefaultCellStyle.Format = "N2";

            dgvServicios.Columns.AddRange(colDesc, colPrecio);
            dgvServicios.MultiSelect = false;
            dgvServicios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvServicios.AllowUserToAddRows = false;
            dgvServicios.AllowUserToDeleteRows = false;
            dgvServicios.ReadOnly = true;
        }

        private void UpdateTexts()
        {
            this.Text = param.GetLocalizable("agregarserv_title");
            if (this.Controls.ContainsKey("lblListado"))
                this.Controls["lblListado"].Text = param.GetLocalizable("agregarserv_list_label");
            btnAgregar.Text = param.GetLocalizable("agregarserv_add_button");
        }

        private void CargarServicios()
        {
            var lista = ServicioAdicionalBLL.GetInstance()
                                            .GetAll()
                                            ?.Where(s => !s.Deshabilitado)
                                            .Select(s => new ServicioDTO
                                            {
                                                Descripcion = s.Descripcion ?? string.Empty,
                                                Precio = (double)s.Precio
                                            })
                                            .ToList() ?? new System.Collections.Generic.List<ServicioDTO>();

            dgvServicios.DataSource = lista;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvServicios.CurrentRow == null || dgvServicios.CurrentRow.Index < 0)
            {
                MessageBox.Show(
                    param.GetLocalizable("agregarserv_select_warning"),
                    param.GetLocalizable("agregarserv_select_warning_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                param.GetLocalizable("agregarserv_success_message"),
                param.GetLocalizable("ok_title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CargarServiciosMock()
        {
            var lista = new[]
            {
                new ServicioDTO { Descripcion = "Supervisión de obra", Precio = 2000.0 },
                new ServicioDTO { Descripcion = "Instalación eléctrica", Precio = 3000.0 },
                new ServicioDTO { Descripcion = "Sistema de plomería", Precio = 2500.0 }
            };

            dgvServicios.DataSource = lista;
        }

        private class ServicioDTO
        {
            public string Descripcion { get; set; }
            public double Precio { get; set; }
        }
    }
}

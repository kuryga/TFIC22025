using System;
using System.Windows.Forms;
using BLL.Audit;
using System.Collections.Generic;
using System.Linq;

using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace UI
{
    public partial class ConsultarBitacoraForm : BaseForm
    {
        // paginado
        private int _page = 1;
        private const int PageSize = 30;
        //
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public ConsultarBitacoraForm()
        {
            InitializeComponent();

            UpdateTexts();
            cmbCriticidad.DropDownStyle = ComboBoxStyle.DropDownList;

            dtpHasta.MaxDate = DateTime.Today;
            dtpHasta.Value = DateTime.Today;

            dtpDesde.MaxDate = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddDays(-1);

            btnPrev.Enabled = false;
            btnNext.Enabled = false;

            lblPageInfo.Text = $"";

            CargarCriticidades();
        }
        private void CargarCriticidades()
        {
            var crits = BitacoraBLL.GetInstance().GetCriticidades();


            var lista = new List<BE.Audit.Criticidad> { BE.Audit.Criticidad.None };
            lista.AddRange(crits);

            cmbCriticidad.DataSource = lista;
            cmbCriticidad.FormattingEnabled = true;

            cmbCriticidad.Format -= CmbCriticidad_Format;
            cmbCriticidad.Format += CmbCriticidad_Format;

            if (cmbCriticidad.Items.Count > 0)
                cmbCriticidad.SelectedIndex = 0;
        }

        private void CmbCriticidad_Format(object sender, ListControlConvertEventArgs e)
        {
            var item = (BE.Audit.Criticidad)e.ListItem;
            e.Value = (item == BE.Audit.Criticidad.None) ? param.GetLocalizable("log_criticality_all") : item.ToString();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            var desde = dtpDesde.Value.Date;
            var hasta = dtpHasta.Value.Date;

            if (desde > hasta)
            {
                MessageBox.Show(
                    param.GetLocalizable("log_invalid_date_range"),
                    param.GetLocalizable("log_invalid_date_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _page = 1;
            LoadPage();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                LoadPage();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _page++;
            LoadPage();
        }

        private void LoadPage()
        {
            DateTime? desde = dtpDesde.Value.Date;
            DateTime? hasta = dtpHasta.Value.Date;

            var criticidadSeleccionada = (BE.Audit.Criticidad)cmbCriticidad.SelectedItem;


            string crit = (criticidadSeleccionada == BE.Audit.Criticidad.None)
                ? null
                : criticidadSeleccionada.ToString();

            var result = BitacoraBLL.GetInstance().GetBitacora(desde, hasta, _page, PageSize, crit);

            if (_page > 1 && (result.Items == null || result.Items.Count == 0))
            {
                _page--;
                result = BitacoraBLL.GetInstance().GetBitacora(desde, hasta, _page, PageSize, crit);
            }

            dgvBitacora.DataSource = null;
            dgvBitacora.AutoGenerateColumns = true;

            if (_page == 1 && (result.Items == null || result.Items.Count == 0))
            {
                MessageBox.Show(
                    param.GetLocalizable("log_no_results_message"),
                    param.GetLocalizable("log_no_results_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            dgvBitacora.DataSource = result.Items;
            UpdateDvg();
            lblPageInfo.Text = $"{param.GetLocalizable("log_page_label")} {_page}";

            btnPrev.Enabled = (_page > 1);
            btnNext.Enabled = (result.Items != null && result.Items.Count == PageSize);
        }

        private void UpdateTexts()
        {
            lblCriticidad.Text = param.GetLocalizable("log_criticality_label");
            lblDesde.Text = param.GetLocalizable("log_date_from_label");
            lblHasta.Text = param.GetLocalizable("log_date_to_label");

            btnConsultar.Text = param.GetLocalizable("log_search_button");
        }
        
        private void UpdateDvg()
        {
            dgvBitacora.Columns["IdRegistro"].HeaderText = param.GetLocalizable("log_col_id");
            dgvBitacora.Columns["Fecha"].HeaderText = param.GetLocalizable("log_col_date");
            dgvBitacora.Columns["Accion"].HeaderText = param.GetLocalizable("log_col_action");
            dgvBitacora.Columns["Criticidad"].HeaderText = param.GetLocalizable("log_col_criticality");
            dgvBitacora.Columns["Mensaje"].HeaderText = param.GetLocalizable("log_col_message");
            dgvBitacora.Columns["IdEjecutor"].HeaderText = param.GetLocalizable("log_col_executor_id");
            dgvBitacora.Columns["UsuarioEjecutor"].HeaderText = param.GetLocalizable("log_col_executor_user");
        }
    }
}

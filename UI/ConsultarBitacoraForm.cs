using System;
using System.Windows.Forms;
using BLL.Audit;
using BitacoraDTO = BE.Audit.Bitacora;

namespace UI
{
    public partial class ConsultarBitacoraForm : Form
    {
        // paginado
        private int _page = 1;
        private const int PageSize = 30;
        //

        public ConsultarBitacoraForm()
        {
            InitializeComponent();

            dtpHasta.MaxDate = DateTime.Today;
            dtpHasta.Value = DateTime.Today;

            dtpDesde.MaxDate = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddDays(-1);

            btnPrev.Click += btnPrev_Click;
            btnNext.Click += btnNext_Click;

            btnPrev.Enabled = false;
            btnNext.Enabled = false;

            lblPageInfo.Text = $"";
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            var desde = dtpDesde.Value.Date;
            var hasta = dtpHasta.Value.Date;

            if (desde > hasta)
            {
                MessageBox.Show(
                    "Rango de fechas inválido. La fecha de inicio debe ser anterior a la fecha de fin.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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

            var result = BitacoraBLL.GetInstance().GetBitacora(desde, hasta, _page, PageSize);

            if (_page > 1 && (result.Items == null || result.Items.Count == 0))
            {
                _page--;
                result = BitacoraBLL.GetInstance().GetBitacora(desde, hasta, _page, PageSize);
            }

            dgvBitacora.DataSource = null;
            dgvBitacora.AutoGenerateColumns = true;
            dgvBitacora.DataSource = result.Items;

            lblPageInfo.Text = $"Página {_page}";

            btnPrev.Enabled = (_page > 1);
            btnNext.Enabled = (result.Items != null && result.Items.Count == PageSize);

            if (_page == 1 && (result.Items == null || result.Items.Count == 0))
            {
                MessageBox.Show(
                    "No se encontraron eventos en el período seleccionado.",
                    "Sin resultados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}

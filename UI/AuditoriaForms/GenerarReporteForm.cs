using System;
using System.IO;
using System.Windows.Forms;
using BLL.Audit;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp.AuditoriaForms
{
    public partial class GenerarReporteForm : Form
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public string Criticidad { get; set; }

        private bool _sinFiltrosPrevios;
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public GenerarReporteForm()
        {
            InitializeComponent();
            this.Text = param.GetLocalizable("report_generation_title");
            Load += GenerarReporteForm_Load;
        }

        public GenerarReporteForm(DateTime? desde, DateTime? hasta, int page, int pageSize, string criticidad)
            : this()
        {
            Desde = desde;
            Hasta = hasta;
            Page = page <= 0 ? 1 : page;
            PageSize = pageSize <= 0 ? 30 : pageSize;
            Criticidad = string.IsNullOrWhiteSpace(criticidad) ? null : criticidad;
            _sinFiltrosPrevios = !desde.HasValue && !hasta.HasValue && string.IsNullOrWhiteSpace(criticidad);
        }

        private void GenerarReporteForm_Load(object sender, EventArgs e)
        {
            UpdateTexts();

            var cbo = Controls["cboDestino"] as ComboBox;
            var chk = Controls["chkPaginaAct"] as CheckBox;

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (cbo != null)
            {
                cbo.DropDownStyle = ComboBoxStyle.DropDownList;
                cbo.Items.Clear();
                if (!string.IsNullOrWhiteSpace(desktop)) cbo.Items.Add(desktop);
                if (!string.IsNullOrWhiteSpace(docs)) cbo.Items.Add(docs);
                cbo.TextChanged += (s, ev) => UpdateButtonState();
                cbo.SelectedIndexChanged += (s, ev) => UpdateButtonState();
            }

            if (chk != null)
            {
                chk.Checked = true;
                chk.Visible = !_sinFiltrosPrevios;
            }

            btnCarpeta.Click += BtnExplorar_Click;
            btnGenerar.Click += BtnGenerar_Click;
            btnGenerar.Enabled = false;

            UpdateButtonState();
        }

        private void UpdateTexts()
        {
            lblUnidad.Text = param.GetLocalizable("report_destination_label");
            chkPaginaAct.Text = param.GetLocalizable("current_page_only_label");
            btnCarpeta.Text = param.GetLocalizable("select_folder_button");
            btnGenerar.Text = param.GetLocalizable("report_generate_button");
        }

        private void BtnExplorar_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = param.GetLocalizable("report_destination_dialog_description");
                dlg.ShowNewFolderButton = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var cbo = Controls["cboDestino"] as ComboBox;
                    if (cbo != null)
                    {
                        if (!cbo.Items.Contains(dlg.SelectedPath))
                            cbo.Items.Add(dlg.SelectedPath);
                        cbo.SelectedItem = dlg.SelectedPath;
                        UpdateButtonState();
                    }
                }
            }
        }

        private void BtnGenerar_Click(object sender, EventArgs e)
        {
            var cbo = Controls["cboDestino"] as ComboBox;
            var chk = Controls["chkPaginaAct"] as CheckBox;

            string destino = cbo != null ? cbo.Text?.Trim() : null;

            if (string.IsNullOrWhiteSpace(destino))
            {
                MessageBox.Show(
                    this,
                    param.GetLocalizable("report_destination_required_message"),
                    param.GetLocalizable("attention_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            bool exportarTodos = _sinFiltrosPrevios || !(chk?.Checked ?? true);

            int? page = exportarTodos ? (int?)null : Math.Max(1, Page);
            int? pageSize = exportarTodos ? (int?)null : Math.Max(1, PageSize);

            try
            {
                string ruta = BitacoraBLL.GetInstance().ExportarReporte(
                    Desde,
                    Hasta,
                    page,
                    pageSize,
                    Criticidad,
                    destino,
                    exportarTodos
                );

                if (!string.IsNullOrWhiteSpace(ruta) && File.Exists(ruta))
                {
                    MessageBox.Show(
                        this,
                        param.GetLocalizable("report_generated_success_message") + Environment.NewLine + ruta,
                        param.GetLocalizable("report_generated_success_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(
                        this,
                        param.GetLocalizable("report_generated_missing_file_message"),
                        param.GetLocalizable("report_generated_missing_file_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    param.GetLocalizable("report_generated_error_message") + Environment.NewLine + ex.Message,
                    param.GetLocalizable("report_generated_error_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateButtonState()
        {
            var cbo = Controls["cboDestino"] as ComboBox;
            var btn = Controls["btnGenerar"] as Button;
            if (btn == null || cbo == null) return;

            bool tieneDestino = !string.IsNullOrWhiteSpace(cbo.Text);
            btn.Enabled = tieneDestino;
        }
    }
}

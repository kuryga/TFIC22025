using System;
using System.IO;
using System.Windows.Forms;
using BLL.Audit;

namespace UI.AuditoriaForms
{
    public partial class GenerarReporteForm : Form
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 30;
        public string Criticidad { get; set; }

        private bool _sinFiltrosPrevios;

        public GenerarReporteForm()
        {
            InitializeComponent();
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

        void GenerarReporteForm_Load(object sender, EventArgs e)
        {
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
                cbo.SelectedIndexChanged += (s, ev) => UpdateButtonState(); // habilita también al seleccionar
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

        void BtnExplorar_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Selecciona la carpeta de destino";
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

        void BtnGenerar_Click(object sender, EventArgs e)
        {
            var cbo = Controls["cboDestino"] as ComboBox;
            var chk = Controls["chkPaginaAct"] as CheckBox;

            string destino = cbo != null ? cbo.Text?.Trim() : null;

            if (string.IsNullOrWhiteSpace(destino))
            {
                MessageBox.Show(this, "Debe seleccionar o escribir una carpeta de destino antes de generar el reporte.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show(this, "Reporte generado correctamente:\n" + ruta, "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(this, "No se pudo verificar el archivo generado.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error al generar el reporte:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void UpdateButtonState()
        {
            var cbo = Controls["cboDestino"] as ComboBox;
            var btn = Controls["btnGenerar"] as Button;
            if (btn == null || cbo == null) return;
            bool tieneDestino = !string.IsNullOrWhiteSpace(cbo.Text);
            btn.Enabled = tieneDestino;
        }
    }
}

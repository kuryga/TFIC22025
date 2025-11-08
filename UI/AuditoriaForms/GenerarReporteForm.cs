using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Utils.Reporting;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp.AuditoriaForms
{
    public partial class GenerarReporteForm : BaseForm
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
            this.Text = param.GetLocalizable("report_generation_title") ?? "Generación de reportes";
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
        }

        private void UpdateTexts()
        {
            chkPaginaAct.Text = param.GetLocalizable("current_page_only_label") ?? "Solo página actual";
            btnGenerar.Text = param.GetLocalizable("report_generate_button") ?? "Imprimir";
            SetHelpContext(
                param.GetLocalizable("report_help_title"),
                param.GetLocalizable("report_help_body")
            );
        }

        private void ImprimirPdfGenerico(string rutaPdf, string printerName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = rutaPdf,
                    UseShellExecute = true,
                    Verb = "printto",
                    Arguments = $"\"{printerName}\""
                };

                try
                {
                    Process.Start(psi);
                    return;
                }
                catch
                {
                    // fallback al print normal
                }

                var psiFallback = new ProcessStartInfo
                {
                    FileName = rutaPdf,
                    UseShellExecute = true,
                    Verb = "print"
                };
                Process.Start(psiFallback);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    (param.GetLocalizable("print_send_error_message") ?? "No fue posible enviar el PDF a impresión.")
                        + Environment.NewLine + ex.Message,
                    param.GetLocalizable("print_error_title") ?? "Error de impresión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ProgramarBorradoTemporal(string ruta)
        {
            try
            {
                var timer = new Timer { Interval = 5000 };
                int intentos = 0;
                timer.Tick += (s, e) =>
                {
                    try
                    {
                        intentos++;
                        if (File.Exists(ruta)) File.Delete(ruta);
                        if (!File.Exists(ruta) || intentos >= 24)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                    catch
                    {
                        if (intentos >= 24)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                };
                timer.Start();
            }
            catch
            {
                // No se requiere notificación
            }
        }

        private byte[] TryLoadLogo()
        {
            try
            {
                string raw = ConfigurationManager.AppSettings["ReportLogoPath"];
                var bases = new[]
                {
            AppDomain.CurrentDomain.BaseDirectory,
            Application.StartupPath
        };

                var candidatos = new System.Collections.Generic.List<string>();

                if (!string.IsNullOrWhiteSpace(raw))
                {
                    raw = raw.Trim('"');
                    if (Path.IsPathRooted(raw))
                    {
                        candidatos.Add(raw);
                    }
                    else
                    {
                        foreach (var b in bases)
                            candidatos.Add(Path.Combine(b, raw));
                    }
                }

                foreach (var b in bases)
                    candidatos.Add(Path.Combine(b, "Resources", "LogoUrbanSoft.png"));

                foreach (var p in candidatos.Distinct())
                {
                    if (File.Exists(p))
                        return File.ReadAllBytes(p);
                }
            }
            catch
            {
                // nada
            }

            return null;
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                this.Enabled = false;

                bool exportarTodos = _sinFiltrosPrevios || !(chkPaginaAct?.Checked ?? true);
                int? page = exportarTodos ? (int?)null : Math.Max(1, Page);
                int? pageSize = exportarTodos ? (int?)null : Math.Max(1, PageSize);
                string empresa = param.GetNombreEmpresa();
                byte[] logo = TryLoadLogo();

                string appTemp = Path.Combine(Path.GetTempPath(), Application.ProductName);
                string tempPdf = FilePDFExporter.GenerarPdfTemporal(Desde, Hasta, page, pageSize, Criticidad, exportarTodos, empresa, logo, appTemp);

                this.Cursor = Cursors.Default;
                this.Enabled = true;

                if (string.IsNullOrWhiteSpace(tempPdf) || !File.Exists(tempPdf))
                {
                    MessageBox.Show(
                        this,
                        param.GetLocalizable("print_temp_fail_message") ?? "No se pudo generar el documento temporal para imprimir.",
                        param.GetLocalizable("warning_title") ?? "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.AllowSomePages = false;
                    printDialog.ShowNetwork = true;

                    if (printDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        string printerName = printDialog.PrinterSettings.PrinterName;
                        ImprimirPdfGenerico(tempPdf, printerName);
                        ProgramarBorradoTemporal(tempPdf);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                this.Enabled = true;

                MessageBox.Show(
                    this,
                    (param.GetLocalizable("print_generic_error_message") ?? "Ocurrió un error al intentar imprimir el documento.")
                        + Environment.NewLine + ex.Message,
                    param.GetLocalizable("print_error_title") ?? "Error de impresión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}

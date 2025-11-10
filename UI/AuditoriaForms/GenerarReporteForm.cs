using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
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

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string pszPrinter);

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);

        private static string GetDefaultPrinterName()
        {
            int size = 0;
            GetDefaultPrinter(null, ref size);
            if (size <= 0) return null;

            var sb = new StringBuilder(size);
            return GetDefaultPrinter(sb, ref size) ? sb.ToString() : null;
        }
        private bool WaitForStablePdf(string path, int timeoutMs = 10000, int stableChecks = 2, int checkIntervalMs = 150)
        {
            var sw = Stopwatch.StartNew();
            long lastLen = -1;
            int stable = 0;

            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        System.Threading.Thread.Sleep(checkIntervalMs);
                        continue;
                    }

                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long len = fs.Length;

                        if (len > 0 && len == lastLen)
                        {
                            stable++;
                            if (stable >= stableChecks)
                                return true;
                        }
                        else
                        {
                            stable = 0;
                        }

                        lastLen = len;
                    }
                }
                catch
                {
                    // Archivo aún en uso
                }

                System.Threading.Thread.Sleep(checkIntervalMs);
            }

            return false;
        }

        private string ShadowCopyStablePdf(string sourcePath)
        {
            var dir = Path.GetDirectoryName(sourcePath) ?? Path.GetTempPath();
            var name = Path.GetFileNameWithoutExtension(sourcePath);
            var ext = Path.GetExtension(sourcePath);
            var shadow = Path.Combine(dir, $"{name}.printcopy.{Guid.NewGuid():N}{ext}");

            File.Copy(sourcePath, shadow, true);

            using (var fs = new FileStream(shadow, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length == 0) throw new IOException("La copia del PDF quedó con tamaño 0.");
            }

            return shadow;
        }

        private void ImprimirPdfConDefaultTemporal(string rutaPdf, string targetPrinter)
        {
            string originalDefault = GetDefaultPrinterName();

            try
            {
                if (!string.IsNullOrEmpty(targetPrinter) && !SetDefaultPrinter(targetPrinter))
                    throw new InvalidOperationException("No se pudo establecer la impresora seleccionada como predeterminada.");

                var psi = new ProcessStartInfo
                {
                    FileName = rutaPdf,
                    UseShellExecute = true,
                    Verb = "print"
                };

                System.Threading.Thread.Sleep(200);

                using (var proc = Process.Start(psi))
                {
                    System.Threading.Thread.Sleep(1200);
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(originalDefault))
                    SetDefaultPrinter(originalDefault);
            }
        }

        private void ProgramarBorradoTemporal(string ruta)
        {
            try
            {
                var timer = new System.Windows.Forms.Timer { Interval = 5000 };
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
                // nada
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

                if (!WaitForStablePdf(tempPdf))
                {
                    MessageBox.Show(
                        this,
                        param.GetLocalizable("print_temp_not_ready_message") ?? "El documento aún se está generando. Intente nuevamente.",
                        param.GetLocalizable("warning_title") ?? "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                string printPdf = ShadowCopyStablePdf(tempPdf);

                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.AllowSomePages = false;
                    printDialog.ShowNetwork = true;

                    if (printDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        string printerName = printDialog.PrinterSettings.PrinterName;

                        ImprimirPdfConDefaultTemporal(printPdf, printerName);

                        ProgramarBorradoTemporal(printPdf);
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

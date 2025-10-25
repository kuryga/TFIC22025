using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL.Seguridad.Mantenimiento;

namespace UI
{
    public partial class RestoreForm : BaseForm
    {
        private OpenFileDialog ofd;

        public RestoreForm()
        {
            InitializeComponent();

            ofd = new OpenFileDialog
            {
                Filter = "Backups (*.bak)|*.bak|Todos (*.*)|*.*",
                Title = "Seleccionar archivos de backup",
                Multiselect = true
            };

            btnSeleccionar.Click += BtnSeleccionar_Click;
            btnRestaurar.Click += BtnRestaurar_Click;
        }

        private void BtnSeleccionar_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtArchivos.Text = string.Join(" | ", ofd.FileNames);
            }
        }

        private async void BtnRestaurar_Click(object sender, EventArgs e)
        {
            var pathsStr = txtArchivos.Text?.Trim();
            if (string.IsNullOrWhiteSpace(pathsStr))
            {
                MessageBox.Show("Seleccione al menos un archivo .bak.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var files = pathsStr
                .Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim('"').Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (files.Count == 0)
            {
                MessageBox.Show("No se detectaron archivos válidos.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var inexistentes = files.Where(f => !File.Exists(f)).ToList();
            if (inexistentes.Any())
            {
                MessageBox.Show("Los siguientes archivos no existen:\r\n" + string.Join("\r\n", inexistentes),
                    "Restore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var noBak = files.Where(f => !string.Equals(Path.GetExtension(f), ".bak", StringComparison.OrdinalIgnoreCase)).ToList();
            if (noBak.Any())
            {
                var seguir = MessageBox.Show(
                    "Se detectaron archivos sin extensión .bak.\r\n¿Desea continuar de todos modos?",
                    "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (seguir != DialogResult.Yes) return;
            }

            files = OrdenarPartes(files);

            var doVerify = chkVerify.Checked;

            var resp = MessageBox.Show(
                "Se restaurará la base de datos.\r\n" +
                "Esto sobrescribirá el estado actual.\r\n¿Desea continuar?",
                "Confirmar restore", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (resp != DialogResult.OK) return;

            btnRestaurar.Enabled = false;
            btnSeleccionar.Enabled = false;
            UseWaitCursor = true;
            txtLog.Clear();

            try
            {
                txtLog.AppendText("Iniciando restore..." + Environment.NewLine);
                txtLog.AppendText("Archivos seleccionados:" + Environment.NewLine);
                foreach (var f in files) txtLog.AppendText(" - " + f + Environment.NewLine);
                txtLog.AppendText("Opciones: VERIFY=" + (doVerify ? "SI" : "NO") + " REPLACE=" + "SI" + Environment.NewLine);

                await Task.Run(() =>
                {
                    BackupBLL.GetInstance().RestoreFull(files, withReplace: true, verifyBefore: doVerify);
                });

                txtLog.AppendText("Restore finalizado correctamente." + Environment.NewLine);
                MessageBox.Show("Restore realizado con éxito.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                txtLog.AppendText("Error: " + ex.Message + Environment.NewLine);
                MessageBox.Show("Error en el restore:" + Environment.NewLine + ex.Message, "Restore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                btnRestaurar.Enabled = true;
                btnSeleccionar.Enabled = true;
            }
        }

        private List<string> OrdenarPartes(List<string> files)
        {
            var parsed = new List<(string File, string BaseName, int Part)>();
            foreach (var f in files)
            {
                var name = Path.GetFileNameWithoutExtension(f) ?? "";
                var part = -1;
                var baseName = name;
                var idx = name.LastIndexOf("_p", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0 && idx + 2 < name.Length)
                {
                    var numStr = name.Substring(idx + 2);
                    if (int.TryParse(numStr, out var n) && n > 0)
                    {
                        part = n;
                        baseName = name.Substring(0, idx);
                    }
                }
                parsed.Add((f, baseName, part));
            }

            if (parsed.All(p => p.Part < 0)) return files;

            var bases = parsed.Select(p => p.BaseName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (bases.Count > 1)
            {
                var seguir = MessageBox.Show(
                    "Se detectaron archivos de diferentes respaldos. Verifique que todas las partes pertenezcan al mismo backup.\r\n¿Continuar de todos modos?",
                    "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (seguir != DialogResult.Yes) return files;
            }

            var ordered = parsed
                .OrderBy(p => p.Part < 0 ? int.MaxValue : p.Part)
                .Select(p => p.File)
                .ToList();

            return ordered;
        }
    }
}

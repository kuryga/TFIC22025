using BLL.Seguridad.Mantenimiento;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class RestoreForm : BaseForm
    {
        private OpenFileDialog ofd;
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public RestoreForm()
        {
            InitializeComponent();

            ofd = new OpenFileDialog
            {
                Filter = param.GetLocalizable("restore_ofd_filter"),
                Title = param.GetLocalizable("restore_ofd_title"),
                Multiselect = true
            };

            this.Load += RestoreForm_Load;
            btnSeleccionar.Click += BtnSeleccionar_Click;
            btnRestaurar.Click += BtnRestaurar_Click;

            UpdateTexts();
        }

        private void RestoreForm_Load(object sender, EventArgs e)
        {
            this.Text = param.GetLocalizable("restore_title");
        }

        private void BtnSeleccionar_Click(object sender, EventArgs e)
        {
            ofd.Filter = param.GetLocalizable("restore_ofd_filter");
            ofd.Title = param.GetLocalizable("restore_ofd_title");

            if (ofd.ShowDialog(this) == DialogResult.OK)
                txtArchivos.Text = string.Join(" | ", ofd.FileNames);
        }

        private async void BtnRestaurar_Click(object sender, EventArgs e)
        {
            var pathsStr = txtArchivos.Text?.Trim();
            if (string.IsNullOrWhiteSpace(pathsStr))
            {
                MessageBox.Show(
                    param.GetLocalizable("restore_select_bak_message"),
                    param.GetLocalizable("restore_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(
                    param.GetLocalizable("restore_no_valid_files_message"),
                    param.GetLocalizable("restore_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var inexistentes = files.Where(f => !File.Exists(f)).ToList();
            if (inexistentes.Any())
            {
                MessageBox.Show(
                    param.GetLocalizable("restore_missing_files_prefix") + Environment.NewLine + string.Join(Environment.NewLine, inexistentes),
                    param.GetLocalizable("restore_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var noBak = files.Where(f => !string.Equals(Path.GetExtension(f), ".bak", StringComparison.OrdinalIgnoreCase)).ToList();
            if (noBak.Any())
            {
                var seguir = MessageBox.Show(
                    param.GetLocalizable("restore_non_bak_warning_message"),
                    param.GetLocalizable("warning_title"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (seguir != DialogResult.Yes) return;
            }

            files = OrdenarPartes(files);
            var doVerify = chkVerify.Checked;

            var resp = MessageBox.Show(
                param.GetLocalizable("restore_confirm_message"),
                param.GetLocalizable("restore_confirm_title"),
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (resp != DialogResult.OK) return;

            ToggleBusy(true);
            txtLog.Clear();

            try
            {
                txtLog.AppendText(param.GetLocalizable("restore_starting_message") + Environment.NewLine);
                txtLog.AppendText(param.GetLocalizable("restore_selected_files_header") + Environment.NewLine);
                foreach (var f in files)
                    txtLog.AppendText(" - " + f + Environment.NewLine);

                var verifyYesNo = doVerify ? param.GetLocalizable("yes_label") : param.GetLocalizable("no_label");
                var replaceYesNo = param.GetLocalizable("yes_label"); // siempre con REPLACE
                txtLog.AppendText(
                    param.GetLocalizable("restore_options_prefix")
                    + " VERIFY=" + verifyYesNo
                    + " REPLACE=" + replaceYesNo + Environment.NewLine);

                await Task.Run(() =>
                    BackupBLL.GetInstance().RestoreFull(files, withReplace: true, verifyBefore: doVerify)
                );

                txtLog.AppendText(param.GetLocalizable("restore_finished_message") + Environment.NewLine);

                MessageBox.Show(
                    param.GetLocalizable("restore_success_message"),
                    param.GetLocalizable("restore_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                txtLog.AppendText(param.GetLocalizable("restore_error_prefix_message") + " " + ex.Message + Environment.NewLine);

                MessageBox.Show(
                    param.GetLocalizable("restore_error_prefix_message") + Environment.NewLine + ex.Message,
                    param.GetLocalizable("restore_title"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void ToggleBusy(bool busy)
        {
            btnRestaurar.Enabled = !busy;
            btnSeleccionar.Enabled = !busy;
            UseWaitCursor = busy;
            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;
            Application.DoEvents();
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
                    param.GetLocalizable("restore_mixed_sets_warning_message"),
                    param.GetLocalizable("warning_title"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (seguir != DialogResult.Yes) return files;
            }

            return parsed
                .OrderBy(p => p.Part < 0 ? int.MaxValue : p.Part)
                .Select(p => p.File)
                .ToList();
        }
        private void UpdateTexts()
        {
            if (this.Controls.ContainsKey("lblArchivos"))
                this.Controls["lblArchivos"].Text = param.GetLocalizable("restore_files_label");

            btnSeleccionar.Text = param.GetLocalizable("restore_browse_button");
            btnRestaurar.Text = param.GetLocalizable("restore_execute_button");

            chkVerify.Text = param.GetLocalizable("restore_verify_checkbox");

            string helpTitle = param.GetLocalizable("restore_help_title");
            string helpBody = param.GetLocalizable("restore_help_body");
            SetHelpContext(helpTitle, helpBody);
        }
    }
}

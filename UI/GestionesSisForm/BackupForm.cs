using BLL.Seguridad.Mantenimiento;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParametrizacionBLL = BLL.Genericos.ParametrizacionBLL;

namespace WinApp
{
    public partial class BackupForm : BaseForm
    {
        private FolderBrowserDialog fbd;
        private readonly ParametrizacionBLL param = ParametrizacionBLL.GetInstance();

        public BackupForm()
        {
            InitializeComponent();
            fbd = new FolderBrowserDialog();

            this.Load += BackupForm_Load;
            btnCarpeta.Click += BtnCarpeta_Click;
            btnBackup.Click += BtnBackup_Click;
            cboDestino.SelectedIndexChanged += CboDestino_SelectedIndexChanged;

            var tb = nudPartes.Controls[1] as TextBox;
            if (tb != null) tb.ReadOnly = true;

            UpdateTexts();
        }

        private void BackupForm_Load(object sender, EventArgs e)
        {
            CargarUnidades();
            btnBackup.Enabled = false;
        }

        private void CargarUnidades()
        {
            cboDestino.Items.Clear();

            var drives = DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Removable)
                .Select(d => d.Name.TrimEnd('\\'))
                .ToList();

            if (drives.Count == 0) drives.Add("C:");

            foreach (var d in drives) cboDestino.Items.Add(d);

            cboDestino.Items.Add(param.GetLocalizable("backup_choose_custom_folder_option"));
        }

        private void BtnCarpeta_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog(this) == DialogResult.OK)
            {
                var path = fbd.SelectedPath;
                if (!cboDestino.Items.Contains(path))
                    cboDestino.Items.Insert(0, path);
                cboDestino.SelectedItem = path;
            }
        }

        private async void BtnBackup_Click(object sender, EventArgs e)
        {
            ToggleBusy(true);
            txtResultado.Clear();

            try
            {
                var selected = cboDestino.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(selected))
                {
                    MessageBox.Show(
                        param.GetLocalizable("backup_no_destination_selected"),
                        param.GetLocalizable("backup_title"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (selected == param.GetLocalizable("backup_choose_custom_folder_option"))
                {
                    BtnCarpeta_Click(sender, e);
                    selected = cboDestino.SelectedItem?.ToString();
                }

                var destino = selected;
                var partes = (int)nudPartes.Value;

                txtResultado.AppendText(param.GetLocalizable("backup_starting_message") + Environment.NewLine);

                var files = await Task.Run(() =>
                    BackupBLL.GetInstance().BackupFull(destino, partes)
                );

                txtResultado.AppendText(param.GetLocalizable("backup_finished_message") + Environment.NewLine);
                var prefix = param.GetLocalizable("backup_generated_files_prefix");
                foreach (var f in files)
                    txtResultado.AppendText($"{prefix} {f}{Environment.NewLine}");

                MessageBox.Show(
                    param.GetLocalizable("backup_success_message"),
                    param.GetLocalizable("backup_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    param.GetLocalizable("backup_error_prefix_message") + Environment.NewLine + ex.Message,
                    param.GetLocalizable("backup_title"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                ToggleBusy(false);
            }
        }

        private void CboDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = cboDestino.SelectedItem?.ToString();
            btnBackup.Enabled = !string.IsNullOrWhiteSpace(selected) &&
                                selected != param.GetLocalizable("backup_choose_custom_folder_option");
        }

        private void ToggleBusy(bool busy)
        {
            btnBackup.Enabled = !busy && !string.IsNullOrWhiteSpace(cboDestino.SelectedItem?.ToString());
            btnCarpeta.Enabled = !busy;
            cboDestino.Enabled = !busy;
            nudPartes.Enabled = !busy;
            UseWaitCursor = busy;
            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;
            Application.DoEvents();
        }

        private void UpdateTexts()
        {
            lblUnidad.Text = param.GetLocalizable("backup_destination_label");
            lblPartes.Text = param.GetLocalizable("backup_parts_label");

            btnCarpeta.Text = param.GetLocalizable("backup_browse_button");
            btnBackup.Text = param.GetLocalizable("backup_execute_button");

            this.Text = param.GetLocalizable("backup_title");

            string helpTitle = param.GetLocalizable("backup_help_title");
            string helpBody = param.GetLocalizable("backup_help_body");
            SetHelpContext(helpTitle, helpBody);
        }
    }
}

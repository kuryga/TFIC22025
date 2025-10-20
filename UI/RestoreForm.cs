// RestoreForm.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class RestoreForm : Form
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
           //btnRestaurar.Click += BtnRestaurar_Click;
        }

        private void BtnSeleccionar_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                txtArchivos.Text = string.Join(" | ", ofd.FileNames);
            }
        }

        /*
        private async void BtnRestaurar_Click(object sender, EventArgs e)
        {
            var pathsStr = txtArchivos.Text?.Trim();
            if (string.IsNullOrWhiteSpace(pathsStr))
            {
                MessageBox.Show("Seleccione al menos un archivo .bak.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var files = pathsStr.Split(new[] { " | " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            btnRestaurar.Enabled = false;
            txtLog.Clear();

            try
            {
                txtLog.AppendText("Iniciando restore..." + Environment.NewLine);
                var doVerify = chkVerify.Checked;
                var doReplace = chkReplace.Checked;

                await Task.Run(() =>
                {
                    BackupDAL.GetInstance().RestoreFull(files, withReplace: doReplace, verifyBefore: doVerify);
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
                btnRestaurar.Enabled = true;
            }
        }
        */
    }
}

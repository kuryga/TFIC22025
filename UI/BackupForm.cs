// BackupForm.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class BackupForm : Form
    {
        private FolderBrowserDialog fbd;

        public BackupForm()
        {
            InitializeComponent();
            fbd = new FolderBrowserDialog();

            this.Load += BackupForm_Load;
            btnCarpeta.Click += BtnCarpeta_Click;
            //btnBackup.Click += BtnBackup_Click;
        }

        private void BackupForm_Load(object sender, EventArgs e)
        {
            CargarUnidades();
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
            cboDestino.Items.Add("[Elegir carpeta personalizada…]");

            cboDestino.SelectedIndex = 0;
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

        /*
        private async void BtnBackup_Click(object sender, EventArgs e)
        {
            btnBackup.Enabled = false;
            txtResultado.Clear();
            try
            {
                var destino = (cboDestino.SelectedItem ?? "C:").ToString();
                var partes = (int)nudPartes.Value;

                txtResultado.AppendText("Iniciando backup..." + Environment.NewLine);

                var files = await Task.Run(() =>
                {
                    return BackupDAL.GetInstance().BackupFull(destino, partes);
                });

                txtResultado.AppendText("Backup finalizado. Archivos generados:" + Environment.NewLine);
                foreach (var f in files) txtResultado.AppendText(" - " + f + Environment.NewLine);

                MessageBox.Show("Backup realizado con éxito.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar el backup:" + Environment.NewLine + ex.Message,
                    "Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnBackup.Enabled = true;
            }
        }
        */
    }
}

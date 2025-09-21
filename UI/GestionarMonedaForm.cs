using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarMonedaForm : Form
    {
        public GestionarMonedaForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvMoneda.Rows.Clear();
            dgvMoneda.Rows.Add("1", "Dólar", "1.00", "$");
            dgvMoneda.Rows.Add("2", "Euro", "1.08", "€");
        }

        private void dgvMoneda_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMoneda.CurrentRow != null && dgvMoneda.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvMoneda.CurrentRow.Cells["idMoneda"].Value?.ToString() ?? "";
                txtNombre.Text = dgvMoneda.CurrentRow.Cells["nombreMoneda"].Value?.ToString() ?? "";
                txtValor.Text = dgvMoneda.CurrentRow.Cells["valorCambio"].Value?.ToString() ?? "";
                txtSimbolo.Text = dgvMoneda.CurrentRow.Cells["simbolo"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear moneda (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar moneda (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar moneda (simulado)");
        }
    }
}

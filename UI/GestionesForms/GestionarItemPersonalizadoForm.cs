using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarItemPersonalizadoForm : BaseForm
    {
        public GestionarItemPersonalizadoForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvItems.Rows.Clear();
            dgvItems.Rows.Add("1", "Rampa de acceso", "unidad", "450.00");
            dgvItems.Rows.Add("2", "Servicio de andamios", "unidad", "320.00");
        }

        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvItems.CurrentRow != null && dgvItems.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvItems.CurrentRow.Cells["idItem"].Value?.ToString() ?? "";
                txtDescripcion.Text = dgvItems.CurrentRow.Cells["descripcion"].Value?.ToString() ?? "";
                txtUnidad.Text = dgvItems.CurrentRow.Cells["unidad"].Value?.ToString() ?? "";
                txtPrecio.Text = dgvItems.CurrentRow.Cells["precioUnitario"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear ítem personalizado (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar ítem personalizado (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar ítem personalizado (simulado)");
        }
    }
}

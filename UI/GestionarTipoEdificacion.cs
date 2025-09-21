using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarTipoEdificacionForm : Form
    {
        public GestionarTipoEdificacionForm()
        {
            InitializeComponent();
            CargarDatosMockeados();
        }

        private void CargarDatosMockeados()
        {
            dgvTipos.Rows.Clear();
            dgvTipos.Rows.Add("1", "Vivienda unifamiliar");
            dgvTipos.Rows.Add("2", "Edificio comercial");
            dgvTipos.Rows.Add("3", "Edificio Residencial");
        }

        private void dgvTipos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTipos.CurrentRow != null && dgvTipos.CurrentRow.Index >= 0)
            {
                txtId.Text = dgvTipos.CurrentRow.Cells["idTipoEdificacion"].Value?.ToString() ?? "";
                txtDescripcion.Text = dgvTipos.CurrentRow.Cells["descripcion"].Value?.ToString() ?? "";
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Crear tipo de edificación (simulado)");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Modificar tipo de edificación (simulado)");
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Borrar tipo de edificación (simulado)");
        }
    }
}

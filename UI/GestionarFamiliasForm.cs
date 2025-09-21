using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarFamiliasForm : Form
    {
        public GestionarFamiliasForm()
        {
            InitializeComponent();
            CargarUsuariosMock();
        }

        private void CargarUsuariosMock()
        {
            dgvUsuarios.Rows.Clear();
            dgvUsuarios.Rows.Add("1", "Ana Pérez");
            dgvUsuarios.Rows.Add("2", "Carlos Ramírez");
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            string idUsuario = dgvUsuarios.CurrentRow.Cells[0].Value.ToString();

            // Simulación: cargar familias según usuario
            dgvDisponibles.Rows.Clear();
            dgvAsignadas.Rows.Clear();

            if (idUsuario == "1")
            {
                dgvDisponibles.Rows.Add("Familia B");
                dgvAsignadas.Rows.Add("Familia A");
            }
            else
            {
                dgvDisponibles.Rows.Add("Familia A");
                dgvAsignadas.Rows.Add("Familia C");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvDisponibles.CurrentRow != null)
            {
                string valor = dgvDisponibles.CurrentRow.Cells[0].Value.ToString();
                dgvDisponibles.Rows.RemoveAt(dgvDisponibles.CurrentRow.Index);
                dgvAsignadas.Rows.Add(valor);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAsignadas.CurrentRow != null)
            {
                string valor = dgvAsignadas.CurrentRow.Cells[0].Value.ToString();
                dgvAsignadas.Rows.RemoveAt(dgvAsignadas.CurrentRow.Index);
                dgvDisponibles.Rows.Add(valor);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Los cambios han sido guardados (simulado)");
        }
    }
}

using System;
using System.Windows.Forms;

namespace UI
{
    public partial class GestionarPatentesForm : Form
    {
        public GestionarPatentesForm()
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

            // Simulación: cargar Patentes según usuario
            dgvDisponibles.Rows.Clear();
            dgvAsignadas.Rows.Clear();

            if (idUsuario == "1")
            {
                dgvDisponibles.Rows.Add("Patente B");
                dgvAsignadas.Rows.Add("Patente A");
            }
            else
            {
                dgvDisponibles.Rows.Add("Patente A");
                dgvAsignadas.Rows.Add("Patente C");
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

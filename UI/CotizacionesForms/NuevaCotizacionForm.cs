using System;
using System.Windows.Forms;

namespace UI
{
    public partial class NuevaCotizacionForm : BaseForm
    {
        public NuevaCotizacionForm()
        {
            InitializeComponent();
            CargarDatosMock();
        }

        private void CargarDatosMock()
        {
            dgvMateriales.DataSource = new[]
            {
        new { Nombre = "Cemento", Unidad = "kg", PrecioUnidad = 50.0, UsoPorM2 = 5 },
        new { Nombre = "Ladrillo", Unidad = "unidad", PrecioUnidad = 2.0, UsoPorM2 = 60 }
    };

            dgvMaquinarias.DataSource = new[]
            {
        new { Nombre = "Excavadora", CostoPorHora = 100.0 },
        new { Nombre = "Grua", CostoPorHora = 150.0 }
    };

            dgvServicios.DataSource = new[]
            {
        new { Descripcion = "Supervisión de obra", Precio = 2000.0 },
        new { Descripcion = "Instalación eléctrica", Precio = 3000.0 }
    };
        }

        private void btnAgregarMaterial_Click(object sender, EventArgs e)
        {
            var form = new AgregarMaterialesForm();
            form.ShowDialog();
        }

        private void btnAgregarMaquinaria_Click(object sender, EventArgs e)
        {
            var form = new AgregarMaquinariasForm();
            form.ShowDialog();
        }

        private void btnAgregarServicio_Click(object sender, EventArgs e)
        {
            var form = new AgregarServiciosAdicionalesForm();
            form.ShowDialog();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cmbTipoEdificacion.SelectedIndex == -1 || cmbMoneda.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validación simulada
            MessageBox.Show("Cotización guardada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

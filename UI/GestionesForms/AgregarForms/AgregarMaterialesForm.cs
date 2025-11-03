using System;
using System.Windows.Forms;

namespace WinApp
{
    public partial class AgregarMaterialesForm : Form
    {
        public AgregarMaterialesForm()
        {
            InitializeComponent();
            CargarMaterialesMock();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Material agregado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CargarMaterialesMock()
        {
            var lista = new[]
            {
        new MaterialDTO { Nombre = "Cemento", Unidad = "kg", PrecioUnidad = 50.0, UsoPorM2 = 5 },
        new MaterialDTO { Nombre = "Ladrillo", Unidad = "unidad", PrecioUnidad = 2.0, UsoPorM2 = 60 },
        new MaterialDTO { Nombre = "Arena", Unidad = "m3", PrecioUnidad = 35.0, UsoPorM2 = 0.2 }
    };

            dgvMateriales.DataSource = lista;
        }

        private class MaterialDTO
        {
            public string Nombre { get; set; }
            public string Unidad { get; set; }
            public double PrecioUnidad { get; set; }
            public double UsoPorM2 { get; set; }
        }
    }
}


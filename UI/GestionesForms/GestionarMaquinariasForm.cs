using System;
using System.Windows.Forms;

namespace UI.Forms
{
    public partial class AgregarMaquinariasForm : BaseForm
    {
        public AgregarMaquinariasForm()
        {
            InitializeComponent();
            CargarMaquinariasMock();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Maquinaria agregada correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CargarMaquinariasMock()
        {
            var lista = new[]
            {
                new MaquinariaDTO { Nombre = "Excavadora", CostoPorHora = 100.0 },
                new MaquinariaDTO { Nombre = "Grua", CostoPorHora = 150.0 },
                new MaquinariaDTO { Nombre = "Mezcladora", CostoPorHora = 80.0 }
            };

            dgvMaquinarias.DataSource = lista;
        }

        private class MaquinariaDTO
        {
            public string Nombre { get; set; }
            public double CostoPorHora { get; set; }
        }
    }
}

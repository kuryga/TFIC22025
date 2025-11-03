using System;
using System.Windows.Forms;

namespace WinApp
{
    public partial class AgregarServiciosAdicionalesForm : Form
    {
        public AgregarServiciosAdicionalesForm()
        {
            InitializeComponent();
            CargarServiciosMock();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Servicio adicional agregado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CargarServiciosMock()
        {
            var lista = new[]
            {
                new ServicioDTO { Descripcion = "Supervisión de obra", Precio = 2000.0 },
                new ServicioDTO { Descripcion = "Instalación eléctrica", Precio = 3000.0 },
                new ServicioDTO { Descripcion = "Sistema de plomería", Precio = 2500.0 }
            };

            dgvServicios.DataSource = lista;
        }

        private class ServicioDTO
        {
            public string Descripcion { get; set; }
            public double Precio { get; set; }
        }
    }
}

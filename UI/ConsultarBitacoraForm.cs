using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UI
{
    public partial class ConsultarBitacoraForm : Form
    {
        public ConsultarBitacoraForm()
        {
            InitializeComponent();

            // Configurar valores por defecto y restricciones
            dtpHasta.MaxDate = DateTime.Today;
            dtpHasta.Value = DateTime.Today;

            dtpDesde.MaxDate = DateTime.Today;
            dtpDesde.Value = DateTime.Today.AddDays(-1);
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            DateTime desde = dtpDesde.Value.Date;
            DateTime hasta = dtpHasta.Value.Date;

            if (desde > hasta)
            {
                MessageBox.Show("Rango de fechas inválido. La fecha de inicio debe ser anterior a la fecha de fin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simular datos mockeados para la bitácora
            var datos = new List<BitacoraDTO>
            {
                new BitacoraDTO { idRegistro = 1, fecha = DateTime.Now.AddDays(-1), criticidad = "C5", accion = "Login", mensaje = "Ingreso de sesión", idEjecutor = "1", idAfectado = ""},
                new BitacoraDTO { idRegistro = 2, fecha = DateTime.Now, criticidad = "C5", accion = "Consultar cotizaciones", mensaje = "Consulta de cotizaciones", idEjecutor = "1", idAfectado = ""},
                new BitacoraDTO { idRegistro = 2, fecha = DateTime.Now, criticidad = "C2", accion = "Baja usuario", mensaje = "Baja manual de usuario", idEjecutor = "2", idAfectado = "1"}
            };
            // Filtrar por fecha
            var filtrado = datos.FindAll(x => x.fecha.Date >= desde && x.fecha.Date <= hasta);

            dgvBitacora.DataSource = filtrado;

            if (filtrado.Count == 0)
            {
                MessageBox.Show("No se encontraron eventos en el período seleccionado.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private class BitacoraDTO
        {
            public int idRegistro { get; set; }
            public DateTime fecha { get; set; }
            public string criticidad { get; set; }
            public string accion { get; set; }
            public string mensaje { get; set; }
            public string idEjecutor { get; set; }
            public string idAfectado { get; set; }
        }
    }
}

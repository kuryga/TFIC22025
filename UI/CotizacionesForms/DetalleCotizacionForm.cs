using System;
using System.Windows.Forms;
using System.Collections.Generic;

using CotizacionBLL = BLL.Genericos.CotizacionBLL;

namespace UI
{
    public partial class DetalleCotizacionForm : BaseForm
    {
        private readonly int cotizacionId;

        public DetalleCotizacionForm(int id)
        {
            cotizacionId = id;
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            var ctz = CotizacionBLL.GetInstance().GetCotizacionCompleta(cotizacionId);

            if (ctz == null)
            {
                MessageBox.Show("No se encontró la cotización seleccionada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            txtId.Text = ctz.IdCotizacion.ToString();


            cmbTipo.DataSource = null;
            cmbTipo.Items.Clear();
            var tipoTxt = (ctz.TipoEdificacion != null && ctz.TipoEdificacion.Descripcion != null)
                ? ctz.TipoEdificacion.Descripcion
                : string.Empty;
            cmbTipo.Items.Add(tipoTxt);
            cmbTipo.SelectedIndex = 0;

            cmbMoneda.DataSource = null;
            cmbMoneda.Items.Clear();
            string monedaNombre = (ctz.Moneda != null && ctz.Moneda.NombreMoneda != null) ? ctz.Moneda.NombreMoneda : "";
            string simbolo = (ctz.Moneda != null && ctz.Moneda.Simbolo != null) ? ctz.Moneda.Simbolo : "";
            var monedaTxt = string.IsNullOrEmpty(simbolo) ? monedaNombre : (monedaNombre + " (" + simbolo + ")");
            cmbMoneda.Items.Add(monedaTxt);
            cmbMoneda.SelectedIndex = 0;

            // ---------- Grillas ----------
            dgvMateriales.AutoGenerateColumns = true;
            dgvMateriales.DataSource = BuildMaterialesView(ctz.ListaMateriales);

            dgvMaquinaria.AutoGenerateColumns = true;
            dgvMaquinaria.DataSource = BuildMaquinariaView(ctz.ListaMaquinaria);

            dgvServicios.AutoGenerateColumns = true;
            dgvServicios.DataSource = BuildServiciosView(ctz.ListaServicios);

            // Formatos opcionales
            SetGridFormats();
        }

        private void SetGridFormats()
        {
            // Materiales
            if (dgvMateriales.Columns.Contains("PrecioUnidad"))
                dgvMateriales.Columns["PrecioUnidad"].DefaultCellStyle.Format = "N2";
            if (dgvMateriales.Columns.Contains("UsoPorM2"))
                dgvMateriales.Columns["UsoPorM2"].DefaultCellStyle.Format = "N2";
            if (dgvMateriales.Columns.Contains("Cantidad"))
                dgvMateriales.Columns["Cantidad"].DefaultCellStyle.Format = "N2";
            if (dgvMateriales.Columns.Contains("Subtotal"))
                dgvMateriales.Columns["Subtotal"].DefaultCellStyle.Format = "N2";

            // Maquinaria
            if (dgvMaquinaria.Columns.Contains("CostoHora"))
                dgvMaquinaria.Columns["CostoHora"].DefaultCellStyle.Format = "N2";
            if (dgvMaquinaria.Columns.Contains("HorasUso"))
                dgvMaquinaria.Columns["HorasUso"].DefaultCellStyle.Format = "N2";
            if (dgvMaquinaria.Columns.Contains("Subtotal"))
                dgvMaquinaria.Columns["Subtotal"].DefaultCellStyle.Format = "N2";

            // Servicios
            if (dgvServicios.Columns.Contains("Precio"))
                dgvServicios.Columns["Precio"].DefaultCellStyle.Format = "N2";
        }

        // ---------- Proyecciones planas para las grillas ----------

        private static List<object> BuildMaterialesView(List<BE.MaterialCotizacion> items)
        {
            var view = new List<object>();
            if (items == null) return view;

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                var mat = (it != null) ? it.Material : null;

                decimal precio = (mat != null) ? mat.PrecioUnidad : 0m;
                decimal cant = (it != null) ? it.Cantidad : 0m;
                decimal costo = precio * cant;

                view.Add(new
                {
                    Id = (it != null) ? it.IdMaterialCotizacion : 0,
                    Material = (mat != null && mat.Nombre != null) ? mat.Nombre : "",
                    Unidad = (mat != null && mat.UnidadMedida != null) ? mat.UnidadMedida : "",
                    PrecioUnidad = precio,
                    Cantidad = cant,
                    UsoPorM2 = (mat != null) ? mat.UsoPorM2 : 0m,
                    Subtotal = costo
                });
            }
            return view;
        }

        private static List<object> BuildMaquinariaView(List<BE.MaquinariaCotizacion> items)
        {
            var view = new List<object>();
            if (items == null) return view;

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                var maq = (it != null) ? it.Maquinaria : null;

                decimal costoHora = (maq != null) ? maq.CostoPorHora : 0m;
                decimal horas = (it != null) ? it.HorasUso : 0m;
                decimal costo = costoHora * horas;

                view.Add(new
                {
                    Id = (it != null) ? it.IdMaquinariaCotizacion : 0,
                    Maquinaria = (maq != null && maq.Nombre != null) ? maq.Nombre : "",
                    CostoHora = costoHora,
                    HorasUso = horas,
                    Subtotal = costo
                });
            }
            return view;
        }

        private static List<object> BuildServiciosView(List<BE.ServicioCotizacion> items)
        {
            var view = new List<object>();
            if (items == null) return view;

            for (int i = 0; i < items.Count; i++)
            {
                var it = items[i];
                var srv = (it != null) ? it.Servicio : null;

                decimal precio = (srv != null) ? srv.Precio : 0m;

                view.Add(new
                {
                    Id = (it != null) ? it.IdServicioCotizacion : 0,
                    Servicio = (srv != null && srv.Descripcion != null) ? srv.Descripcion : "",
                    Precio = precio
                });
            }
            return view;
        }
    }
}

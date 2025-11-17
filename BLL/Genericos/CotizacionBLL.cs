using System;
using System.Collections.Generic;
using CotizacionDAL = DAL.Genericos.CotizacionDAL;

namespace BLL.Genericos
{
    public class CotizacionBLL
    {
        private static CotizacionBLL _instance;
        private readonly ParametrizacionBLL _param = ParametrizacionBLL.GetInstance();

        private CotizacionBLL() { }

        public static CotizacionBLL GetInstance()
        {
            if (_instance == null) _instance = new CotizacionBLL();
            return _instance;
        }

        public List<BE.Cotizacion> GetListaCotizaciones()
        {
            return CotizacionDAL.GetInstance().GetListaCotizaciones();
        }

        public BE.Cotizacion GetCotizacionCompleta(int idCotizacion)
        {
            if (idCotizacion <= 0)
                throw new ArgumentOutOfRangeException(nameof(idCotizacion),
                    _param.GetLocalizable("quotation_id_required_message"));

            return CotizacionDAL.GetInstance().GetCotizacionCompleta(idCotizacion);
        }

        public int CrearCotizacion(BE.Cotizacion ctz)
        {
            if (ctz == null)
                throw new ArgumentNullException(nameof(ctz),
                    _param.GetLocalizable("quotation_object_required_message"));

            ValidarHeader(ctz);
            ValidarLineas(ctz);

            CotizacionDAL.GetInstance().Create(ctz);

            return ctz.IdCotizacion;
        }

        public void ActualizarCotizacion(BE.Cotizacion ctz)
        {
            if (ctz == null)
                throw new ArgumentNullException(nameof(ctz),
                    _param.GetLocalizable("quotation_object_required_message"));

            if (ctz.IdCotizacion <= 0)
                throw new ArgumentOutOfRangeException(nameof(ctz.IdCotizacion),
                    _param.GetLocalizable("quotation_id_required_message"));

            ValidarHeader(ctz);
            ValidarLineas(ctz);

            CotizacionDAL.GetInstance().Update(ctz);
        }

        public decimal CalcularTotal(BE.Cotizacion ctz)
        {
            if (ctz == null) return 0m;

            decimal total = 0m;

            if (ctz.ListaMateriales != null)
            {
                for (int i = 0; i < ctz.ListaMateriales.Count; i++)
                {
                    var it = ctz.ListaMateriales[i];
                    decimal precio = (it != null && it.Material != null) ? it.Material.PrecioUnidad : 0m;
                    decimal cant = (it != null) ? it.Cantidad : 0m;
                    total += precio * cant;
                }
            }

            if (ctz.ListaMaquinaria != null)
            {
                for (int i = 0; i < ctz.ListaMaquinaria.Count; i++)
                {
                    var it = ctz.ListaMaquinaria[i];
                    decimal costoHora = (it != null && it.Maquinaria != null) ? it.Maquinaria.CostoPorHora : 0m;
                    decimal horas = (it != null) ? it.HorasUso : 0m;
                    total += costoHora * horas;
                }
            }

            if (ctz.ListaServicios != null)
            {
                for (int i = 0; i < ctz.ListaServicios.Count; i++)
                {
                    var it = ctz.ListaServicios[i];
                    decimal precioServ = (it != null && it.Servicio != null) ? it.Servicio.Precio : 0m;
                    total += precioServ;
                }
            }

            return total;
        }

        public decimal CalcularTotalPorId(int idCotizacion)
        {
            if (idCotizacion <= 0)
                throw new ArgumentOutOfRangeException(nameof(idCotizacion),
                    _param.GetLocalizable("quotation_id_required_message"));

            var ctz = GetCotizacionCompleta(idCotizacion);
            return CalcularTotal(ctz);
        }

        private void ValidarHeader(BE.Cotizacion ctz)
        {
            if (ctz == null)
                throw new ArgumentNullException(nameof(ctz),
                    _param.GetLocalizable("quotation_object_required_message"));

            if (ctz.TipoEdificacion == null || ctz.TipoEdificacion.IdTipoEdificacion <= 0)
                throw new ArgumentException(_param.GetLocalizable("quotation_type_required_message"));

            if (ctz.Moneda == null || ctz.Moneda.IdMoneda <= 0)
                throw new ArgumentException(_param.GetLocalizable("quotation_currency_required_message"));

            if (ctz.FechaCreacion == default(DateTime))
                ctz.FechaCreacion = DateTime.UtcNow;
        }

        public void ValidarLineas(BE.Cotizacion ctz)
        {
            if (ctz == null)
                throw new ArgumentNullException(nameof(ctz),
                    _param.GetLocalizable("quotation_object_required_message"));

            bool tieneAlgo =
                (ctz.ListaMateriales != null && ctz.ListaMateriales.Count > 0) ||
                (ctz.ListaMaquinaria != null && ctz.ListaMaquinaria.Count > 0) ||
                (ctz.ListaServicios != null && ctz.ListaServicios.Count > 0);

            if (!tieneAlgo)
                throw new ArgumentException(_param.GetLocalizable("quotation_empty_items_message"));

            if (ctz.ListaMateriales != null)
            {
                for (int i = 0; i < ctz.ListaMateriales.Count; i++)
                {
                    var it = ctz.ListaMateriales[i];
                    if (it == null)
                        throw new ArgumentException(_param.GetLocalizable("quotation_material_item_invalid"));

                    if (it.Material == null || it.Material.IdMaterial <= 0)
                        throw new ArgumentException(_param.GetLocalizable("quotation_material_invalid"));

                    if (it.Cantidad < 0m)
                        throw new ArgumentException(_param.GetLocalizable("quotation_material_qty_negative"));
                }
            }

            if (ctz.ListaMaquinaria != null)
            {
                for (int i = 0; i < ctz.ListaMaquinaria.Count; i++)
                {
                    var it = ctz.ListaMaquinaria[i];
                    if (it == null)
                        throw new ArgumentException(_param.GetLocalizable("quotation_machinery_item_invalid"));

                    if (it.Maquinaria == null || it.Maquinaria.IdMaquinaria <= 0)
                        throw new ArgumentException(_param.GetLocalizable("quotation_machinery_invalid"));

                    if (it.HorasUso < 0m)
                        throw new ArgumentException(_param.GetLocalizable("quotation_machinery_hours_negative"));
                }
            }

            if (ctz.ListaServicios != null)
            {
                for (int i = 0; i < ctz.ListaServicios.Count; i++)
                {
                    var it = ctz.ListaServicios[i];
                    if (it == null)
                        throw new ArgumentException(_param.GetLocalizable("quotation_service_item_invalid"));

                    if (it.Servicio == null || it.Servicio.IdServicio <= 0)
                        throw new ArgumentException(_param.GetLocalizable("quotation_service_invalid"));
                }
            }
        }
    }
}

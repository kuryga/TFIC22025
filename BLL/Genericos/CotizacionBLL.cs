using System;
using System.Collections.Generic;

using CotizacionDAL = DAL.Genericos.CotizacionDAL;

namespace BLL.Genericos
{
    public class CotizacionBLL
    {
        private static CotizacionBLL _instance;
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
            if (idCotizacion <= 0) throw new ArgumentOutOfRangeException("idCotizacion");
            return CotizacionDAL.GetInstance().GetCotizacionCompleta(idCotizacion);
        }


        public int CrearCotizacion(BE.Cotizacion ctz)
        {
            //  ValidarHeader(ctz);
            // CotizacionDAL.GetInstance().Create(ctz);

            return ctz.IdCotizacion;
        }

        public void ActualizarCotizacion(BE.Cotizacion ctz)
        {
            if (ctz == null) throw new ArgumentNullException("ctz");
            if (ctz.IdCotizacion <= 0) throw new ArgumentOutOfRangeException("ctz.IdCotizacion");

            ValidarHeader(ctz);
            CotizacionDAL.GetInstance().Update(ctz);

            // GuardarLineas(ctz);
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

        private void ValidarHeader(BE.Cotizacion ctz)
        {
            if (ctz == null) throw new ArgumentNullException("ctz");

            if (ctz.TipoEdificacion == null || ctz.TipoEdificacion.IdTipoEdificacion <= 0)
                throw new ArgumentException("Debe indicar un Tipo de Edificación válido.");

            if (ctz.Moneda == null || ctz.Moneda.IdMoneda <= 0)
                throw new ArgumentException("Debe indicar una Moneda válida.");

            if (ctz.FechaCreacion == default(DateTime))
                ctz.FechaCreacion = DateTime.UtcNow;
        }

        public void ValidarLineas(BE.Cotizacion ctz)
        {
            if (ctz == null) throw new ArgumentNullException("ctz");

            if (ctz.ListaMateriales != null)
            {
                for (int i = 0; i < ctz.ListaMateriales.Count; i++)
                {
                    var it = ctz.ListaMateriales[i];
                    if (it == null) throw new ArgumentException("Ítem de material inválido.");
                    if (it.Material == null || it.Material.IdMaterial <= 0)
                        throw new ArgumentException("Material inválido en ítems.");
                    if (it.Cantidad < 0m)
                        throw new ArgumentException("Cantidad de material no puede ser negativa.");
                }
            }

            if (ctz.ListaMaquinaria != null)
            {
                for (int i = 0; i < ctz.ListaMaquinaria.Count; i++)
                {
                    var it = ctz.ListaMaquinaria[i];
                    if (it == null) throw new ArgumentException("Ítem de maquinaria inválido.");
                    if (it.Maquinaria == null || it.Maquinaria.IdMaquinaria <= 0)
                        throw new ArgumentException("Maquinaria inválida en ítems.");
                    if (it.HorasUso < 0m)
                        throw new ArgumentException("Horas de uso no pueden ser negativas.");
                }
            }

            if (ctz.ListaServicios != null)
            {
                for (int i = 0; i < ctz.ListaServicios.Count; i++)
                {
                    var it = ctz.ListaServicios[i];
                    if (it == null) throw new ArgumentException("Ítem de servicio inválido.");
                    if (it.Servicio == null || it.Servicio.IdServicio <= 0)
                        throw new ArgumentException("Servicio inválido en ítems.");
                }
            }
        }

        // TODO: hace falta DAL para MaterialCotizacion, MaquinariaCotizacion y ServicioCotizacion????? revisar cuando vaya por el crear
        /*
        private void GuardarLineas(BE.Cotizacion ctz)
        {
            if (ctz == null || ctz.IdCotizacion <= 0) return;

            // var matDal = DAL.Genericos.MaterialCotizacionDAL.GetInstance();
            // var maqDal = DAL.Genericos.MaquinariaCotizacionDAL.GetInstance();
            // var srvDal = DAL.Genericos.ServicioCotizacionDAL.GetInstance();
        }
        */
    }
}

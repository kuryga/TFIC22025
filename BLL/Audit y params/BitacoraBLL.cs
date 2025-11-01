using System;
using System.Collections.Generic;
using DAL.Audit;

using PagedResult = BE.Genericos.PagedResult<BE.Audit.Bitacora>;
namespace BLL.Audit
{
    public sealed class BitacoraBLL
    {
        private static BitacoraBLL _instance;
        private BitacoraBLL() { }
        public static BitacoraBLL GetInstance()
        {
            if (_instance == null) _instance = new BitacoraBLL();
            return _instance;
        }

        public PagedResult GetBitacora(
             DateTime? desde,
             DateTime? hasta,
             int page = 1,
             int pageSize = 30,
             string criticidad = null)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 30;

            var items = BitacoraDAL.GetInstance().GetBitacoraList(desde, hasta, page, pageSize, criticidad);

            return new PagedResult
            {
                Items = items ?? new List<BE.Audit.Bitacora>(),
                Total = items?.Count ?? 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult GetBitacora(
            DateTime? desde,
            DateTime? hasta,
            int page,
            int pageSize,
            BE.Audit.Criticidad criticidad)
        {
            string crit = (criticidad == BE.Audit.Criticidad.None) ? null : criticidad.ToString();
            return GetBitacora(desde, hasta, page, pageSize, crit);
        }

        public List<BE.Audit.Criticidad> GetCriticidades()
            => BitacoraDAL.GetInstance().GetCriticidades();

        public int Log(string accion, string mensaje)
            => BitacoraDAL.GetInstance().Log(accion, mensaje);

        public int Log(string accion, string mensaje, BE.Audit.Criticidad criticidad)
            => BitacoraDAL.GetInstance().Log(accion, mensaje, criticidad.ToString());

        public string ExportarReporte(
            DateTime? desde,
            DateTime? hasta,
            int? page,
            int? pageSize,
            string criticidad = null,
            string destino = null,
            bool exportarTodos = false)
            => BitacoraDAL.GetInstance().ExportarReporte(desde, hasta, page, pageSize, criticidad, destino, exportarTodos);

        public string ExportarReporte(
            DateTime? desde,
            DateTime? hasta,
            int? page,
            int? pageSize,
            BE.Audit.Criticidad criticidad,
            string destino = null,
            bool exportarTodos = false)
        {
            string crit = (criticidad == BE.Audit.Criticidad.None) ? null : criticidad.ToString();
            return BitacoraDAL.GetInstance().ExportarReporte(desde, hasta, page, pageSize, crit, destino, exportarTodos);
        }

        public string ExportarReportePagina(
            DateTime? desde,
            DateTime? hasta,
            int page,
            int pageSize,
            string criticidad = null,
            string destino = null)
            => BitacoraDAL.GetInstance().ExportarReporte(desde, hasta, page, pageSize, criticidad, destino, false);

        public string ExportarReporteTodos(
            DateTime? desde,
            DateTime? hasta,
            string criticidad = null,
            string destino = null)
            => BitacoraDAL.GetInstance().ExportarReporte(desde, hasta, null, null, criticidad, destino, true);
    }
}

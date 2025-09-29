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
            int pageSize = 30)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 30;

            var items = BitacoraDAL.GetInstance().GetBitacoraList(desde, hasta, page, pageSize);

            return new PagedResult
            {
                Items = items ?? new List<BE.Audit.Bitacora>(),
                Total = items.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        public int Log(string accion, string mensaje)
            => BitacoraDAL.GetInstance().Log(accion, mensaje);

        public int Log(string accion, string mensaje, BE.Audit.Criticidad criticidad)
            => BitacoraDAL.GetInstance().Log(accion, mensaje, criticidad.ToString());
    }
}

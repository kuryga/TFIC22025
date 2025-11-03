using System;
using System.Collections.Generic;

using TipoEdificacionDAL = DAL.Genericos.TipoEdificacionDAL;

namespace BLL.Genericos
{
    public class TipoEdificacionBLL : BE.ICrud<BE.TipoEdificacion>
    {
        private TipoEdificacionBLL() { }
        private static TipoEdificacionBLL instance;
        public static TipoEdificacionBLL GetInstance()
        {
            if (instance == null) instance = new TipoEdificacionBLL();
            return instance;
        }

        public bool Create(BE.TipoEdificacion objAdd)
        {
            try
            {
                if (objAdd == null) throw new ArgumentNullException(nameof(objAdd));
                TipoEdificacionDAL.GetInstance().Create(objAdd);
                return objAdd.IdTipoEdificacion > 0;
            }
            catch (Exception) { throw; }
        }

        public List<BE.TipoEdificacion> GetAll()
        {
            try { return TipoEdificacionDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }

        public bool Update(BE.TipoEdificacion objUpd)
        {
            try
            {
                if (objUpd == null) throw new ArgumentNullException(nameof(objUpd));
                if (objUpd.IdTipoEdificacion <= 0) throw new ArgumentException("Id inválido");
                TipoEdificacionDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Delete(BE.TipoEdificacion objUdp)
        {
            try
            {
                if (objUdp == null) throw new ArgumentNullException(nameof(objUdp));
                if (objUdp.IdTipoEdificacion <= 0) throw new ArgumentException("Id inválido");
                TipoEdificacionDAL.GetInstance().Deshabilitar(objUdp.IdTipoEdificacion, true);
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Deshabilitar(int idTipoEdificacion, bool deshabilitar)
        {
            try
            {
                if (idTipoEdificacion <= 0) throw new ArgumentException("Id inválido");
                TipoEdificacionDAL.GetInstance().Deshabilitar(idTipoEdificacion, deshabilitar);
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}

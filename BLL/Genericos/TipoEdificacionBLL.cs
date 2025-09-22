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
            //try { return UsuarioDAL.GetInstance().Create(objAdd); }
            //catch (Exception) { throw; }

            return false;
        }


        public List<BE.TipoEdificacion> GetAll()
        {
            try { return TipoEdificacionDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }


        public BE.TipoEdificacion GetByID(string idEdificacion)
        {
            // try { return MonedaDAL.GetInstance().GetByCorreoElectronico(username); }
            //catch (Exception) { throw; }
            // TODO: sacar esto y modificar
            return new BE.TipoEdificacion();
        }


        public bool Update(BE.TipoEdificacion objUpd)
        {
            try
            {
                TipoEdificacionDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }


        public bool Delete(BE.TipoEdificacion objUdp)
        {
            //try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            //catch (Exception) { throw; }

            return false;
        }
    }
}

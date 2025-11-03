using System;
using System.Collections.Generic;

using ServicioAdicionalDAL = DAL.Genericos.ServicioAdicionalDAL;

namespace BLL.Genericos
{
    public class ServicioAdicionalBLL : BE.ICrud<BE.ServicioAdicional>
    {
        private ServicioAdicionalBLL() { }
        private static ServicioAdicionalBLL instance;
        public static ServicioAdicionalBLL GetInstance()
        {
            if (instance == null) instance = new ServicioAdicionalBLL();
            return instance;
        }


        public bool Create(BE.ServicioAdicional objAdd)
        {
            //try { return UsuarioDAL.GetInstance().Create(objAdd); }
            //catch (Exception) { throw; }

            return false;
        }


        public List<BE.ServicioAdicional> GetAll()
        {
            try { return ServicioAdicionalDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }


        public BE.ServicioAdicional GetByID(string idServicioAd)
        {
            // try { return MonedaDAL.GetInstance().GetByCorreoElectronico(username); }
            //catch (Exception) { throw; }
            // TODO: sacar esto y modificar
            return new BE.ServicioAdicional();
        }


        public bool Update(BE.ServicioAdicional objUpd)
        {
            try
            {
                ServicioAdicionalDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }


        public bool Deshabilitar(int idTipEd, bool deshabilitar)
        {
            //try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            //catch (Exception) { throw; }

            return false;
        }
    }
}

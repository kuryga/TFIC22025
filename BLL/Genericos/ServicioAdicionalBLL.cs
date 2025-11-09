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
            try
            {
                if (objAdd == null) throw new ArgumentNullException(nameof(objAdd));
                ServicioAdicionalDAL.GetInstance().Create(objAdd);
                return objAdd.IdServicio > 0;
            }
            catch (Exception) { throw; }
        }

        public List<BE.ServicioAdicional> GetAll()
        {
            try { return ServicioAdicionalDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }

        public bool Update(BE.ServicioAdicional objUpd)
        {
            try
            {
                if (objUpd == null) throw new ArgumentNullException(nameof(objUpd));
                if (objUpd.IdServicio <= 0) throw new ArgumentException("Id inválido");
                ServicioAdicionalDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Deshabilitar(int idServicio, bool deshabilitar)
        {
            try
            {
                if (idServicio <= 0) throw new ArgumentException("Id inválido");
                ServicioAdicionalDAL.GetInstance().Deshabilitar(idServicio, deshabilitar);
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}

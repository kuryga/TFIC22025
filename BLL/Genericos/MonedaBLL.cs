using System;
using System.Collections.Generic;
using MonedaDAL = DAL.Genericos.MonedaDAL;

namespace BLL.Genericos
{
    public class MonedaBLL : BE.ICrud<BE.Moneda>
    {
        private MonedaBLL() { }
        private static MonedaBLL instance;
        public static MonedaBLL GetInstance()
        {
            if (instance == null) instance = new MonedaBLL();
            return instance;
        }

        public bool Create(BE.Moneda objAdd)
        {
            try
            {
                if (objAdd == null) throw new ArgumentNullException(nameof(objAdd));
                MonedaDAL.GetInstance().Create(objAdd);
                return objAdd.IdMoneda > 0;
            }
            catch (Exception) { throw; }
        }

        public List<BE.Moneda> GetAll()
        {
            try { return MonedaDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }

        public bool Update(BE.Moneda objUpd)
        {
            try
            {
                if (objUpd == null) throw new ArgumentNullException(nameof(objUpd));
                if (objUpd.IdMoneda <= 0) throw new ArgumentException("Id inválido");
                MonedaDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Deshabilitar(int idMoneda, bool deshabilitar)
        {
            try
            {
                if (idMoneda <= 0) throw new ArgumentException("Id inválido");
                MonedaDAL.GetInstance().Deshabilitar(idMoneda, deshabilitar);
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}

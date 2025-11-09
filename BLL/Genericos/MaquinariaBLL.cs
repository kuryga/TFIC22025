using System;
using System.Collections.Generic;

using MaquinariaDAL = DAL.Genericos.MaquinariaDAL;

namespace BLL.Genericos
{
    public class MaquinariaBLL : BE.ICrud<BE.Maquinaria>
    {
        private MaquinariaBLL() { }
        private static MaquinariaBLL instance;
        public static MaquinariaBLL GetInstance()
        {
            if (instance == null) instance = new MaquinariaBLL();
            return instance;
        }

        public bool Create(BE.Maquinaria objAdd)
        {
            try
            {
                if (objAdd == null) throw new ArgumentNullException(nameof(objAdd));
                MaquinariaDAL.GetInstance().Create(objAdd);
                return objAdd.IdMaquinaria > 0;
            }
            catch (Exception) { throw; }
        }

        public List<BE.Maquinaria> GetAll()
        {
            try { return MaquinariaDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }

        public bool Update(BE.Maquinaria objUpd)
        {
            try
            {
                if (objUpd == null) throw new ArgumentNullException(nameof(objUpd));
                if (objUpd.IdMaquinaria <= 0) throw new ArgumentException("Id inválido");
                MaquinariaDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Deshabilitar(int idMaq, bool deshabilitar)
        {
            try
            {
                if (idMaq <= 0) throw new ArgumentException("Id inválido");
                MaquinariaDAL.GetInstance().Deshabilitar(idMaq, deshabilitar);
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}

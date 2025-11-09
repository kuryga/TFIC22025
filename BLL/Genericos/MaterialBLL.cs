using System;
using System.Collections.Generic;

using MaterialDAL = DAL.Genericos.MaterialDAL;

namespace BLL.Genericos
{
    public class MaterialBLL : BE.ICrud<BE.Material>
    {
        private MaterialBLL() { }
        private static MaterialBLL instance;
        public static MaterialBLL GetInstance()
        {
            if (instance == null) instance = new MaterialBLL();
            return instance;
        }

        public bool Create(BE.Material objAdd)
        {
            try
            {
                if (objAdd == null) throw new ArgumentNullException(nameof(objAdd));
                MaterialDAL.GetInstance().Create(objAdd);
                return objAdd.IdMaterial > 0;
            }
            catch (Exception) { throw; }
        }

        public List<BE.Material> GetAll()
        {
            try { return MaterialDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }

        public bool Update(BE.Material objUpd)
        {
            try
            {
                if (objUpd == null) throw new ArgumentNullException(nameof(objUpd));
                if (objUpd.IdMaterial <= 0) throw new ArgumentException("Id inválido");
                MaterialDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Deshabilitar(int idMat, bool deshabilitar)
        {
            try
            {
                if (idMat <= 0) throw new ArgumentException("Id inválido");
                MaterialDAL.GetInstance().Deshabilitar(idMat, deshabilitar);
                return true;
            }
            catch (Exception) { throw; }
        }
    }
}

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
            //try { return UsuarioDAL.GetInstance().Create(objAdd); }
            //catch (Exception) { throw; }

            return false;
        }


        public List<BE.Material> GetAll()
        {
            try { return MaterialDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }


        public BE.Material GetByID(string idMat)
        {
            // try { return MaterialDAL.GetInstance().GetByCorreoElectronico(username); }
            //catch (Exception) { throw; }
            // TODO: sacar esto y modificar
            return new BE.Material();
        }


        public bool Update(BE.Material objUpd)
        {
            try
            {
                MaterialDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }


        public bool Deshabilitar(int idMat, bool deshabilitar)
        {
            //try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            //catch (Exception) { throw; }

            return false;
        }
    }
}

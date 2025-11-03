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
            //try { return UsuarioDAL.GetInstance().Create(objAdd); }
            //catch (Exception) { throw; }

            return false;
        }


        public List<BE.Maquinaria> GetAll()
        {
            try { return MaquinariaDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }


        public BE.Maquinaria GetByID(string idMaq)
        {
            // try { return MonedaDAL.GetInstance().GetByCorreoElectronico(username); }
            //catch (Exception) { throw; }
            // TODO: sacar esto y modificar
            return new BE.Maquinaria();
        }


        public bool Update(BE.Maquinaria objUpd)
        {
            try
            {
                MaquinariaDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }


        public bool Deshabilitar(int idMaq, bool deshabilitar)
        {
            //try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            //catch (Exception) { throw; }

            return false;
        }
    }
}

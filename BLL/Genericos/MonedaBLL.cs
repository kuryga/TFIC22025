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
            //try { return UsuarioDAL.GetInstance().Create(objAdd); }
            //catch (Exception) { throw; }

            return false;
        }


        public List<BE.Moneda> GetAll()
        {
            try { return MonedaDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }


        public BE.Moneda GetByID(string username)
        {
            // try { return MonedaDAL.GetInstance().GetByCorreoElectronico(username); }
            //catch (Exception) { throw; }
            // TODO: sacar esto y modificar
            return new BE.Moneda();
        }


        public bool Update(BE.Moneda objUpd)
        {
            try
            {
                MonedaDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }


        public bool Deshabilitar(int idMoneda, bool deshabilitar)
        {
            //try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            //catch (Exception) { throw; }

            return false;
        }
    }
}

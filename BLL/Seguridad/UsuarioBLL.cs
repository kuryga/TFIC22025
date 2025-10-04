using System;
using System.Collections.Generic;

using UsuarioDAL = DAL.Seguridad.UsuarioDAL;
namespace BLL.Seguridad
{
    public class UsuarioBLL : BE.ICrud<BE.Usuario>
    {
        private UsuarioBLL() { }
        private static UsuarioBLL instance;
        public static UsuarioBLL GetInstance()
        {
            if (instance == null) instance = new UsuarioBLL();
            return instance;
        }


        public bool Create(BE.Usuario objAdd)
        {
            //try { return UsuarioDAL.GetInstance().Create(objAdd); }
            //catch (Exception) { throw; }

            return false;
        }


        public List<BE.Usuario> GetAll()
        {
              try { return UsuarioDAL.GetInstance().GetAll(); }
              catch (Exception) { throw; }
        }

        public bool Update(BE.Usuario objUpd)
        {
            try { UsuarioDAL.GetInstance().Update(objUpd);
                return true;
                }
            catch (Exception) { throw; }
        }


        public bool Delete(BE.Usuario objUdp)
        {
            //try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            //catch (Exception) { throw; }

            return false;
        }
    }
}

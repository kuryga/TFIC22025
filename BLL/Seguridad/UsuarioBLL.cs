using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UsuarioDAL = DAL.Seguridad.Usuario;

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
          try { return UsuarioDAL.GetInstance().Create(objAdd); }
          catch (Exception) { throw; }
        }


        public List<BE.Usuario> GetAll()
        {
              try { return UsuarioDAL.GetInstance().GetAll(); }
              catch (Exception) { throw; }
        }


        public BE.Usuario GetByUsername(string username)
        {
             try { return UsuarioDAL.GetInstance().GetByUsername(username); }
             catch (Exception) { throw; }
        }


        public bool Update(BE.Usuario objUpd)
        {
            try { return UsuarioDAL.GetInstance().Update(objUpd); }
            catch (Exception) { throw; }
        }


        public bool Delete(BE.Usuario objUdp)
        {
            try { return UsuarioDAL.GetInstance().Delete(objUdp); }
            catch (Exception) { throw; }
        }
    }
}

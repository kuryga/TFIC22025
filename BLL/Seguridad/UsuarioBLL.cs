using System;
using System.Collections.Generic;

using UsuarioDAL = DAL.Seguridad.UsuarioDAL;
using SessionContext = DAL.Seguridad.SessionContext;

namespace BLL.Seguridad
{
    public class UsuarioBLL
    {
        private UsuarioBLL() { }
        private static UsuarioBLL instance;
        public static UsuarioBLL GetInstance()
        {
            if (instance == null) instance = new UsuarioBLL();
            return instance;
        }

        public void Create(BE.Usuario objAdd)
        {
            try { UsuarioDAL.GetInstance().Create(objAdd); }
            catch (Exception) { throw; }
        }

        public List<BE.Usuario> GetAll()
        {
            try { return UsuarioDAL.GetInstance().GetAll(); }
            catch (Exception) { throw; }
        }

        public bool Update(BE.Usuario objUpd)
        {
            try
            {
                UsuarioDAL.GetInstance().Update(objUpd);
                return true;
            }
            catch (Exception) { throw; }
        }

        public string GetSesionActivaNombreCompleto()
        {
            return SessionContext.Current.NombreCompleto;
        }

        public void EnviarRecuperoContrasena(string correoUsuario)
        {
            try
            {
                UsuarioDAL.GetInstance().enviarRecuperoContrasena(correoUsuario);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int VerificarCodigoRecuperacion(string correoUsuario, string codigoPlano)
        {
            try
            {
                return UsuarioDAL.GetInstance().VerificarCodigoRecuperacion(correoUsuario?.Trim(), codigoPlano?.Trim());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CambiarContrasenaConToken(int idUsuario, string nuevaContrasena)
        {
            try
            {
                UsuarioDAL.GetInstance().CambiarContrasenaConToken(idUsuario, nuevaContrasena);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

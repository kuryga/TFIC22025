using System;
using DAL.Seguridad;

namespace BLL.Seguridad
{
    public class EncriptacionBLL
    {
        private EncriptacionBLL() { }
        private static EncriptacionBLL instance;

        public static EncriptacionBLL GetInstance()
        {
            if (instance == null)
                instance = new EncriptacionBLL();
            return instance;
        }

        public string EncriptarReversible(string textoPlano)
        {
            try
            {
                return SecurityUtilities.EncriptarReversible(textoPlano);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string DesencriptarReversible(string textoCifrado)
        {
            try
            {
                return SecurityUtilities.DesencriptarReversible(textoCifrado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string EncriptarIrreversible(string textoPlano)
        {
            try
            {
                return SecurityUtilities.EncriptarIrreversible(textoPlano);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool VerificarIrreversible(string textoPlano, string hashMd5Base64)
        {
            try
            {
                return SecurityUtilities.VerificarIrreversible(textoPlano, hashMd5Base64);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

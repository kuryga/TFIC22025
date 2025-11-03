using System;
using System.Collections.Generic;
using IdiomaDAL = DAL.Genericos.IdiomaDAL;
using ParametrizacionDAL = DAL.Genericos.ParametrizacionDAL;
using SessionContext = DAL.Seguridad.SessionContext;
using TranslationContext = DAL.Genericos.TraduccionContext;

namespace BLL.Genericos
{
    public class ParametrizacionBLL
    {
        private static ParametrizacionBLL instance;

        private ParametrizacionBLL() { }

        public static ParametrizacionBLL GetInstance()
        {
            if (instance == null) instance = new ParametrizacionBLL();
            return instance;
        }

        public void LoadParametrizacion()
        {
            try
            {
                SessionContext.Current.parametrizacion = ParametrizacionDAL.GetInstance().GetParametrizacion();
                TranslationContext.CargarTraducciones();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void LoadLocalizablesForIdioma(int idIdioma)
        {
            try
            {
                TranslationContext.CargarTraduccionesEspecificas(idIdioma);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<BE.Idioma> GetIdiomas()
        {
            try
            {
                return IdiomaDAL.GetInstance().GetAll();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetNombreEmpresa()
        {
            return SessionContext.Current.parametrizacion.NombreEmpresa;
        }

        public int GetIdIdioma()
        {
            return SessionContext.Current.parametrizacion.IdIdioma;
        }

        public string GetLocalizable(string cod)
        {
            return TranslationContext.Traducir(cod);
        }
    }
}

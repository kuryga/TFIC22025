using System;
using System.Collections.Generic;
using ParametrizacionDAL = DAL.Genericos.ParametrizacionDAL;
using Parametrizacion = BE.Params.Parametrizacion;
using IdiomanDAL = DAL.Genericos.IdiomaDAL;

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

        public Parametrizacion GetParametrizacion()
        {
            try
            {
                return ParametrizacionDAL.GetInstance().GetParametrizacion();
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
                return IdiomanDAL.GetInstance().GetAll();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

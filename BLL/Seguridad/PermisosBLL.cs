using System;
using System.Collections.Generic;
using DAL.Seguridad;

namespace BLL.Seguridad
{
    public sealed class PermisosBLL
    {
        private static PermisosBLL _instance;
        private PermisosBLL() { }
        public static PermisosBLL GetInstance()
        {
            if (_instance == null) _instance = new PermisosBLL();
            return _instance;
        }

        public List<BE.Familia> GetFamiliasByUsuario(int idUsuario)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            return PermisosDAL.GetInstance().GetFamiliasByUsuario(idUsuario);
        }

        public List<BE.Patente> GetPatentesByUsuario(int idUsuario)
        {
            if (idUsuario <= 0) throw new ArgumentOutOfRangeException(nameof(idUsuario));
            return PermisosDAL.GetInstance().GetPatentesByUsuario(idUsuario);
        }

        public List<BE.Patente> GetPatentesByFamilia(int idFamilia)
        {
            if (idFamilia <= 0) throw new ArgumentOutOfRangeException(nameof(idFamilia));
            return PermisosDAL.GetInstance().GetPatentesByFamilia(idFamilia);
        }
    }
}

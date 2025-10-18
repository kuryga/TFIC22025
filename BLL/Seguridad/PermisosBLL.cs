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
        public List<BE.Familia> GetAllFamilias()
            => PermisosDAL.GetInstance().GetAllFamilias();
        public List<BE.Patente> GetAllPatentes()
            => PermisosDAL.GetInstance().GetAllPatentes();

        public void SetPatentesForUsuario(int idUsuario, IEnumerable<int> idsPatente)
            => PermisosDAL.GetInstance().SetPatentesForUsuario(idUsuario, idsPatente);

        public void SetFamiliasForUsuario(int idUsuario, IEnumerable<int> idsFamilia)
            => PermisosDAL.GetInstance().SetFamiliasForUsuario(idUsuario, idsFamilia);
       
        public int CreateFamilia(BE.Familia familia, IEnumerable<int> idsPatente = null)
            => PermisosDAL.GetInstance().CreateFamilia(familia, idsPatente ?? Array.Empty<int>());

        public void UpdateFamilia(BE.Familia familia, IEnumerable<int> idsPatente)
            => PermisosDAL.GetInstance().UpdateFamilia(familia, idsPatente ?? Array.Empty<int>());
    }
}

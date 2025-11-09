using DAL.Seguridad;
using System;
using System.Collections.Generic;
using CodigoPatentes = BE.Seguridad.CodigoPatente;

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

        private bool TieneAlguna(params CodigoPatentes[] codigos)
        {
            var codigosStr = new List<string>();
            foreach (var c in codigos)
                codigosStr.Add(c.ToString());

            return SessionContext.Current.TieneAlguna(codigosStr.ToArray());
        }


        // gestor de permisos especificos
        public bool DebeVerUsuarios() => TieneAlguna(CodigoPatentes.PT_SEG_USUARIOS_ABM);
        public bool DebeVerFamilias() => TieneAlguna(CodigoPatentes.PT_SEG_FAMILIAS_VER, CodigoPatentes.PT_SEG_FAMILIAS_CREAR, CodigoPatentes.PT_SEG_FAMILIAS_MODIFICAR);
        public bool DebeVerPatentes() => TieneAlguna(CodigoPatentes.PT_SEG_PATENTES_VER, CodigoPatentes.PT_SEG_PATENTES_ASIGNAR);
        public bool DebeVerBitacora() => TieneAlguna(CodigoPatentes.PT_SEG_BITACORA_VER);

        public bool DebeVerMaquinaria() => TieneAlguna(CodigoPatentes.PT_MAQ_ABM);
        public bool DebeVerMateriales() => TieneAlguna(CodigoPatentes.PT_MAT_ABM);
        public bool DebeVerServicios() => TieneAlguna(CodigoPatentes.PT_SRV_ABM);
        public bool DebeVerTipoEdificacion() => TieneAlguna(CodigoPatentes.PT_TED_ABM);
        public bool DebeVerMoneda() => TieneAlguna(CodigoPatentes.PT_MON_ABM);

        public bool DebeVerCotizaciones() => TieneAlguna(CodigoPatentes.PT_COT_VER, CodigoPatentes.PT_COT_CREAR);
        public bool DebeCrearCotizacion() => TieneAlguna(CodigoPatentes.PT_COT_CREAR);

        public bool DebeVerBackup() => TieneAlguna(CodigoPatentes.PT_SEG_BACKUP_EJECUTAR);
        public bool DebeVerRestore() => TieneAlguna(CodigoPatentes.PT_SEG_RESTORE_EJECUTAR);

        public bool PuedeCrearFamilia() => TieneAlguna(CodigoPatentes.PT_SEG_FAMILIAS_CREAR);
        public bool PuedeModificarFamilia() => TieneAlguna(CodigoPatentes.PT_SEG_FAMILIAS_MODIFICAR);
        public bool PuedeAsignarFamilia() => TieneAlguna(CodigoPatentes.PT_SEG_FAMILIA_ASIGNAR);
    }
}

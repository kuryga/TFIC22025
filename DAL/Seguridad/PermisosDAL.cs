using System.Collections.Generic;
using System.Data;

namespace DAL.Seguridad
{
    public sealed class PermisosDAL
    {
        private static PermisosDAL instance;
        private PermisosDAL() { }

        public static PermisosDAL GetInstance()
        {
            if (instance == null) instance = new PermisosDAL();
            return instance;
        }

        private static readonly DalToolkit db = new DalToolkit();

        private const string familiaTable = "dbo.Familia";
        private const string familiaIdCol = "idFamilia";
        private const string familiaCols = "idFamilia AS IdFamilia, nombreFamilia AS NombreFamilia, descripcion AS Descripcion";

        private const string patenteTable = "dbo.Patente";
        private const string patenteIdCol = "idPatente";
        private const string patenteCols = "idPatente AS IdPatente, nombrePatente AS NombrePatente, descripcion AS Descripcion";

        private const string usuarioIdCol = "idUsuario";

        public List<BE.Familia> GetFamiliasByUsuario(int idUsuario)
        {
            string sql = @"
SELECT DISTINCT " + familiaCols + @"
FROM dbo.UsuarioFamilia uf
JOIN " + familiaTable + @" f ON f." + familiaIdCol + @" = uf." + familiaIdCol + @"
WHERE uf." + usuarioIdCol + @" = @idUsuario;";

            return db.QueryListAndLog<BE.Familia>(
                sql,
                cmd => cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario,
                familiaTable, familiaIdCol,
                BE.Audit.AuditEvents.ConsultaFamiliasPorUsuario,
                "Familias por usuario Id=" + idUsuario
            );
        }

        public List<BE.Patente> GetPatentesByUsuario(int idUsuario)
        {
            string sql = @"
SELECT DISTINCT " + patenteCols + @"
FROM dbo.UsuarioPatente up
JOIN " + patenteTable + @" p ON p." + patenteIdCol + @" = up." + patenteIdCol + @"
WHERE up." + usuarioIdCol + @" = @idUsuario
UNION
SELECT DISTINCT " + patenteCols + @"
FROM dbo.UsuarioFamilia uf
JOIN dbo.FamiliaPatente fp ON fp." + familiaIdCol + @" = uf." + familiaIdCol + @"
JOIN " + patenteTable + @" p ON p." + patenteIdCol + @" = fp." + patenteIdCol + @"
WHERE uf." + usuarioIdCol + @" = @idUsuario;";

            return db.QueryListAndLog<BE.Patente>(
                sql,
                cmd => cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario,
                patenteTable, patenteIdCol,
                BE.Audit.AuditEvents.ConsultaPatentesPorUsuario,
                "Patentes por usuario Id=" + idUsuario
            );
        }

        public List<BE.Patente> GetPatentesByFamilia(int idFamilia)
        {
            string sql = @"
SELECT DISTINCT " + patenteCols + @"
FROM dbo.FamiliaPatente fp
JOIN " + patenteTable + @" p ON p." + patenteIdCol + @" = fp." + patenteIdCol + @"
WHERE fp." + familiaIdCol + @" = @idFamilia;";

            return db.QueryListAndLog<BE.Patente>(
                sql,
                cmd => cmd.Parameters.Add("@idFamilia", SqlDbType.Int).Value = idFamilia,
                patenteTable, patenteIdCol,
                BE.Audit.AuditEvents.ConsultaPatentesPorFamilia,
                "Patentes por familia Id=" + idFamilia
            );
        }
    }
}

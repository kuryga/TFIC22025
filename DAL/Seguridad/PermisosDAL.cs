using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        public List<BE.Familia> GetAllFamilias()
        {
            string sql = "SELECT idFamilia AS IdFamilia, nombreFamilia AS NombreFamilia, descripcion AS Descripcion FROM dbo.Familia ORDER BY nombreFamilia;";
            return db.QueryListAndLog<BE.Familia>(sql, null, "dbo.Familia", "idFamilia",
                BE.Audit.AuditEvents.ConsultaFamilias, "Listado de familias");
        }

        public List<BE.Patente> GetAllPatentes()
        {
            string sql = "SELECT idPatente AS IdPatente, nombrePatente AS NombrePatente, descripcion AS Descripcion FROM dbo.Patente ORDER BY nombrePatente;";
            return db.QueryListAndLog<BE.Patente>(sql, null, "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentes, "Listado de patentes");
        }


        public List<BE.Familia> GetFamiliasByUsuario(int idUsuario)
        {
            string sql = @"
SELECT DISTINCT 
    f.idFamilia   AS IdFamilia,
    f.nombreFamilia AS NombreFamilia,
    f.descripcion AS Descripcion
FROM dbo.UsuarioFamilia uf
JOIN dbo.Familia f ON f.idFamilia = uf.idFamilia
WHERE uf.idUsuario = @idUsuario;";

            return db.QueryListAndLog<BE.Familia>(
                sql,
                cmd => cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario,
                "dbo.Familia", "idFamilia",
                BE.Audit.AuditEvents.ConsultaFamiliasPorUsuario,
                "Familias por usuario Id=" + idUsuario
            );
        }


        public List<BE.Patente> GetPatentesByUsuario(int idUsuario)
        {
            string sql = @"
SELECT DISTINCT
    p.idPatente   AS IdPatente,
    p.nombrePatente AS NombrePatente,
    p.descripcion AS Descripcion
FROM dbo.UsuarioPatente up
JOIN dbo.Patente p ON p.idPatente = up.idPatente
WHERE up.idUsuario = @idUsuario

UNION

SELECT DISTINCT
    p.idPatente   AS IdPatente,
    p.nombrePatente AS NombrePatente,
    p.descripcion AS Descripcion
FROM dbo.UsuarioFamilia uf
JOIN dbo.FamiliaPatente fp ON fp.idFamilia = uf.idFamilia
JOIN dbo.Patente p ON p.idPatente = fp.idPatente
WHERE uf.idUsuario = @idUsuario;";

            return db.QueryListAndLog<BE.Patente>(
                sql,
                cmd => cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario,
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorUsuario,
                "Patentes por usuario Id=" + idUsuario
            );
        }

        public List<BE.Patente> GetPatentesByFamilia(int idFamilia)
        {
            string sql = @"
SELECT DISTINCT
    p.idPatente   AS IdPatente,
    p.nombrePatente AS NombrePatente,
    p.descripcion AS Descripcion
FROM dbo.FamiliaPatente fp
JOIN dbo.Patente p ON p.idPatente = fp.idPatente
WHERE fp.idFamilia = @idFamilia;";

            return db.QueryListAndLog<BE.Patente>(
                sql,
                cmd => cmd.Parameters.Add("@idFamilia", SqlDbType.Int).Value = idFamilia,
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorFamilia,
                "Patentes por familia Id=" + idFamilia
            );
        }

        public void SetFamiliasForUsuario(int idUsuario, IEnumerable<int> idsFamilia)
        {
            var ids = (idsFamilia ?? Enumerable.Empty<int>()).Distinct().ToList();

            string del = "DELETE FROM dbo.UsuarioFamilia WHERE idUsuario = @u;";
            db.ExecuteNonQueryAndLog(del, c => c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario,
                "dbo.UsuarioFamilia", "idUsuario",
                BE.Audit.AuditEvents.ModificarFamiliasUsuario,
                "Limpiar familias del usuario Id=" + idUsuario);

            if (ids.Count == 0) return;

            var values = string.Join(", ", ids.Select((_, i) => $"(@u, @f{i})"));
            var ins = $"INSERT INTO dbo.UsuarioFamilia (idUsuario, idFamilia) VALUES {values};";

            db.ExecuteNonQueryAndLog(ins, c =>
            {
                c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
                for (int i = 0; i < ids.Count; i++)
                    c.Parameters.Add($"@f{i}", SqlDbType.Int).Value = ids[i];
            }, "dbo.UsuarioFamilia", "idUsuario",
            BE.Audit.AuditEvents.ModificarFamiliasUsuario,
            "Asignar familias al usuario Id=" + idUsuario, shouldCalculate: true);
        }

        public void SetPatentesForUsuario(int idUsuario, IEnumerable<int> idsPatente)
        {
            var ids = (idsPatente ?? Enumerable.Empty<int>()).Distinct().ToList();

            string del = "DELETE FROM dbo.UsuarioPatente WHERE idUsuario = @u;";
            db.ExecuteNonQueryAndLog(del, c => c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario,
                "dbo.UsuarioPatente", "idUsuario",
                BE.Audit.AuditEvents.ModificarPatentesUsuario,
                "Limpiar patentes del usuario Id=" + idUsuario);

            if (ids.Count == 0) return;

            var values = string.Join(", ", ids.Select((_, i) => $"(@u, @p{i})"));
            var ins = $"INSERT INTO dbo.UsuarioPatente (idUsuario, idPatente) VALUES {values};";

            db.ExecuteNonQueryAndLog(ins, c =>
            {
                c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
                for (int i = 0; i < ids.Count; i++)
                    c.Parameters.Add($"@p{i}", SqlDbType.Int).Value = ids[i];
            }, "dbo.UsuarioPatente", "idUsuario",
            BE.Audit.AuditEvents.ModificarPatentesUsuario,
            "Asignar patentes al usuario Id=" + idUsuario, shouldCalculate: true);
        }
    }
}

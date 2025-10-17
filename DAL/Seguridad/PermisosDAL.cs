using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    f.idFamilia     AS IdFamilia,
    f.nombreFamilia AS NombreFamilia,
    f.descripcion   AS Descripcion
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
    p.idPatente      AS IdPatente,
    p.nombrePatente  AS NombrePatente,
    p.descripcion    AS Descripcion
FROM dbo.UsuarioPatente up
JOIN dbo.Patente p ON p.idPatente = up.idPatente
WHERE up.idUsuario = @idUsuario
UNION
SELECT DISTINCT
    p.idPatente      AS IdPatente,
    p.nombrePatente  AS NombrePatente,
    p.descripcion    AS Descripcion
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
    p.idPatente      AS IdPatente,
    p.nombrePatente  AS NombrePatente,
    p.descripcion    AS Descripcion
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

        public List<int> GetUsuariosConPatente(int idPatente)
        {
            string sql = @"
SELECT DISTINCT u.idUsuario
FROM dbo.Usuario u
LEFT JOIN dbo.UsuarioPatente up
       ON up.idUsuario = u.idUsuario AND up.idPatente = @p
LEFT JOIN dbo.UsuarioFamilia uf
       ON uf.idUsuario = u.idUsuario
LEFT JOIN dbo.FamiliaPatente fp
       ON fp.idFamilia = uf.idFamilia AND fp.idPatente = @p
WHERE up.idPatente IS NOT NULL OR fp.idPatente IS NOT NULL;";

            return db.QueryListAndLog<int>(
                sql,
                c => c.Parameters.Add("@p", SqlDbType.Int).Value = idPatente,
                "dbo.Usuario", "idUsuario",
                BE.Audit.AuditEvents.ConsultaUsuariosPorPatente,
                "Usuarios con patente Id=" + idPatente
            );
        }

        public List<BE.Patente> GetPatentesDirectasByUsuario(int idUsuario)
        {
            string sql = @"
SELECT DISTINCT 
    p.idPatente      AS IdPatente, 
    p.nombrePatente  AS NombrePatente, 
    p.descripcion    AS Descripcion
FROM dbo.UsuarioPatente up
JOIN dbo.Patente p ON p.idPatente = up.idPatente
WHERE up.idUsuario = @u;";
            return db.QueryListAndLog<BE.Patente>(
                sql,
                c => c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario,
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorUsuario,
                "Patentes directas por usuario Id=" + idUsuario
            );
        }

        public List<BE.Patente> GetPatentesPorFamilias(IEnumerable<int> idsFamilia)
        {
            var list = (idsFamilia ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (list.Count == 0) return new List<BE.Patente>();

            var placeholders = string.Join(", ", list.Select((_, i) => $"@f{i}"));
            string sql = $@"
SELECT DISTINCT 
    p.idPatente      AS IdPatente, 
    p.nombrePatente  AS NombrePatente, 
    p.descripcion    AS Descripcion
FROM dbo.FamiliaPatente fp
JOIN dbo.Patente p ON p.idPatente = fp.idPatente
WHERE fp.idFamilia IN ({placeholders});";

            return db.QueryListAndLog<BE.Patente>(
                sql,
                c =>
                {
                    for (int i = 0; i < list.Count; i++)
                        c.Parameters.Add($"@f{i}", SqlDbType.Int).Value = list[i];
                },
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorFamilia,
                "Patentes por lista de familias"
            );
        }


        public void SetFamiliasForUsuario(int idUsuario, IEnumerable<int> idsFamilia)
        {
            var nuevasFamilias = (idsFamilia ?? Enumerable.Empty<int>()).Distinct().ToList();

            // Validación previa (rápida): no dejar huérfanas patentes que perdería este usuario
            var patentesOld = new HashSet<int>(GetPatentesByUsuario(idUsuario).Select(p => p.IdPatente));
            var patDirectas = GetPatentesDirectasByUsuario(idUsuario) ?? new List<BE.Patente>();
            var patPorFamilias = GetPatentesPorFamilias(nuevasFamilias) ?? new List<BE.Patente>();
            var patentesNew = new HashSet<int>(
                patDirectas.Select(p => p.IdPatente)
                           .Concat(patPorFamilias.Select(p => p.IdPatente)));

            var patentesPerdidas = patentesOld.Except(patentesNew).ToList();
            foreach (var idPat in patentesPerdidas)
            {
                var otros = new HashSet<int>(GetUsuariosConPatente(idPat));
                otros.Remove(idUsuario);
                if (otros.Count == 0)
                    throw new InvalidOperationException(
                        "No se puede quitar la patente Id=" + idPat + " porque quedaría sin ningún usuario asignado.");
            }

            string core;
            Action<SqlCommand> binder;

            if (nuevasFamilias.Count == 0)
            {
                core = "DELETE FROM dbo.UsuarioFamilia WHERE idUsuario = @u;";
                binder = c => c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
            }
            else
            {
                var values = string.Join(", ", nuevasFamilias.Select((_, i) => $"(@u, @f{i})"));
                core = $"DELETE FROM dbo.UsuarioFamilia WHERE idUsuario = @u; " +
                       $"INSERT INTO dbo.UsuarioFamilia (idUsuario, idFamilia) VALUES {values};";

                binder = c =>
                {
                    c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
                    for (int i = 0; i < nuevasFamilias.Count; i++)
                        c.Parameters.Add($"@f{i}", SqlDbType.Int).Value = nuevasFamilias[i];
                };
            }

            var sql = @"
BEGIN TRY
    BEGIN TRAN;

    " + core + @"

    IF EXISTS (
        SELECT 1
        FROM dbo.Patente p
        WHERE NOT EXISTS (SELECT 1 FROM dbo.UsuarioPatente up WHERE up.idPatente = p.idPatente)
          AND NOT EXISTS (
              SELECT 1
              FROM dbo.UsuarioFamilia uf
              JOIN dbo.FamiliaPatente fp ON fp.idFamilia = uf.idFamilia
              WHERE fp.idPatente = p.idPatente
          )
    )
    BEGIN
        ROLLBACK TRAN;
        THROW 51001, 'No se puede dejar una patente sin usuarios asignados.', 1;
    END

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF (XACT_STATE() <> 0) ROLLBACK TRAN;
    THROW;
END CATCH;";

            db.ExecuteNonQueryAndLog(
                sql,
                binder,
                "dbo.UsuarioFamilia", "idUsuario",
                BE.Audit.AuditEvents.ModificarFamiliasUsuario,
                "Actualizar familias (protección global no-orfandad de patentes). Usuario Id=" + idUsuario,
                shouldCalculate: true
            );
        }


        public void SetPatentesForUsuario(int idUsuario, IEnumerable<int> idsPatente)
        {
            var nuevasPatentes = (idsPatente ?? Enumerable.Empty<int>()).Distinct().ToList();

            // Validación previa (rápida)
            var patentesOld = new HashSet<int>(GetPatentesByUsuario(idUsuario).Select(p => p.IdPatente));
            var familiasActuales = GetFamiliasByUsuario(idUsuario) ?? new List<BE.Familia>();
            var patPorFamilias = GetPatentesPorFamilias(familiasActuales.Select(f => f.IdFamilia)) ?? new List<BE.Patente>();
            var patentesNew = new HashSet<int>(nuevasPatentes.Concat(patPorFamilias.Select(p => p.IdPatente)));

            var patentesPerdidas = patentesOld.Except(patentesNew).ToList();
            foreach (var idPat in patentesPerdidas)
            {
                var otros = new HashSet<int>(GetUsuariosConPatente(idPat));
                otros.Remove(idUsuario);
                if (otros.Count == 0)
                    throw new InvalidOperationException(
                        "No se puede quitar la patente Id=" + idPat + " porque quedaría sin ningún usuario asignado.");
            }

            string core;
            Action<SqlCommand> binder;

            if (nuevasPatentes.Count == 0)
            {
                core = "DELETE FROM dbo.UsuarioPatente WHERE idUsuario = @u;";
                binder = c => c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
            }
            else
            {
                var values = string.Join(", ", nuevasPatentes.Select((_, i) => $"(@u, @p{i})"));
                core = $"DELETE FROM dbo.UsuarioPatente WHERE idUsuario = @u; " +
                       $"INSERT INTO dbo.UsuarioPatente (idUsuario, idPatente) VALUES {values};";

                binder = c =>
                {
                    c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
                    for (int i = 0; i < nuevasPatentes.Count; i++)
                        c.Parameters.Add($"@p{i}", SqlDbType.Int).Value = nuevasPatentes[i];
                };
            }

            var sql = @"
BEGIN TRY
    BEGIN TRAN;

    " + core + @"

    IF EXISTS (
        SELECT 1
        FROM dbo.Patente p
        WHERE NOT EXISTS (SELECT 1 FROM dbo.UsuarioPatente up WHERE up.idPatente = p.idPatente)
          AND NOT EXISTS (
              SELECT 1
              FROM dbo.UsuarioFamilia uf
              JOIN dbo.FamiliaPatente fp ON fp.idFamilia = uf.idFamilia
              WHERE fp.idPatente = p.idPatente
          )
    )
    BEGIN
        ROLLBACK TRAN;
        THROW 51001, 'No se puede dejar una patente sin usuarios asignados.', 1;
    END

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF (XACT_STATE() <> 0) ROLLBACK TRAN;
    THROW;
END CATCH;";

            db.ExecuteNonQueryAndLog(
                sql,
                binder,
                "dbo.UsuarioPatente", "idUsuario",
                BE.Audit.AuditEvents.ModificarPatentesUsuario,
                "Actualizar patentes (protección global no-orfandad de patentes). Usuario Id=" + idUsuario,
                shouldCalculate: true
            );
        }

    }
}

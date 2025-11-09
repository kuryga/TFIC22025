using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using segUtils = DAL.Seguridad.SecurityUtilities;

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

        private static string EncName(string s)
        {
            var t = (s ?? string.Empty).Trim();
            return string.IsNullOrEmpty(t) ? null : segUtils.EncriptarReversible(t);
        }

        private static string DecName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            try
            {
                var d = segUtils.DesencriptarReversible(s);
                return string.IsNullOrEmpty(d) ? null : d;
            }
            catch
            {
                return s;
            }
        }

        public List<BE.Familia> GetAllFamilias()
        {
            string sql = "SELECT idFamilia AS IdFamilia, nombreFamilia AS NombreFamilia, descripcion AS Descripcion FROM dbo.Familia ORDER BY nombreFamilia;";
            var list = db.QueryListAndLog<BE.Familia>(sql, null, "dbo.Familia", "idFamilia",
                BE.Audit.AuditEvents.ConsultaFamilias, "Listado de familias");

            foreach (var f in list ?? Enumerable.Empty<BE.Familia>())
                f.NombreFamilia = DecName(f.NombreFamilia);

            return list;
        }

        public List<BE.Patente> GetAllPatentes()
        {
            string sql = "SELECT idPatente AS IdPatente, nombrePatente AS NombrePatente, descripcion AS Descripcion FROM dbo.Patente ORDER BY nombrePatente;";
            var list = db.QueryListAndLog<BE.Patente>(sql, null, "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentes, "Listado de patentes");

            foreach (var p in list ?? Enumerable.Empty<BE.Patente>())
                p.NombrePatente = DecName(p.NombrePatente);

            return list;
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
            var list = db.QueryListAndLog<BE.Familia>(
                sql,
                cmd => cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario,
                "dbo.Familia", "idFamilia",
                BE.Audit.AuditEvents.ConsultaFamiliasPorUsuario,
                "Familias por usuario Id=" + idUsuario
            );

            foreach (var f in list ?? Enumerable.Empty<BE.Familia>())
                f.NombreFamilia = DecName(f.NombreFamilia);

            return list;
        }

        public List<BE.Patente> GetPatentesByUsuario(int idUsuario)
        {
            string sql = @"
SELECT DISTINCT
    p.idPatente     AS IdPatente,
    p.nombrePatente AS NombrePatente,
    p.descripcion   AS Descripcion
FROM dbo.UsuarioPatente up
JOIN dbo.Patente p ON p.idPatente = up.idPatente
WHERE up.idUsuario = @idUsuario

UNION

SELECT DISTINCT
    p.idPatente     AS IdPatente,
    p.nombrePatente AS NombrePatente,
    p.descripcion   AS Descripcion
FROM dbo.UsuarioFamilia uf
JOIN dbo.FamiliaPatente fp ON fp.idFamilia = uf.idFamilia
JOIN dbo.Patente p ON p.idPatente = fp.idPatente
WHERE uf.idUsuario = @idUsuario

ORDER BY NombrePatente;";
            var list = db.QueryListAndLog<BE.Patente>(
                sql,
                cmd => cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario,
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorUsuario,
                "Patentes directas y por familia del usuario Id=" + idUsuario
            );

            foreach (var p in list ?? Enumerable.Empty<BE.Patente>())
                p.NombrePatente = DecName(p.NombrePatente);

            return list;
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
            var list = db.QueryListAndLog<BE.Patente>(
                sql,
                cmd => cmd.Parameters.Add("@idFamilia", SqlDbType.Int).Value = idFamilia,
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorFamilia,
                "Patentes por familia Id=" + idFamilia
            );

            foreach (var p in list ?? Enumerable.Empty<BE.Patente>())
                p.NombrePatente = DecName(p.NombrePatente);

            return list;
        }

        public List<int> GetUsuariosConPatente(int idPatente)
        {
            string sql = @"
SELECT DISTINCT u.idUsuario
FROM dbo.Usuario u
WHERE EXISTS (
    SELECT 1
    FROM dbo.UsuarioPatente up
    WHERE up.idUsuario = u.idUsuario
      AND up.idPatente = @p
)
OR EXISTS (
    SELECT 1
    FROM dbo.UsuarioFamilia uf
    JOIN dbo.FamiliaPatente fp ON fp.idFamilia = uf.idFamilia
    WHERE uf.idUsuario = u.idUsuario
      AND fp.idPatente = @p
);";
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
            var list = db.QueryListAndLog<BE.Patente>(
                sql,
                c => c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario,
                "dbo.Patente", "idPatente",
                BE.Audit.AuditEvents.ConsultaPatentesPorUsuario,
                "Patentes directas por usuario Id=" + idUsuario
            );

            foreach (var p in list ?? Enumerable.Empty<BE.Patente>())
                p.NombrePatente = DecName(p.NombrePatente);

            return list;
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
            var res = db.QueryListAndLog<BE.Patente>(
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

            foreach (var p in res ?? Enumerable.Empty<BE.Patente>())
                p.NombrePatente = DecName(p.NombrePatente);

            return res;
        }

        public void SetFamiliasForUsuario(int idUsuario, IEnumerable<int> idsFamilia)
        {
            var nuevasFamilias = (idsFamilia ?? Enumerable.Empty<int>()).Distinct().ToList();

            var patentesOld = new HashSet<int>(GetPatentesByUsuario(idUsuario).Select(p => p.IdPatente));
            var patDirectas = GetPatentesDirectasByUsuario(idUsuario) ?? new List<BE.Patente>();
            var patPorFamilias = GetPatentesPorFamilias(nuevasFamilias) ?? new List<BE.Patente>();
            var patentesNew = new HashSet<int>(
                patDirectas.Select(p => p.IdPatente)
                           .Concat(patPorFamilias.Select(p => p.IdPatente))
            );

            var patentesPerdidas = patentesOld.Except(patentesNew).ToList();
            foreach (var idPat in patentesPerdidas)
            {
                var otros = new HashSet<int>(GetUsuariosConPatente(idPat));
                otros.Remove(idUsuario);

                if (otros.Count == 0)
                {
                    Audit.BitacoraDAL.GetInstance().Log(
                        BE.Audit.AuditEvents.EliminacionPatentesCriticaUsuario,
                        $"Intento de dejar patente Id={idPat} sin usuarios asignados (Usuario={idUsuario})."
                    );

                    throw new InvalidOperationException(
                        $"No se puede quitar la patente Id={idPat} porque quedaría sin ningún usuario asignado."
                    );
                }
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

        private sealed class _FamPatRow
        {
            public int idFamilia { get; set; }
            public int idPatente { get; set; }
        }

        public void SetPatentesForUsuario(int idUsuario, IEnumerable<int> idsPatente)
        {
            var nuevasPatentes = (idsPatente ?? Enumerable.Empty<int>()).Distinct().ToList();
            var patentesOld = new HashSet<int>(GetPatentesByUsuario(idUsuario).Select(p => p.IdPatente));

            var familiasActuales = GetFamiliasByUsuario(idUsuario) ?? new List<BE.Familia>();
            var idsFamiliasActuales = familiasActuales.Select(f => f.IdFamilia).Distinct().ToList();

            var patPorFamiliasActuales = idsFamiliasActuales.Count == 0
                ? new List<BE.Patente>()
                : (GetPatentesPorFamilias(idsFamiliasActuales) ?? new List<BE.Patente>());

            var patentesNew = new HashSet<int>(
                nuevasPatentes.Concat(patPorFamiliasActuales.Select(p => p.IdPatente))
            );

            var patentesPerdidas = patentesOld.Except(patentesNew).ToList();
            foreach (var idPat in patentesPerdidas)
            {
                var otros = new HashSet<int>(GetUsuariosConPatente(idPat));
                otros.Remove(idUsuario);
                if (otros.Count == 0)
                {
                    Audit.BitacoraDAL.GetInstance().Log(
                        BE.Audit.AuditEvents.EliminacionPatentesCriticaUsuario,
                        $"Intento de dejar patente Id={idPat} sin usuarios asignados (Usuario={idUsuario})."
                    );
                    throw new InvalidOperationException(
                        $"No se puede quitar la patente Id={idPat} porque quedaría sin ningún usuario asignado."
                    );
                }
            }

            string core;
            Action<SqlCommand> binder;

            if (nuevasPatentes.Count == 0)
            {
                core = "DELETE FROM dbo.UsuarioPatente WHERE idUsuario = @u;";
                binder = c =>
                {
                    c.Parameters.Add("@u", SqlDbType.Int).Value = idUsuario;
                };
            }
            else
            {
                var values = string.Join(", ", nuevasPatentes.Select((_, i) => $"(@u, @p{i})"));
                core = "DELETE FROM dbo.UsuarioPatente WHERE idUsuario = @u; " +
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
                "Actualizar patentes directas. Usuario Id=" + idUsuario,
                shouldCalculate: true
            );
        }

        public int CreateFamilia(BE.Familia familia, IEnumerable<int> idsPatente)
        {
            if (familia == null) throw new ArgumentNullException(nameof(familia));

            var nPlain = (familia.NombreFamilia ?? string.Empty).Trim();
            var nEnc = EncName(nPlain) ?? string.Empty;

            var sqlInsert = @"
BEGIN TRY
    BEGIN TRAN;

    IF EXISTS (
        SELECT 1
        FROM dbo.Familia
        WHERE UPPER(LTRIM(RTRIM(nombreFamilia))) = UPPER(LTRIM(RTRIM(@nPlain)))
           OR nombreFamilia = @nEnc
    )
    BEGIN
        ROLLBACK TRAN;
        THROW 51010, 'Ya existe una familia con ese nombre.', 1;
    END

    INSERT INTO dbo.Familia (nombreFamilia, descripcion)
    VALUES (@nEnc, @d);
    SELECT CAST(SCOPE_IDENTITY() AS int);

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF (XACT_STATE() <> 0) ROLLBACK TRAN;
    THROW;
END CATCH;";

            object newId = db.ExecuteScalarAndLog(
                sqlInsert,
                c =>
                {
                    c.Parameters.Add("@nPlain", SqlDbType.NVarChar, 100).Value = (object)nPlain ?? DBNull.Value;
                    c.Parameters.Add("@nEnc", SqlDbType.VarChar, 256).Value = (object)nEnc ?? DBNull.Value;
                    c.Parameters.Add("@d", SqlDbType.NVarChar, 255).Value = (object)(familia.Descripcion ?? string.Empty) ?? DBNull.Value;
                },
                "dbo.Familia", "idFamilia",
                BE.Audit.AuditEvents.AltaFamilia,
                "Alta de familia: " + nPlain,
                shouldCalculate: false
            );

            int idNueva = (newId != null && newId != DBNull.Value) ? Convert.ToInt32(newId) : 0;
            if (idNueva > 0)
            {
                familia.IdFamilia = idNueva;
                familia.NombreFamilia = nPlain;

                db.RefreshRowDvAndTableDvv("dbo.Familia", "idFamilia", idNueva, false);
            }

            var patList = (idsPatente ?? Enumerable.Empty<int>()).Distinct().ToList();
            if (idNueva > 0 && patList.Count > 0)
            {
                var values = string.Join(", ", patList.Select((_, i) => $"(@f, @p{i})"));
                string sqlRel = $"INSERT INTO dbo.FamiliaPatente (idFamilia, idPatente) VALUES {values};";

                db.ExecuteNonQueryAndLog(
                    sqlRel,
                    c =>
                    {
                        c.Parameters.Add("@f", SqlDbType.Int).Value = idNueva;
                        for (int i = 0; i < patList.Count; i++)
                            c.Parameters.Add($"@p{i}", SqlDbType.Int).Value = patList[i];
                    },
                    "dbo.FamiliaPatente", "idFamilia",
                    BE.Audit.AuditEvents.AsignarPatentesAFamilia,
                    "Asignación inicial de patentes a familia Id=" + idNueva,
                    shouldCalculate: true
                );
            }

            return idNueva;
        }

        public void UpdateFamilia(BE.Familia familia, IEnumerable<int> idsPatente)
        {
            if (familia == null) throw new ArgumentNullException(nameof(familia));
            if (familia.IdFamilia <= 0) throw new ArgumentException("IdFamilia inválido.", nameof(familia));

            var nuevasPatentes = (idsPatente ?? Enumerable.Empty<int>()).Distinct().ToList();

            var nPlain = (familia.NombreFamilia ?? string.Empty).Trim();
            var nEnc = EncName(nPlain) ?? string.Empty;

            string core;
            Action<SqlCommand> binder;

            if (nuevasPatentes.Count == 0)
            {
                core = @"
UPDATE dbo.Familia
   SET nombreFamilia = @nEnc, descripcion = @d
 WHERE idFamilia = @f;

DELETE FROM dbo.FamiliaPatente WHERE idFamilia = @f;";
                binder = c =>
                {
                    c.Parameters.Add("@f", SqlDbType.Int).Value = familia.IdFamilia;
                    c.Parameters.Add("@nEnc", SqlDbType.VarChar, 256).Value = (object)nEnc ?? DBNull.Value;
                    c.Parameters.Add("@d", SqlDbType.NVarChar, 255).Value = (object)(familia.Descripcion ?? string.Empty) ?? DBNull.Value;
                };
            }
            else
            {
                var values = string.Join(", ", nuevasPatentes.Select((_, i) => $"(@f, @p{i})"));
                core = $@"
UPDATE dbo.Familia
   SET nombreFamilia = @nEnc, descripcion = @d
 WHERE idFamilia = @f;

DELETE FROM dbo.FamiliaPatente WHERE idFamilia = @f;
INSERT INTO dbo.FamiliaPatente (idFamilia, idPatente) VALUES {values};";

                binder = c =>
                {
                    c.Parameters.Add("@f", SqlDbType.Int).Value = familia.IdFamilia;
                    c.Parameters.Add("@nEnc", SqlDbType.VarChar, 256).Value = (object)nEnc ?? DBNull.Value;
                    c.Parameters.Add("@d", SqlDbType.NVarChar, 255).Value = (object)(familia.Descripcion ?? string.Empty) ?? DBNull.Value;
                    for (int i = 0; i < nuevasPatentes.Count; i++)
                        c.Parameters.Add($"@p{i}", SqlDbType.Int).Value = nuevasPatentes[i];
                };
            }

            var sql = @"
BEGIN TRY
    BEGIN TRAN;

    IF EXISTS (
        SELECT 1
        FROM dbo.Familia
        WHERE (UPPER(LTRIM(RTRIM(nombreFamilia))) = UPPER(LTRIM(RTRIM(@nPlain)))
               OR nombreFamilia = @nEnc)
          AND idFamilia <> @f
    )
    BEGIN
        ROLLBACK TRAN;
        THROW 51011, 'Ya existe una familia con ese nombre.', 1;
    END

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
        THROW 51002, 'No se puede dejar una patente sin usuarios asignados (al modificar familia).', 1;
    END

    COMMIT TRAN;
END TRY
BEGIN CATCH
    IF (XACT_STATE() <> 0) ROLLBACK TRAN;
    THROW;
END CATCH;";

            db.ExecuteNonQueryAndLog(
                sql,
                c =>
                {
                    binder(c);
                    c.Parameters.Add("@nPlain", SqlDbType.NVarChar, 100).Value = (object)nPlain ?? DBNull.Value;
                },
                "dbo.FamiliaPatente",
                "idFamilia",
                BE.Audit.AuditEvents.ModificacionFamilia,
                "Modificación de familia Id=" + familia.IdFamilia,
                shouldCalculate: true
            );

            familia.NombreFamilia = nPlain;
            db.RefreshRowDvAndTableDvv("dbo.Familia", "idFamilia", familia.IdFamilia, false);
        }
    }
}

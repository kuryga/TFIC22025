using System.Collections.Generic;
using System.Data;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Maquinaria>;

namespace DAL.Genericos
{
    public class MaquinariaDAL : DaoInterface
    {
        private static MaquinariaDAL instance;
        private MaquinariaDAL() { }
        public static MaquinariaDAL GetInstance()
        {
            if (instance == null)
                instance = new MaquinariaDAL();
            return instance;
        }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.Maquinaria";
        private const string idCol = "idMaquinaria";
        private const string publicCols = "idMaquinaria, nombre, costoPorHora, deshabilitado";

        public List<BE.Maquinaria> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";

            return db.QueryListAndLog<BE.Maquinaria>(
                sql,
                null,
                table, idCol,
                BE.Audit.AuditEvents.ConsultaMaquinarias,
                "Listado de maquinarias",
                false
            );
        }

        public void Create(BE.Maquinaria obj)
        {
            var sql = @"
INSERT INTO " + table + @" (nombre, costoPorHora)
VALUES (@nombre, @costoPorHora);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value =
                        (object)(obj.Nombre ?? string.Empty) ?? System.DBNull.Value;

                    var p = cmd.Parameters.Add("@costoPorHora", SqlDbType.Decimal);
                    p.Precision = 18;
                    p.Scale = 2;
                    p.Value = obj.CostoPorHora;
                },
                table, idCol,
                BE.Audit.AuditEvents.CreacionMaquinaria,
                "Alta de maquinaria: " + (obj.Nombre ?? string.Empty),
                false
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdMaquinaria = System.Convert.ToInt32(newId);

            if (obj.IdMaquinaria > 0)
                db.RefreshRowDvAndTableDvv(table, idCol, obj.IdMaquinaria, false);
        }

        public void Update(BE.Maquinaria obj)
        {
            var sql = @"
UPDATE " + table + @"
   SET nombre = @nombre,
       costoPorHora = @costoPorHora
 WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdMaquinaria;

                    cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value =
                        (object)(obj.Nombre ?? string.Empty) ?? System.DBNull.Value;

                    var p = cmd.Parameters.Add("@costoPorHora", SqlDbType.Decimal);
                    p.Precision = 18;
                    p.Scale = 2;
                    p.Value = obj.CostoPorHora;
                },
                table, idCol, obj.IdMaquinaria,
                BE.Audit.AuditEvents.ModificacionMaquinaria,
                "Modificación de maquinaria Id=" + obj.IdMaquinaria,
                true
            );
        }

        public void Deshabilitar(int idMaquinaria, bool deshabilitar)
        {
            var sql = @"
UPDATE " + table + @"
   SET deshabilitado = @deshabilitado
 WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idMaquinaria;
                    cmd.Parameters.Add("@deshabilitado", SqlDbType.Bit).Value = deshabilitar;
                },
                table, idCol, idMaquinaria,
                deshabilitar
                    ? BE.Audit.AuditEvents.DeshabilitacionMaquinaria
                    : BE.Audit.AuditEvents.HabilitacionMaquinaria,
                (deshabilitar ? "Deshabilitación" : "Habilitación") + " de maquinaria Id=" + idMaquinaria,
                true
            );

            db.RefreshRowDvAndTableDvv(table, idCol, idMaquinaria, false);
        }
    }
}

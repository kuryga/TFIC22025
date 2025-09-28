using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.TipoEdificacion>;

namespace DAL.Genericos
{
    public class TipoEdificacionDAL : DaoInterface
    {
        private static TipoEdificacionDAL instance;
        private TipoEdificacionDAL() { }
        public static TipoEdificacionDAL GetInstance() { if (instance == null) instance = new TipoEdificacionDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.TipoEdificacion";
        private const string idCol = "idTipoEdificacion";
        private const string publicCols = "idTipoEdificacion, descripcion";

        public List<BE.TipoEdificacion> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndLog<BE.TipoEdificacion>(
                sql,
                null,
                table, idCol,
                BE.Audit.AuditEvents.ConsultaTiposEdificacion,
                "Listado de tipos de edificación"
            );
        }

        public void Create(BE.TipoEdificacion obj)
        {
            var sql = @"
INSERT INTO " + table + @" (descripcion)
VALUES (@descripcion);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value =
                        (object)obj.Descripcion ?? System.DBNull.Value;
                },
                table, idCol,
                BE.Audit.AuditEvents.CreacionTipoEdificacion,
                "Alta de tipo de edificación: " + (obj.Descripcion ?? string.Empty)
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdTipoEdificacion = System.Convert.ToInt32(newId);
        }

        public void Update(BE.TipoEdificacion obj)
        {
            var sql = @"
UPDATE " + table + @"
   SET descripcion = @descripcion
 WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdTipoEdificacion;
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value =
                        (object)obj.Descripcion ?? System.DBNull.Value;
                },
                table, idCol,
                BE.Audit.AuditEvents.ModificacionTipoEdificacion,
                "Modificación de tipo de edificación Id=" + obj.IdTipoEdificacion
            );
        }
    }
}

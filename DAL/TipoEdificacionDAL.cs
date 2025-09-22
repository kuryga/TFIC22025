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
            return db.QueryListAndUpdateDv<BE.TipoEdificacion>(sql, null, table, idCol);
        }

        public void Create(BE.TipoEdificacion obj)
        {
            var sql = @"INSERT INTO " + table + @" ( descripcion )
                        VALUES ( @descripcion );
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value = (object)obj.Descripcion ?? System.DBNull.Value;
                },
                table, idCol
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdTipoEdificacion = System.Convert.ToInt32(newId);
        }

        public void Update(BE.TipoEdificacion obj)
        {
            var sql = @"UPDATE " + table + 
                      @" SET descripcion = @descripcion,
                      WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdTipoEdificacion;
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value = (object)obj.Descripcion ?? System.DBNull.Value;
                },
                table, idCol
            );
        }
    }
}

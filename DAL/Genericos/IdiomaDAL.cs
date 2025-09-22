using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Idioma>;

namespace DAL.Genericos
{
    public class IdiomaDAL : DaoInterface
    {
        private static IdiomaDAL instance;
        private IdiomaDAL() { }
        public static IdiomaDAL GetInstance() { if (instance == null) instance = new IdiomaDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.Idioma";
        private const string idCol = "idIdioma";
        private const string publicCols = "idIdioma, nombre, codigoISO";

        public List<BE.Idioma> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndUpdateDv<BE.Idioma>(sql, null, table, idCol);
        }

        public void Create(BE.Idioma obj)
        {
            var sql = @"INSERT INTO " + table + @"( nombre, codigoISO )
                        VALUES ( @nombre, @codigoISO );
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? System.DBNull.Value;
                    cmd.Parameters.Add("@codigoISO", SqlDbType.VarChar, 10).Value = (object)obj.CodigoISO ?? System.DBNull.Value;
                },
                table, idCol
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdIdioma = System.Convert.ToInt32(newId);
        }

        public void Update(BE.Idioma obj)
        {
            var sql = @"UPDATE " + table + 
                      @" SET
                            nombre    = @nombre,
                            codigoISO = @codigoISO
                      WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdIdioma;
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? System.DBNull.Value;
                    cmd.Parameters.Add("@codigoISO", SqlDbType.VarChar, 10).Value = (object)obj.CodigoISO ?? System.DBNull.Value;
                },
                table, idCol
            );
        }
    }
}

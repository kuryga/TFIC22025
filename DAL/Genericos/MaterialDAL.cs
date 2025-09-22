using System.Collections.Generic;
using System.Data;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Material>;

namespace DAL.Genericos
{
    public class MaterialDAL : DaoInterface
    {
        private static MaterialDAL instance;
        private MaterialDAL() { }
        public static MaterialDAL GetInstance() { if (instance == null) instance = new MaterialDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.Material";
        private const string idCol = "idMaterial";
        private const string publicCols = "idMaterial, nombre, unidadMedida, precioUnidad, usoPorM2";

        public List<BE.Material> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndUpdateDv<BE.Material>(sql, null, table, idCol);
        }

        public void Create(BE.Material obj)
        {
            var sql = @"INSERT INTO " + table + @"( nombre, unidadMedida, precioUnidad, usoPorM2 )
                        VALUES( @nombre, @unidadMedida, @precioUnidad, @usoPorM2 );
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? System.DBNull.Value;
                    cmd.Parameters.Add("@unidadMedida", SqlDbType.VarChar, 250).Value = (object)obj.UnidadMedida ?? System.DBNull.Value;
                    cmd.Parameters.Add("@precioUnidad", SqlDbType.Decimal).Value = obj.PrecioUnidad;
                    cmd.Parameters.Add("@usoPorM2", SqlDbType.Decimal).Value = obj.UsoPorM2;
                },
                table, idCol
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdMaterial = System.Convert.ToInt32(newId);
        }

        public void Update(BE.Material obj)
        {
            var sql = @"UPDATE " + table +
                @" SET
                     nombre      = @nombre,
                     unidadMedida = @unidadMedida,
                     precioUnidad   = @precioUnidad,
                     usoPorM2   = @usoPorM2
                     WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? System.DBNull.Value;
                    cmd.Parameters.Add("@unidadMedida", SqlDbType.VarChar, 250).Value = (object)obj.UnidadMedida ?? System.DBNull.Value;
                    cmd.Parameters.Add("@precioUnidad", SqlDbType.Decimal).Value = obj.PrecioUnidad;
                    cmd.Parameters.Add("@usoPorM2", SqlDbType.Decimal).Value = obj.UsoPorM2;
                },
                table, idCol
            );
        }
    }
}

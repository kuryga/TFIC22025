using System.Collections.Generic;
using System.Data;


using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Maquinaria>;

namespace DAL.Genericos
{
    public class MaquinariaDAL : DaoInterface
    {
        private static MaquinariaDAL instance;
        private MaquinariaDAL() { }
        public static MaquinariaDAL GetInstance() { if (instance == null) instance = new MaquinariaDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.Maquinaria";
        private const string idCol = "idMaquinaria";
        private const string publicCols = "idMaquinaria, nombre, costoPorHora";

        public List<BE.Maquinaria> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndUpdateDv<BE.Maquinaria>(sql, null, table, idCol);
        }

        public void Create(BE.Maquinaria obj)
        {
            var sql = @"INSERT INTO " + table + @"( nombre, costoPorHora )
                        VALUES ( @nombre, @costoPorHora );
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? System.DBNull.Value;
                    cmd.Parameters.Add("@costoPorHora", SqlDbType.Decimal, 250).Value = (object)obj.CostoPorHora ?? System.DBNull.Value;
                },
                table, idCol
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdMaquinaria = System.Convert.ToInt32(newId);
        }

        public void Update(BE.Maquinaria obj)
        {
            var sql = @"UPDATE " + table +
                @" SET
                    nombre      = @nombre,
                    costoPorHora = @costoPorHora
                WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdMaquinaria;
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value = (object)obj.Nombre ?? System.DBNull.Value;
                    cmd.Parameters.Add("@costoPorHora", SqlDbType.Decimal, 250).Value = (object)obj.CostoPorHora ?? System.DBNull.Value;
                },
                table, idCol
            );
        }
    }
}

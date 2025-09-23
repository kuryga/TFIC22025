using System.Collections.Generic;
using System.Data;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.ServicioAdicional>;

namespace DAL.Genericos
{
    public class ServicioAdicionalDAL : DaoInterface
    {
        private static ServicioAdicionalDAL instance;
        private ServicioAdicionalDAL() { }
        public static ServicioAdicionalDAL GetInstance() { if (instance == null) instance = new ServicioAdicionalDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.ServicioAdicional";
        private const string idCol = "idServicio";
        private const string publicCols = "idServicio, descripcion, precio";

        public List<BE.ServicioAdicional> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndUpdateDv<BE.ServicioAdicional>(sql, null, table, idCol);
        }

        public void Create(BE.ServicioAdicional obj)
        {
            var sql = @"INSERT INTO " + table + @"( descripcion, precio )
                        VALUES (@descripcion, @precio );
                        SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value = (object)obj.Descripcion ?? System.DBNull.Value;
                    cmd.Parameters.Add("@precio", SqlDbType.Decimal).Value = obj.Precio;
                },
                table, idCol
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdServicio = System.Convert.ToInt32(newId);
        }

        public void Update(BE.ServicioAdicional obj)
        {
            var sql = @"UPDATE " + table +
                @" SET
                    nombre      = @nombre,
                    descripcion = @descripcion,
                    precio   = @precio
                WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndRefresh(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdServicio;
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 100).Value = (object)obj.Descripcion ?? System.DBNull.Value;
                    cmd.Parameters.Add("@precio", SqlDbType.Decimal, 250).Value = (object)obj.Precio ?? System.DBNull.Value;
                },
                table, idCol
            );
        }
    }
}

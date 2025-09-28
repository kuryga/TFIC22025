using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
            return db.QueryListAndLog<BE.Material>(
                sql,
                null,
                table, idCol,
                BE.Audit.AuditEvents.ConsultaMateriales,
                "Listado de materiales"
            );
        }

        public void Create(BE.Material obj)
        {
            var sql = @"
INSERT INTO " + table + @" (nombre, unidadMedida, precioUnidad, usoPorM2)
VALUES (@nombre, @unidadMedida, @precioUnidad, @usoPorM2);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value =
                        (object)obj.Nombre ?? System.DBNull.Value;

                    cmd.Parameters.Add("@unidadMedida", SqlDbType.VarChar, 250).Value =
                        (object)obj.UnidadMedida ?? System.DBNull.Value;

                    var pPrecio = cmd.Parameters.Add("@precioUnidad", SqlDbType.Decimal);
                    pPrecio.Precision = 18; pPrecio.Scale = 2;     // precio con 2 decimales
                    pPrecio.Value = obj.PrecioUnidad;

                    var pUso = cmd.Parameters.Add("@usoPorM2", SqlDbType.Decimal);
                    pUso.Precision = 18; pUso.Scale = 4;          // uso por m2 con más precisión
                    pUso.Value = obj.UsoPorM2;
                },
                table, idCol,
                BE.Audit.AuditEvents.CreacionMaterial,
                "Alta de material: " + (obj.Nombre ?? string.Empty)
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdMaterial = System.Convert.ToInt32(newId);
        }

        public void Update(BE.Material obj)
        {
            var sql = @"
UPDATE " + table + @"
   SET nombre       = @nombre,
       unidadMedida = @unidadMedida,
       precioUnidad = @precioUnidad,
       usoPorM2     = @usoPorM2
 WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdMaterial;

                    cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100).Value =
                        (object)obj.Nombre ?? System.DBNull.Value;

                    cmd.Parameters.Add("@unidadMedida", SqlDbType.VarChar, 250).Value =
                        (object)obj.UnidadMedida ?? System.DBNull.Value;

                    var pPrecio = cmd.Parameters.Add("@precioUnidad", SqlDbType.Decimal);
                    pPrecio.Precision = 18; pPrecio.Scale = 2;
                    pPrecio.Value = obj.PrecioUnidad;

                    var pUso = cmd.Parameters.Add("@usoPorM2", SqlDbType.Decimal);
                    pUso.Precision = 18; pUso.Scale = 4;
                    pUso.Value = obj.UsoPorM2;
                },
                table, idCol,
                BE.Audit.AuditEvents.ModificacionMaterial,
                "Modificación de material Id=" + obj.IdMaterial
            );
        }
    }
}

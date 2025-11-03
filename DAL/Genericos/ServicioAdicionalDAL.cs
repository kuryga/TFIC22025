using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.ServicioAdicional>;

namespace DAL.Genericos
{
    public class ServicioAdicionalDAL : DaoInterface
    {
        private static ServicioAdicionalDAL instance;
        private ServicioAdicionalDAL() { }
        public static ServicioAdicionalDAL GetInstance()
        {
            if (instance == null) instance = new ServicioAdicionalDAL();
            return instance;
        }

        private static readonly DalToolkit db = new DalToolkit();

        private const string table = "dbo.ServicioAdicional";
        private const string idCol = "idServicio";
        private const string publicCols = "idServicio, descripcion, precio, deshabilitado";

        public List<BE.ServicioAdicional> GetAll()
        {
            var sql = "SELECT " + publicCols + " FROM " + table + ";";
            return db.QueryListAndLog<BE.ServicioAdicional>(
                sql,
                null,
                table, idCol,
                BE.Audit.AuditEvents.ConsultaServicios,
                "Listado de servicios adicionales",
                false
            );
        }

        public void Create(BE.ServicioAdicional obj)
        {
            var sql = @"
INSERT INTO " + table + @" (descripcion, precio)
VALUES (@descripcion, @precio);
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value =
                        (object)obj.Descripcion ?? System.DBNull.Value;

                    var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
                    pPrecio.Precision = 18;
                    pPrecio.Scale = 2;
                    pPrecio.Value = obj.Precio;
                },
                table, idCol,
                BE.Audit.AuditEvents.CreacionServicioAdicional,
                "Alta de servicio adicional",
                false
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdServicio = System.Convert.ToInt32(newId);

            if (obj.IdServicio > 0)
                db.RefreshRowDvAndTableDvv(table, idCol, obj.IdServicio, false);
        }

        public void Update(BE.ServicioAdicional obj)
        {
            var sql = @"
UPDATE " + table + @"
   SET descripcion = @descripcion,
       precio      = @precio
 WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdServicio;

                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, 250).Value =
                        (object)obj.Descripcion ?? System.DBNull.Value;

                    var pPrecio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
                    pPrecio.Precision = 18;
                    pPrecio.Scale = 2;
                    pPrecio.Value = obj.Precio;
                },
                table, idCol, obj.IdServicio,
                BE.Audit.AuditEvents.ModificacionServicioAdicional,
                "Modificación de servicio adicional Id=" + obj.IdServicio,
                true
            );
        }

        public void Deshabilitar(int idServicio, bool deshabilitar)
        {
            var sql = @"
UPDATE " + table + @"
   SET deshabilitado = @deshabilitado
 WHERE " + idCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idServicio;
                    cmd.Parameters.Add("@deshabilitado", SqlDbType.Bit).Value = deshabilitar;
                },
                table, idCol, idServicio,
                deshabilitar
                    ? BE.Audit.AuditEvents.DeshabilitacionServicioAdicional
                    : BE.Audit.AuditEvents.HabilitacionServicioAdicional,
                (deshabilitar ? "Deshabilitación" : "Habilitación") +
                " de servicio adicional Id=" + idServicio,
                true
            );

            db.RefreshRowDvAndTableDvv(table, idCol, idServicio, false);
        }
    }
}

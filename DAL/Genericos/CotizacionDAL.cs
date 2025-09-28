using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Cotizacion>;

namespace DAL.Genericos
{
    public class CotizacionDAL : DaoInterface
    {
        private static CotizacionDAL instance;
        private CotizacionDAL() { }
        public static CotizacionDAL GetInstance() { if (instance == null) instance = new CotizacionDAL(); return instance; }

        private static readonly DalToolkit db = new DalToolkit();

        private const string connectionString =
            @"Server=.;Database=UrbanSoft;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        private const string TblCotizacion = "dbo.Cotizacion";
        private const string PkCotizacion = "idCotizacion";
        private const string TblMaterialCotizacion = "dbo.MaterialCotizacion";
        private const string PkMaterialCotizacion = "idMaterialCotizacion";
        private const string TblMaquinariaCotizacion = "dbo.MaquinariaCotizacion";
        private const string PkMaquinariaCotizacion = "idMaquinariaCotizacion";
        private const string TblServicioCotizacion = "dbo.ServicioCotizacion";
        private const string PkServicioCotizacion = "idServicioCotizacion";

        private const string PublicCols =
            "c.idCotizacion, c.fechaCreacion, c.idTipoEdificacion, c.idMoneda";

        public List<BE.Cotizacion> GetAll() { return GetListaCotizaciones(); }

        // DTO plano para mapear el SELECT con DbMapper
        private class CotizacionListRow
        {
            public int idCotizacion { get; set; }
            public DateTime fechaCreacion { get; set; }
            public int idTipoEdificacion { get; set; }
            public int idMoneda { get; set; }
            public string tipoDescripcion { get; set; }
            public string nombreMoneda { get; set; }
            public decimal valorCambio { get; set; }
            public string Simbolo { get; set; }
        }

        public List<BE.Cotizacion> GetListaCotizaciones()
        {
            var sql = @"
SELECT " + PublicCols + @",
       te.descripcion AS tipoDescripcion,
       m.nombreMoneda,
       m.valorCambio,
       m.Simbolo
FROM dbo.Cotizacion c
JOIN dbo.TipoEdificacion te ON te.idTipoEdificacion = c.idTipoEdificacion
JOIN dbo.Moneda          m  ON m.idMoneda          = c.idMoneda
ORDER BY c.idCotizacion DESC;";

            var rows = db.QueryListAndLog<CotizacionListRow>(
                sql,
                null,
                TblCotizacion, PkCotizacion,
                BE.Audit.AuditEvents.ConsultaCotizaciones,
                "Listado de cotizaciones"
            );

            var list = new List<BE.Cotizacion>(rows.Count);
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var ctz = new BE.Cotizacion
                {
                    IdCotizacion = r.idCotizacion,
                    FechaCreacion = r.fechaCreacion,
                    TipoEdificacion = new BE.TipoEdificacion
                    {
                        IdTipoEdificacion = r.idTipoEdificacion,
                        Descripcion = r.tipoDescripcion
                    },
                    Moneda = new BE.Moneda
                    {
                        IdMoneda = r.idMoneda,
                        NombreMoneda = r.nombreMoneda,
                        ValorCambio = r.valorCambio,
                        Simbolo = r.Simbolo
                    },
                    ListaMateriales = null,
                    ListaMaquinaria = null,
                    ListaServicios = null
                };
                list.Add(ctz);
            }
            return list;
        }

        public BE.Cotizacion GetCotizacionCompleta(int idCotizacion)
        {
            var sql = @"
-- Header
SELECT c.idCotizacion, c.fechaCreacion, c.idTipoEdificacion,
       te.descripcion AS tipoDescripcion,
       c.idMoneda, m.nombreMoneda, m.valorCambio, m.Simbolo
FROM dbo.Cotizacion c
JOIN dbo.TipoEdificacion te ON te.idTipoEdificacion = c.idTipoEdificacion
JOIN dbo.Moneda          m  ON m.idMoneda          = c.idMoneda
WHERE c.idCotizacion = @id;

-- Materiales
SELECT mc.idMaterialCotizacion, mc.idCotizacion, mc.idMaterial, mc.cantidad,
       mat.nombre AS materialNombre, mat.unidadMedida, mat.precioUnidad, mat.usoPorM2
FROM dbo.MaterialCotizacion mc
JOIN dbo.Material mat ON mat.idMaterial = mc.idMaterial
WHERE mc.idCotizacion = @id;

-- Maquinaria
SELECT mq.idMaquinariaCotizacion, mq.idCotizacion, mq.idMaquinaria, mq.horasUso,
       maq.nombre AS maquinariaNombre, maq.costoPorHora
FROM dbo.MaquinariaCotizacion mq
JOIN dbo.Maquinaria maq ON maq.idMaquinaria = mq.idMaquinaria
WHERE mq.idCotizacion = @id;

-- Servicios
SELECT sc.idServicioCotizacion, sc.idCotizacion, sc.idServicio,
       srv.descripcion AS servicioDescripcion, srv.precio AS servicioPrecio
FROM dbo.ServicioCotizacion sc
JOIN dbo.ServicioAdicional srv ON srv.idServicio = sc.idServicio
WHERE sc.idCotizacion = @id;
";

            BE.Cotizacion ctz = null;

            var conn = new SqlConnection(connectionString);
            var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idCotizacion;

            try
            {
                conn.Open();
                var rdr = cmd.ExecuteReader();

                // Header
                if (rdr.Read())
                {
                    ctz = new BE.Cotizacion();
                    ctz.IdCotizacion = rdr.GetInt32(rdr.GetOrdinal("idCotizacion"));
                    ctz.FechaCreacion = rdr.GetDateTime(rdr.GetOrdinal("fechaCreacion"));

                    var te = new BE.TipoEdificacion();
                    te.IdTipoEdificacion = rdr.GetInt32(rdr.GetOrdinal("idTipoEdificacion"));
                    te.Descripcion = rdr["tipoDescripcion"] == DBNull.Value ? null : rdr["tipoDescripcion"].ToString();
                    ctz.TipoEdificacion = te;

                    var mon = new BE.Moneda();
                    mon.IdMoneda = rdr.GetInt32(rdr.GetOrdinal("idMoneda"));
                    mon.NombreMoneda = rdr["nombreMoneda"] == DBNull.Value ? null : rdr["nombreMoneda"].ToString();
                    mon.ValorCambio = rdr["valorCambio"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["valorCambio"]);
                    mon.Simbolo = rdr["Simbolo"] == DBNull.Value ? null : rdr["Simbolo"].ToString();
                    ctz.Moneda = mon;
                }

                if (ctz == null)
                {
                    conn.Close();
                    db.ExecuteScalarAndLog("SELECT 1", null, TblCotizacion, PkCotizacion,
                        BE.Audit.AuditEvents.ConsultaCotizacionDetalle, "Detalle inexistente Id=" + idCotizacion);
                    return null;
                }

                // Materiales
                ctz.ListaMateriales = new List<BE.MaterialCotizacion>();
                rdr.NextResult();
                while (rdr.Read())
                {
                    var mc = new BE.MaterialCotizacion();
                    mc.IdMaterialCotizacion = rdr.GetInt32(rdr.GetOrdinal("idMaterialCotizacion"));
                    mc.IdCotizacion = rdr.GetInt32(rdr.GetOrdinal("idCotizacion"));
                    mc.Cantidad = rdr["cantidad"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["cantidad"]);

                    var mat = new BE.Material();
                    mat.IdMaterial = rdr.GetInt32(rdr.GetOrdinal("idMaterial"));
                    mat.Nombre = rdr["materialNombre"] == DBNull.Value ? null : rdr["materialNombre"].ToString();
                    mat.UnidadMedida = rdr["unidadMedida"] == DBNull.Value ? null : rdr["unidadMedida"].ToString();
                    mat.PrecioUnidad = rdr["precioUnidad"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["precioUnidad"]);
                    mat.UsoPorM2 = rdr["usoPorM2"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["usoPorM2"]);
                    mc.Material = mat;

                    ctz.ListaMateriales.Add(mc);
                }

                // Maquinaria
                ctz.ListaMaquinaria = new List<BE.MaquinariaCotizacion>();
                rdr.NextResult();
                while (rdr.Read())
                {
                    var mq = new BE.MaquinariaCotizacion();
                    mq.IdMaquinariaCotizacion = rdr.GetInt32(rdr.GetOrdinal("idMaquinariaCotizacion"));
                    mq.IdCotizacion = rdr.GetInt32(rdr.GetOrdinal("idCotizacion"));
                    mq.HorasUso = rdr["horasUso"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["horasUso"]);

                    var maq = new BE.Maquinaria();
                    maq.IdMaquinaria = rdr.GetInt32(rdr.GetOrdinal("idMaquinaria"));
                    maq.Nombre = rdr["maquinariaNombre"] == DBNull.Value ? null : rdr["maquinariaNombre"].ToString();
                    maq.CostoPorHora = rdr["costoPorHora"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["costoPorHora"]);
                    mq.Maquinaria = maq;

                    ctz.ListaMaquinaria.Add(mq);
                }

                // Servicios
                ctz.ListaServicios = new List<BE.ServicioCotizacion>();
                rdr.NextResult();
                while (rdr.Read())
                {
                    var sc = new BE.ServicioCotizacion();
                    sc.IdServicioCotizacion = rdr.GetInt32(rdr.GetOrdinal("idServicioCotizacion"));
                    sc.IdCotizacion = rdr.GetInt32(rdr.GetOrdinal("idCotizacion"));

                    var srv = new BE.ServicioAdicional();
                    srv.IdServicio = rdr.GetInt32(rdr.GetOrdinal("idServicio"));
                    srv.Descripcion = rdr["servicioDescripcion"] == DBNull.Value ? null : rdr["servicioDescripcion"].ToString();
                    srv.Precio = rdr["servicioPrecio"] == DBNull.Value ? 0m : Convert.ToDecimal(rdr["servicioPrecio"]);
                    sc.Servicio = srv;

                    ctz.ListaServicios.Add(sc);
                }

                conn.Close();

                // Registrar lectura y refrescar DV de cabecera (liviano)
                db.ExecuteScalarAndLog("SELECT 1", null, TblCotizacion, PkCotizacion,
                    BE.Audit.AuditEvents.ConsultaCotizacionDetalle, "Detalle Id=" + idCotizacion);
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open) conn.Close();
                // Log explícito de la falla usando el toolkit (C1)
                db.ExecuteScalarAndLog(
                    "SELECT 1", null,
                    TblCotizacion, PkCotizacion,
                    BE.Audit.AuditEvents.FalloConexionBD,
                    "GetCotizacionCompleta sobre cotizacion: " + ex.Message
                );
                throw;
            }

            return ctz;
        }

        public void Create(BE.Cotizacion obj)
        {
            var sql = @"
INSERT INTO " + TblCotizacion + @"
( fechaCreacion, idTipoEdificacion, idMoneda )
VALUES
( @fecha, @idTipoEdificacion, @idMoneda );
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = obj.FechaCreacion;
                    cmd.Parameters.Add("@idTipoEdificacion", SqlDbType.Int).Value =
                        (obj.TipoEdificacion != null) ? obj.TipoEdificacion.IdTipoEdificacion : 0;
                    cmd.Parameters.Add("@idMoneda", SqlDbType.Int).Value =
                        (obj.Moneda != null) ? obj.Moneda.IdMoneda : 0;
                },
                TblCotizacion, PkCotizacion,
                BE.Audit.AuditEvents.CreacionCotizacion,
                "Alta de cotización"
            );

            if (newId != null && newId != DBNull.Value)
                obj.IdCotizacion = Convert.ToInt32(newId);
        }

        public void Update(BE.Cotizacion obj)
        {
            var sql = @"
UPDATE " + TblCotizacion + @" SET
    fechaCreacion     = @fecha,
    idTipoEdificacion = @idTipoEdificacion,
    idMoneda          = @idMoneda
WHERE " + PkCotizacion + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = obj.IdCotizacion;
                    cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = obj.FechaCreacion;
                    cmd.Parameters.Add("@idTipoEdificacion", SqlDbType.Int).Value =
                        (obj.TipoEdificacion != null) ? obj.TipoEdificacion.IdTipoEdificacion : 0;
                    cmd.Parameters.Add("@idMoneda", SqlDbType.Int).Value =
                        (obj.Moneda != null) ? obj.Moneda.IdMoneda : 0;
                },
                TblCotizacion, PkCotizacion,
                BE.Audit.AuditEvents.ModificacionCotizacionHeader,
                "Actualización de cotización Id=" + obj.IdCotizacion
            );
        }
    }
}

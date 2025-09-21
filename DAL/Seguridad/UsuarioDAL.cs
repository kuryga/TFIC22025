using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Utilities;

using UserDaoInterface = DAL.Seguridad.DV.IDVDAOInterface<BE.Usuario>;
using DVService = DAL.Seguridad.DV.DVService;

namespace DAL.Seguridad
{
    public class UsuarioDAL : UserDaoInterface
    {
        private static UsuarioDAL instance;
        private UsuarioDAL() { }

        public static UsuarioDAL GetInstance()
        {
            if (instance == null) instance = new UsuarioDAL();
            return instance;
        }
        string connectionString = @"Server=.;Database=UrbanSoft;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";

        private const string USER_TABLE = "dbo.Usuario";

        private const string DVV_LEGACY_TABLE = "[UrbanSoft].[dbo].[dvv]";
        private const string DVV_LEGACY_COL_DVV = "dvv";
        private const string DVV_LEGACY_COL_TABLENAME = "tablename";
        private const string DVV_LEGACY_TABLENAME_VALUE = "Usuario";

        private static BE.Usuario Map(IDataRecord r)
        {
            return new BE.Usuario
            {
                IdUsuario = r.GetInt32(r.GetOrdinal("idUsuario")),
                NombreUsuario = r["nombreUsuario"] as string ?? string.Empty,
                ApellidoUsuario = r["apellidoUsuario"] as string ?? string.Empty,
                CorreoElectronico = r["correoElectronico"] as string ?? string.Empty,
                TelefonoContacto = r["telefonoContacto"] as string,
                DireccionUsuario = r["direccionUsuario"] as string,
                NumeroDocumento = r["numeroDocumento"] as string,
               // ContrasenaHash = r["contrasenaHash"] as string ?? string.Empty,
            //    TokenRecuperoSesionHash = r["tokenRecuperoSesionHash"] as string,
          //      TiempoExpiracionToken = r["tiempoExpiracionToken"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["tiempoExpiracionToken"]),
          //      ContadorIntentosFallidos = r["contadorIntentosFallidos"] == DBNull.Value ? 0 : Convert.ToInt32(r["contadorIntentosFallidos"]),
                Bloqueado = HasColumn(r, "Bloqueado") && r["Bloqueado"] != DBNull.Value && Convert.ToBoolean(r["Bloqueado"])
            };
        }

        private static bool HasColumn(IDataRecord r, string name)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (r.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static void BindParams(SqlCommand cmd, BE.Usuario u, bool includeId)
        {
            if (includeId) cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = u.IdUsuario;
            cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)u.NombreUsuario ?? DBNull.Value;
            cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)u.ApellidoUsuario ?? DBNull.Value;
            cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)u.CorreoElectronico ?? DBNull.Value;
            cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)u.TelefonoContacto ?? DBNull.Value;
            cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)u.DireccionUsuario ?? DBNull.Value;
            cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)u.NumeroDocumento ?? DBNull.Value;
          //  cmd.Parameters.Add("@contrasenaHash", SqlDbType.Char, 32).Value = (object)u.ContrasenaHash ?? DBNull.Value;
        //    cmd.Parameters.Add("@tokenRecuperoSesionHash", SqlDbType.Char, 64).Value = (object)u.TokenRecuperoSesionHash ?? DBNull.Value;
         //   cmd.Parameters.Add("@tiempoExpiracionToken", SqlDbType.DateTime).Value = (object)u.TiempoExpiracionToken ?? DBNull.Value;
          //  cmd.Parameters.Add("@contadorIntentosFallidos", SqlDbType.Int).Value = u.ContadorIntentosFallidos;
            cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = u.Bloqueado;
        }

        public List<BE.Usuario> GetAll()
        {
           // var list = new List<BE.Usuario>();
            var sql = @"SELECT * FROM dbo.Usuario;";

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;

            comm.Connection = conn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = sql;

            try
            {
                conn.Open();

                reader = comm.ExecuteReader();
                var users = DbMapper.MapToList<BE.Usuario>(reader);

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string CalculateHorizontal(BE.Usuario u)
        {
            var sb = new StringBuilder();
            sb.Append(u.NombreUsuario ?? string.Empty)
              .Append(u.ApellidoUsuario ?? string.Empty)
              .Append(u.CorreoElectronico ?? string.Empty)
              .Append(u.TelefonoContacto ?? string.Empty)
              .Append(u.DireccionUsuario ?? string.Empty)
              .Append(u.NumeroDocumento ?? string.Empty)
              //.Append(u.ContrasenaHash ?? string.Empty)
            //  .Append(u.TokenRecuperoSesionHash ?? string.Empty)
            //  .Append(u.TiempoExpiracionToken?.ToString("o") ?? string.Empty)
            //  .Append(u.ContadorIntentosFallidos.ToString())
               .Append(u.Bloqueado.ToString());
            return DVService.getDV(sb.ToString());
        }

        public string CalculateVertical(List<BE.Usuario> list)
        {
            string acc = string.Empty;
            if (list != null)
            {
                foreach (var u in list)
                {
                    var dvh = CalculateHorizontal(u);
                    acc = DVService.getDV(acc + dvh);
                }
            }
            return acc;
        }

        public void Save(BE.Usuario obj)
        {
            string dvh = CalculateHorizontal(obj);
            var sql = @"
INSERT INTO dbo.Usuario
( nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
  numeroDocumento, contrasenaHash, tokenRecuperoSesionHash, tiempoExpiracionToken,
  contadorIntentosFallidos, Bloqueado, DVH )
VALUES
( @nombreUsuario, @apellidoUsuario, @correoElectronico, @telefonoContacto, @direccionUsuario,
  @numeroDocumento, @contrasenaHash, @tokenRecuperoSesionHash, @tiempoExpiracionToken,
  @contadorIntentosFallidos, @Bloqueado, @DVH );
SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (var con = ConnectionSingleton.getConnection())
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    BindParams(cmd, obj, false);
                    cmd.Parameters.Add("@DVH", SqlDbType.VarChar, 100).Value = dvh;

                    if (con.State != ConnectionState.Open) con.Open();
                    var newIdObj = cmd.ExecuteScalar();
                    con.Close();

                    if (newIdObj != null && newIdObj != DBNull.Value)
                        obj.IdUsuario = Convert.ToInt32(newIdObj);
                }
            }

            UpdateVertical();
        }

        public void Update(BE.Usuario obj)
        {
            string dvh = CalculateHorizontal(obj);

            var sql = @"
UPDATE dbo.Usuario SET
    nombreUsuario = @nombreUsuario,
    apellidoUsuario = @apellidoUsuario,
    correoElectronico = @correoElectronico,
    telefonoContacto = @telefonoContacto,
    direccionUsuario = @direccionUsuario,
    numeroDocumento = @numeroDocumento,
    contrasenaHash = @contrasenaHash,
    tokenRecuperoSesionHash = @tokenRecuperoSesionHash,
    tiempoExpiracionToken = @tiempoExpiracionToken,
    contadorIntentosFallidos = @contadorIntentosFallidos,
    Bloqueado = @Bloqueado,
    DVH = @DVH
WHERE idUsuario = @idUsuario;";

            using (var con = ConnectionSingleton.getConnection())
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    BindParams(cmd, obj, true);
                    cmd.Parameters.Add("@DVH", SqlDbType.VarChar, 100).Value = dvh;

                    if (con.State != ConnectionState.Open) con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            UpdateVertical();
        }

        public void UpdateVertical()
        {
            var dvvString = CalculateVertical(GetAll());

            using (var connection = ConnectionSingleton.getConnection())
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
UPDATE [UrbanSoft].[dbo].[dvv]
   SET [dvv] = @dvv
 WHERE [tablename] = @tablename;";

                    var dvv = command.CreateParameter();
                    dvv.ParameterName = "@dvv";
                    dvv.Value = dvvString;
                    command.Parameters.Add(dvv);

                    var tablename = command.CreateParameter();
                    tablename.ParameterName = "@tablename";
                    tablename.Value = DVV_LEGACY_TABLENAME_VALUE;
                    command.Parameters.Add(tablename);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void UpdateAllDV()
        {
            var users = GetAll();
            using (var con = ConnectionSingleton.getConnection())
            {
                if (con.State != ConnectionState.Open) con.Open();
                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        foreach (var u in users)
                        {
                            var dvh = CalculateHorizontal(u);
                            var sqlUpd = "UPDATE dbo.Usuario SET DVH = @DVH WHERE idUsuario = @id;";
                            using (var cmd = new SqlCommand(sqlUpd, con, tx))
                            {
                                cmd.Parameters.Add("@DVH", SqlDbType.VarChar, 100).Value = dvh;
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = u.IdUsuario;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }

            UpdateVertical();
        }

        public string GetVertical()
        {
            var sql = @"SELECT dvv FROM [UrbanSoft].[dbo].[dvv] WHERE tablename = @tablename;";
            using (var con = ConnectionSingleton.getConnection())
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@tablename", SqlDbType.VarChar, 128).Value = DVV_LEGACY_TABLENAME_VALUE;

                    if (con.State != ConnectionState.Open) con.Open();
                    var obj = cmd.ExecuteScalar();
                    con.Close();

                    return obj != null ? obj.ToString() : string.Empty;
                }
            }
        }

        public BE.Usuario GetByCorreoElectronico(string correoElectronico)
        {
            const string sql = @"
SELECT TOP 1
    idUsuario, nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto,
    direccionUsuario, numeroDocumento, contrasenaHash, tokenRecuperoSesionHash,
    tiempoExpiracionToken, contadorIntentosFallidos, Bloqueado
FROM dbo.Usuario
WHERE correoElectronico = @mail;";

            using (var con = ConnectionSingleton.getConnection())
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value =
                        (correoElectronico ?? string.Empty).Trim();

                    if (con.State != ConnectionState.Open) con.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        BE.Usuario u = null;
                        if (rdr.Read())
                            u = Map(rdr);
                        con.Close();
                        return u;
                    }
                }
            }
        }
    }
}

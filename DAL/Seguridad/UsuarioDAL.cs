using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using Utilities;

namespace DAL.Seguridad
{
    public class Usuario : BE.ICrud<BE.Usuario>
    {
        private static Usuario instance;
        private Usuario() { }
        public static Usuario GetInstance() => instance ??= new Usuario();

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
                ContrasenaHash = r["contrasenaHash"] as string ?? string.Empty,
                TokenRecuperoSesionHash = r["tokenRecuperoSesionHash"] as string,
                TiempoExpiracionToken = r["tiempoExpiracionToken"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["tiempoExpiracionToken"]),
                ContadorIntentosFallidos = r["contadorIntentosFallidos"] == DBNull.Value ? 0 : Convert.ToInt32(r["contadorIntentosFallidos"]),
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

        private static void BindParams(SqlCommand cmd, BE.Usuario u, bool includeId = true)
        {
            if (includeId) cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = u.IdUsuario;

            cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object?)u.NombreUsuario ?? DBNull.Value;
            cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object?)u.ApellidoUsuario ?? DBNull.Value;
            cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object?)u.CorreoElectronico ?? DBNull.Value;
            cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object?)u.TelefonoContacto ?? DBNull.Value;
            cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object?)u.DireccionUsuario ?? DBNull.Value;
            cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object?)u.NumeroDocumento ?? DBNull.Value;
            cmd.Parameters.Add("@contrasenaHash", SqlDbType.Char, 32).Value = (object?)u.ContrasenaHash ?? DBNull.Value;
            cmd.Parameters.Add("@tokenRecuperoSesionHash", SqlDbType.Char, 64).Value = (object?)u.TokenRecuperoSesionHash ?? DBNull.Value;
            cmd.Parameters.Add("@tiempoExpiracionToken", SqlDbType.DateTime).Value = (object?)u.TiempoExpiracionToken ?? DBNull.Value;
            cmd.Parameters.Add("@contadorIntentosFallidos", SqlDbType.Int).Value = u.ContadorIntentosFallidos;
            cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = u.Bloqueado;
        }

        public bool Create(BE.Usuario objAdd)
        {
            const string sql = @"
INSERT INTO dbo.Usuario
( idUsuario, nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
  numeroDocumento, contrasenaHash, tokenRecuperoSesionHash, tiempoExpiracionToken,
  contadorIntentosFallidos, Bloqueado )
VALUES
( @idUsuario, @nombreUsuario, @apellidoUsuario, @correoElectronico, @telefonoContacto, @direccionUsuario,
  @numeroDocumento, @contrasenaHash, @tokenRecuperoSesionHash, @tiempoExpiracionToken,
  @contadorIntentosFallidos, @Bloqueado );";

            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            BindParams(cmd, objAdd, includeId: true);

            if (con.State != ConnectionState.Open) con.Open();
            var rows = cmd.ExecuteNonQuery();
            con.Close();
            return rows == 1;
        }

        public List<BE.Usuario> GetAll()
        {
            const string sql = @"
SELECT idUsuario, nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
       numeroDocumento, contrasenaHash, tokenRecuperoSesionHash, tiempoExpiracionToken,
       contadorIntentosFallidos,
       /* si existe la columna: */ Bloqueado
FROM dbo.Usuario;";

            var list = new List<BE.Usuario>();
            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            if (con.State != ConnectionState.Open) con.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(Map(rdr));
            con.Close();
            return list;
        }

        public bool Update(BE.Usuario objUpd)
        {
            const string sql = @"
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
    Bloqueado = @Bloqueado
WHERE idUsuario = @idUsuario;";

            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            BindParams(cmd, objUpd, includeId: true);

            if (con.State != ConnectionState.Open) con.Open();
            var rows = cmd.ExecuteNonQuery();
            con.Close();
            return rows == 1;
        }

        public bool Delete(BE.Usuario objUdp)
        {
            const string sql = @"DELETE FROM dbo.Usuario WHERE idUsuario = @idUsuario;";
            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = objUdp.IdUsuario;

            if (con.State != ConnectionState.Open) con.Open();
            var rows = cmd.ExecuteNonQuery();
            con.Close();
            return rows == 1;
        }

        public BE.Usuario GetByUsername(string username)
        {
            const string sql = @"
SELECT TOP 1 idUsuario, nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
             numeroDocumento, contrasenaHash, tokenRecuperoSesionHash, tiempoExpiracionToken,
             contadorIntentosFallidos,
             /* si existe la columna: */ Bloqueado
FROM dbo.Usuario
WHERE correoElectronico = @u OR nombreUsuario = @u;";

            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@u", SqlDbType.VarChar, 150).Value = username;

            if (con.State != ConnectionState.Open) con.Open();
            using var rdr = cmd.ExecuteReader();
            BE.Usuario u = null;
            if (rdr.Read()) u = Map(rdr);
            con.Close();
            return u;
        }

        public BE.Usuario GetById(int idUsuario)
        {
            const string sql = @"
SELECT TOP 1 idUsuario, nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
             numeroDocumento, contrasenaHash, tokenRecuperoSesionHash, tiempoExpiracionToken,
             contadorIntentosFallidos,
             /* si existe la columna: */ Bloqueado
FROM dbo.Usuario
WHERE idUsuario = @id;";

            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;

            if (con.State != ConnectionState.Open) con.Open();
            using var rdr = cmd.ExecuteReader();
            BE.Usuario u = null;
            if (rdr.Read()) u = Map(rdr);
            con.Close();
            return u;
        }

        public void UpdatePassword(int idUsuario, string newHash)
        {
            const string sql = @"UPDATE dbo.Usuario SET contrasenaHash = @pwd WHERE idUsuario = @id;";
            using var con = ConnectionSingleton.getConnection();
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
            cmd.Parameters.Add("@pwd", SqlDbType.Char, 32).Value = newHash ?? (object)DBNull.Value;

            if (con.State != ConnectionState.Open) con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}

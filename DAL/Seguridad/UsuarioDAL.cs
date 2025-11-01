using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.IO;

using UserDaoInterface = DAL.Seguridad.DV.IDAOInterface<BE.Usuario>;
using segUtils = DAL.Seguridad.SecurityUtilities;

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

        private static readonly DalToolkit db = new DalToolkit();

        private const string userTable = "dbo.Usuario";
        private const string userIdCol = "idUsuario";
        private const string userPublicCols =
            "idUsuario, nombreUsuario, apellidoUsuario, correoElectronico, " +
            "telefonoContacto, direccionUsuario, numeroDocumento, Bloqueado, Deshabilitado";

        private const int AdminUserId = 999;

        public List<BE.Usuario> GetAll()
        {
            var sql = "SELECT " + userPublicCols + " FROM " + userTable +
                      " WHERE " + userIdCol + " <> " + AdminUserId + ";";

            var list = db.QueryListAndLog<BE.Usuario>(
                sql,
                null,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarios,
                "Listado de usuarios"
            );

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        list[i].CorreoElectronico =
                            segUtils.DesencriptarReversible(list[i].CorreoElectronico ?? string.Empty);
                    }
                    catch
                    {
                        // nada
                    }
                }
            }

            return list;
        }

        public void Create(BE.Usuario obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            string mailEnc = segUtils.EncriptarReversible((obj.CorreoElectronico ?? string.Empty).Trim());
            string documento = (obj.NumeroDocumento ?? string.Empty).Trim();

            string sqlCheckMail = @"
SELECT COUNT(1)
FROM " + userTable + @"
WHERE correoElectronico = @mail;";

            object oCountMail = db.ExecuteScalarAndLog(
                sqlCheckMail,
                c => c.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value = mailEnc,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarioPorCorreo,
                "Verificación de email duplicado: " + (obj.CorreoElectronico ?? string.Empty),
                shouldCalculate: false
            );
            int countMail = Convert.ToInt32(oCountMail ?? 0);
            if (countMail > 0)
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("user_email_exists"));

            string sqlCheckDoc = @"
SELECT COUNT(1)
FROM " + userTable + @"
WHERE numeroDocumento = @doc;";

            object oCountDoc = db.ExecuteScalarAndLog(
                sqlCheckDoc,
                c => c.Parameters.Add("@doc", SqlDbType.VarChar, 20).Value = documento,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarios,
                "Verificación de documento duplicado: " + documento,
                shouldCalculate: false
            );
            int countDoc = Convert.ToInt32(oCountDoc ?? 0);
            if (countDoc > 0)
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("user_document_exists"));


            string randomPassword = GenerarContrasenaAleatoria(20);
            string passwordHash = segUtils.EncriptarIrreversible(randomPassword);

            var sql = @"
INSERT INTO " + userTable + @"
( nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
  numeroDocumento, contrasenaHash, Bloqueado, Deshabilitado )
VALUES
( @nombreUsuario, @apellidoUsuario, @correoElectronico, @telefonoContacto, @direccionUsuario,
  @numeroDocumento, @contrasenaHash, @Bloqueado, @Deshabilitado );
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)obj.NombreUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)obj.ApellidoUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)mailEnc ?? System.DBNull.Value;
                    cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)obj.TelefonoContacto ?? System.DBNull.Value;
                    cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)obj.DireccionUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)documento ?? System.DBNull.Value;
                    cmd.Parameters.Add("@contrasenaHash", SqlDbType.Char, 32).Value = (object)passwordHash ?? System.DBNull.Value;
                    cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = 1;       // se crea bloqueado
                    cmd.Parameters.Add("@Deshabilitado", SqlDbType.Bit).Value = 0;   // por defecto habilitado
                },
                userTable, userIdCol,
                BE.Audit.AuditEvents.CreacionUsuario,
                "Alta de usuario: " + (obj.CorreoElectronico ?? string.Empty),
                shouldCalculate: false
            );

            if (newId != null && newId != System.DBNull.Value)
                obj.IdUsuario = System.Convert.ToInt32(newId);

            if (obj.IdUsuario > 0)
                db.RefreshRowDvAndTableDvv(userTable, userIdCol, obj.IdUsuario, false);
        }

        public void Update(BE.Usuario obj)
        {
            var sql = @"
UPDATE " + userTable + @" SET
    nombreUsuario     = @nombreUsuario,
    apellidoUsuario   = @apellidoUsuario,
    correoElectronico = @correoElectronico,
    telefonoContacto  = @telefonoContacto,
    direccionUsuario  = @direccionUsuario,
    numeroDocumento   = @numeroDocumento,
    Bloqueado         = @Bloqueado,
    Deshabilitado     = @Deshabilitado
WHERE " + userIdCol + @" = @idUsuario;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = obj.IdUsuario;
                    cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)obj.NombreUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)obj.ApellidoUsuario ?? System.DBNull.Value;

                    string encMail = segUtils.EncriptarReversible(obj.CorreoElectronico ?? string.Empty);
                    cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)encMail ?? System.DBNull.Value;

                    cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)obj.TelefonoContacto ?? System.DBNull.Value;
                    cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)obj.DireccionUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)obj.NumeroDocumento ?? System.DBNull.Value;
                    cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = obj.Bloqueado;
                    cmd.Parameters.Add("@Deshabilitado", SqlDbType.Bit).Value = obj.Deshabilitado;
                },
                userTable, userIdCol, obj.IdUsuario,
                BE.Audit.AuditEvents.ModificacionUsuario,
                "Modificación de usuario Id=" + obj.IdUsuario,
                true
            );
        }

        public class UsuarioLoginRow
        {
            public int idUsuario { get; set; }
            public string correoElectronico { get; set; }
            public string nombreUsuario { get; set; }
            public string apellidoUsuario { get; set; }
            public string contrasenaHash { get; set; }
            public int? contadorIntentosFallidos { get; set; }
            public bool Bloqueado { get; set; }
            public bool Deshabilitado { get; set; }
        }

        public UsuarioLoginRow GetLoginRowByCorreo(string correoElectronico)
        {
            string mailEnc = segUtils.EncriptarReversible((correoElectronico ?? string.Empty).Trim());

            string sql = @"
SELECT TOP 1
    idUsuario,
    correoElectronico,
    nombreUsuario,
    apellidoUsuario,
    contrasenaHash,
    contadorIntentosFallidos,
    Bloqueado,
    Deshabilitado
FROM dbo.Usuario
WHERE correoElectronico = @mail;";

            var row = db.QuerySingleOrDefaultAndLog<UsuarioLoginRow>(
                sql,
                c => c.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value = (mailEnc ?? string.Empty),
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarioPorCorreo,
                "Búsqueda por correo (login): " + (correoElectronico ?? string.Empty)
            );

            if (row != null)
            {
                try
                {
                    row.correoElectronico =
                        segUtils.DesencriptarReversible(row.correoElectronico ?? string.Empty);
                }
                catch
                {
                    // nada
                }
            }

            return row;
        }

        public void enviarRecuperoContrasena(string userMail)
        {
            if (string.IsNullOrWhiteSpace(userMail))
                throw new ArgumentNullException(nameof(userMail));

            string mailEnc = segUtils.EncriptarReversible(userMail.Trim());

            const string sqlGetId = @"
SELECT TOP 1 " + userIdCol + @"
FROM " + userTable + @"
WHERE correoElectronico = @mail;";

            object oId = db.ExecuteScalarAndLog(
                sqlGetId,
                c => c.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value = mailEnc,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarioPorCorreo,
                "Búsqueda de usuario para recupero: " + userMail,
                shouldCalculate: false
            );

            int idUsuario = Convert.ToInt32(oId ?? 0);
            if (idUsuario <= 0)
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("recover_email_sent_generic_message"));

            string codigoPlano = GenerarCodigoRecupero(6);

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string safeMail = SanitizarParaNombreArchivo(userMail);
            string filePath = Path.Combine(desktop, $"Recupero_{safeMail}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            File.WriteAllText(filePath, codigoPlano, System.Text.Encoding.UTF8);

            string tokenHash = segUtils.EncriptarIrreversible(codigoPlano);
            DateTime expiracion = DateTime.Now.AddMinutes(5);

            string sqlUpdate = @"
UPDATE " + userTable + @"
   SET tokenRecuperoSesionHash = @token,
       tiempoExpiracionToken   = @exp
 WHERE " + userIdCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sqlUpdate,
                cmd =>
                {
                    cmd.Parameters.Add("@token", SqlDbType.VarChar, -1).Value = (object)tokenHash ?? DBNull.Value;
                    cmd.Parameters.Add("@exp", SqlDbType.DateTime).Value = expiracion;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                },
                userTable, userIdCol, idUsuario,
                BE.Audit.AuditEvents.RecuperoContrasenaSolicitud,
                "Generación de token de recupero. IdUsuario=" + idUsuario,
                true
            );
        }
        private sealed class UsuarioTokenRow
        {
            public int idUsuario { get; set; }
            public string tokenRecuperoSesionHash { get; set; }
            public DateTime? tiempoExpiracionToken { get; set; }
        }

        public int VerificarCodigoRecuperacion(string userMail, string codigoPlano)
        {
            if (string.IsNullOrWhiteSpace(userMail))
                throw new ArgumentNullException(nameof(userMail));
            if (string.IsNullOrWhiteSpace(codigoPlano))
                throw new ArgumentNullException(nameof(codigoPlano));

            string mailEnc = segUtils.EncriptarReversible(userMail.Trim());

            const string sqlGet = @"
SELECT TOP 1
    idUsuario,
    tokenRecuperoSesionHash,
    tiempoExpiracionToken
FROM dbo.Usuario
WHERE correoElectronico = @mail;";

            var row = db.QuerySingleOrDefaultAndLog<UsuarioTokenRow>(
                sqlGet,
                c => c.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value = mailEnc,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarioPorCorreo,
                "Verificación de código de recupero: " + userMail
            );

            if (row == null || string.IsNullOrWhiteSpace(row.tokenRecuperoSesionHash))
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("security_code_incorrect_message"));

            DateTime ahora = DateTime.Now;

            if (!row.tiempoExpiracionToken.HasValue || ahora > row.tiempoExpiracionToken.Value)
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("security_code_expired_message"));

            bool valido = segUtils.VerificarIrreversible(
                (codigoPlano ?? string.Empty).Trim().ToUpperInvariant(),
                row.tokenRecuperoSesionHash
            );

            if (!valido)
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("security_code_incorrect_message"));


            DateTime nuevaExpiracion = ahora.AddMinutes(5);

            const string sqlUpd = @"
UPDATE dbo.Usuario
   SET tiempoExpiracionToken = @nuevaExp
 WHERE idUsuario = @id;";

            db.ExecuteNonQueryAndLog(
                sqlUpd,
                cmd =>
                {
                    cmd.Parameters.Add("@nuevaExp", SqlDbType.DateTime).Value = nuevaExpiracion;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = row.idUsuario;
                },
                userTable, userIdCol, row.idUsuario,
                BE.Audit.AuditEvents.RecuperoContrasenaSolicitud,
                "Extensión de expiración tras verificación OK. IdUsuario=" + row.idUsuario,
                shouldCalculate: true
            );

            return row.idUsuario;
        }

        public void CambiarContrasenaConToken(int idUsuario, string nuevaContrasena)
        {
            if (idUsuario <= 0)
                throw new ArgumentOutOfRangeException(nameof(idUsuario));
            if (string.IsNullOrWhiteSpace(nuevaContrasena))
                throw new ArgumentNullException(nameof(nuevaContrasena));

            const string sqlGet = @"
SELECT TOP 1
    idUsuario,
    tokenRecuperoSesionHash,
    tiempoExpiracionToken
FROM dbo.Usuario
WHERE idUsuario = @id;";

            var row = db.QuerySingleOrDefaultAndLog<UsuarioTokenRow>(
                sqlGet,
                c => c.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarios,
                "Validación de token previo a cambio de contraseña. IdUsuario=" + idUsuario
            );

            if (row == null || string.IsNullOrWhiteSpace(row.tokenRecuperoSesionHash))
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("security_code_incorrect_message"));

            var ahora = DateTime.Now;
            if (!row.tiempoExpiracionToken.HasValue || ahora > row.tiempoExpiracionToken.Value)
                throw new InvalidOperationException(Genericos.TraduccionContext.Traducir("security_code_expired_message"));

            string nuevaHashIrreversible = segUtils.EncriptarIrreversible(nuevaContrasena.Trim());

            const string sqlUpd = @"
UPDATE dbo.Usuario
   SET contrasenaHash           = @hash,
       tokenRecuperoSesionHash  = NULL,
       tiempoExpiracionToken    = NULL,
       contadorIntentosFallidos = 0,
       Bloqueado                = 0
 WHERE idUsuario = @id;";

            db.ExecuteNonQueryAndLog(
                sqlUpd,
                cmd =>
                {
                    cmd.Parameters.Add("@hash", SqlDbType.VarChar, -1).Value = (object)nuevaHashIrreversible ?? DBNull.Value;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                },
                userTable, userIdCol, idUsuario,
                BE.Audit.AuditEvents.ModificacionUsuario,
                "Cambio de contraseña (recupero). IdUsuario=" + idUsuario,
                shouldCalculate: true
            );
        }

        public void IncrementarIntentosFallidos(int idUsuario, int nuevosIntentos)
        {
            string sql = @"
UPDATE " + userTable + @"
   SET contadorIntentosFallidos = @intentos
 WHERE " + userIdCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@intentos", SqlDbType.Int).Value = nuevosIntentos;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                },
                userTable, userIdCol, idUsuario,
                BE.Audit.AuditEvents.IntentosFallidosAcceso,
                "Intento fallido. IdUsuario=" + idUsuario + " | Intentos=" + nuevosIntentos,
                true
            );
        }

        public void BloquearUsuario(int idUsuario, int intentos)
        {
            string sql = @"
UPDATE " + userTable + @"
   SET Bloqueado = 1,
       contadorIntentosFallidos = @intentos
 WHERE " + userIdCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@intentos", SqlDbType.Int).Value = intentos;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario;
                },
                userTable, userIdCol, idUsuario,
                BE.Audit.AuditEvents.BloquearUsuario,
                "Usuario bloqueado por superar intentos. IdUsuario=" + idUsuario,
                true
            );
        }

        public void ResetearIntentosFallidos(int idUsuario)
        {
            string sql = @"
UPDATE " + userTable + @"
   SET contadorIntentosFallidos = 0
 WHERE " + userIdCol + @" = @id;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd => cmd.Parameters.Add("@id", SqlDbType.Int).Value = idUsuario,
                userTable, userIdCol, idUsuario,
                BE.Audit.AuditEvents.IngresoSesion,
                "Login OK. Reset de intentos. IdUsuario=" + idUsuario,
                true
            );
        }

        public bool VerificarHash(string plainPassword, string storedHash)
        {
            return segUtils.VerificarIrreversible(
                plainPassword ?? string.Empty,
                storedHash ?? string.Empty
            );
        }

        public void VerifyAndRepairAllTablesAuto()
        {
            db.VerifyAndRepairAllTablesAuto();
        }

        private static string GenerarContrasenaAleatoria(int length)
        {
            const string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var sb = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(buffer);
                    int val = System.BitConverter.ToInt32(buffer, 0) & int.MaxValue;
                    sb.Append(pool[val % pool.Length]);
                }
            }
            return sb.ToString();
        }

        private static string GenerarCodigoRecupero(int length)
        {
            const string pool = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var sb = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(buffer);
                    int val = BitConverter.ToInt32(buffer, 0) & int.MaxValue;
                    sb.Append(pool[val % pool.Length]);
                }
            }
            return sb.ToString().ToUpperInvariant();
        }

        private static string SanitizarParaNombreArchivo(string input)
        {
            if (string.IsNullOrEmpty(input)) return "usuario";
            foreach (var c in Path.GetInvalidFileNameChars())
                input = input.Replace(c, '_');
            return input;
        }
    }
}

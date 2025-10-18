using System.Collections.Generic;
using System.Data;

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
            "telefonoContacto, direccionUsuario, numeroDocumento, Bloqueado";

        public List<BE.Usuario> GetAll()
        {
            var sql = "SELECT " + userPublicCols + " FROM " + userTable + ";";

            var list = db.QueryListAndLog<BE.Usuario>(
                sql,
                null,
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarios,
                "Listado de usuarios"
            );

            // Desencriptar correo antes de devolver
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
                        // si fallara el formato, devolvemos lo que viene
                    }
                }
            }

            return list;
        }

        public void Create(BE.Usuario obj)
        {
            var sql = @"
INSERT INTO " + userTable + @"
( nombreUsuario, apellidoUsuario, correoElectronico, telefonoContacto, direccionUsuario,
  numeroDocumento, Bloqueado )
VALUES
( @nombreUsuario, @apellidoUsuario, @correoElectronico, @telefonoContacto, @direccionUsuario,
  @numeroDocumento, @Bloqueado );
SELECT CAST(SCOPE_IDENTITY() AS int);";

            object newId = db.ExecuteScalarAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)obj.NombreUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)obj.ApellidoUsuario ?? System.DBNull.Value;

                    // Encriptar reversible (determinístico) antes de guardar
                    string encMail = segUtils.EncriptarReversible(obj.CorreoElectronico ?? string.Empty);
                    cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)encMail ?? System.DBNull.Value;

                    cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)obj.TelefonoContacto ?? System.DBNull.Value;
                    cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)obj.DireccionUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)obj.NumeroDocumento ?? System.DBNull.Value;
                    cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = obj.Bloqueado;
                },
                userTable, userIdCol,
                BE.Audit.AuditEvents.CreacionUsuario,
                "Alta de usuario: " + (obj.CorreoElectronico ?? string.Empty)
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
    Bloqueado         = @Bloqueado
WHERE " + userIdCol + @" = @idUsuario;";

            db.ExecuteNonQueryAndLog(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = obj.IdUsuario;
                    cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)obj.NombreUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)obj.ApellidoUsuario ?? System.DBNull.Value;

                    // Encriptar reversible (determinístico) antes de actualizar
                    string encMail = segUtils.EncriptarReversible(obj.CorreoElectronico ?? string.Empty);
                    cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)encMail ?? System.DBNull.Value;

                    cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)obj.TelefonoContacto ?? System.DBNull.Value;
                    cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)obj.DireccionUsuario ?? System.DBNull.Value;
                    cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)obj.NumeroDocumento ?? System.DBNull.Value;
                    cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = obj.Bloqueado;
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
    Bloqueado
FROM dbo.Usuario
WHERE correoElectronico = @mail;";

            var row = db.QuerySingleOrDefaultAndLog<UsuarioLoginRow>(
                sql,
                c => c.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value = (mailEnc ?? string.Empty),
                userTable, userIdCol,
                BE.Audit.AuditEvents.ConsultaUsuarioPorCorreo,
                "Búsqueda por correo (login): " + (correoElectronico ?? string.Empty)
            );

            // Desencriptar el correo antes de devolver
            if (row != null)
            {
                try
                {
                    row.correoElectronico =
                        segUtils.DesencriptarReversible(row.correoElectronico ?? string.Empty);
                }
                catch
                {
                    // si falla el desencriptado, devolver crudo
                }
            }

            return row;
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
    }
}

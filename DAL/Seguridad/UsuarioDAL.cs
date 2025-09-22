using System.Collections.Generic;
using System.Data;
using System;

using UserDaoInterface = DAL.Seguridad.DV.IDVDAOInterface<BE.Usuario>;

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

            return db.QueryListAndUpdateDv<BE.Usuario>(
                sql,
                null,                         
                userTable,
                userIdCol,
                u => u.IdUsuario
            );
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

            // Ejecuta y obtiene nuevo ID con el toolkit
            object newId = db.ExecuteScalar(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)obj.NombreUsuario ?? DBNull.Value;
                    cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)obj.ApellidoUsuario ?? DBNull.Value;
                    cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)obj.CorreoElectronico ?? DBNull.Value;
                    cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)obj.TelefonoContacto ?? DBNull.Value;
                    cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)obj.DireccionUsuario ?? DBNull.Value;
                    cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)obj.NumeroDocumento ?? DBNull.Value;
                    cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = obj.Bloqueado;
                }
            );

            if (newId != null && newId != DBNull.Value)
                obj.IdUsuario = System.Convert.ToInt32(newId);

            // Recalcular DVH/DVV de la tabla
            RefreshUserDvs();
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

            db.ExecuteNonQuery(
                sql,
                cmd =>
                {
                    cmd.Parameters.Add("@idUsuario", SqlDbType.Int).Value = obj.IdUsuario;
                    cmd.Parameters.Add("@nombreUsuario", SqlDbType.VarChar, 100).Value = (object)obj.NombreUsuario ?? DBNull.Value;
                    cmd.Parameters.Add("@apellidoUsuario", SqlDbType.VarChar, 100).Value = (object)obj.ApellidoUsuario ?? DBNull.Value;
                    cmd.Parameters.Add("@correoElectronico", SqlDbType.VarChar, 150).Value = (object)obj.CorreoElectronico ?? DBNull.Value;
                    cmd.Parameters.Add("@telefonoContacto", SqlDbType.VarChar, 50).Value = (object)obj.TelefonoContacto ?? DBNull.Value;
                    cmd.Parameters.Add("@direccionUsuario", SqlDbType.VarChar, 150).Value = (object)obj.DireccionUsuario ?? DBNull.Value;
                    cmd.Parameters.Add("@numeroDocumento", SqlDbType.VarChar, 20).Value = (object)obj.NumeroDocumento ?? DBNull.Value;
                    cmd.Parameters.Add("@Bloqueado", SqlDbType.Bit).Value = obj.Bloqueado;
                }
            );

            // Recalcular DVH/DVV de la tabla
            RefreshUserDvs();
        }

        public BE.Usuario GetByCorreoElectronico(string correoElectronico)
        {
            var sql = @"
SELECT TOP 1 " + userPublicCols + @"
FROM " + userTable + @"
WHERE correoElectronico = @mail;";

            return db.QuerySingleOrDefaultAndUpdateDv<BE.Usuario>(
                sql,
                c => c.Parameters.Add("@mail", SqlDbType.VarChar, 150).Value =
                        (correoElectronico ?? string.Empty).Trim(),
                userTable,
                userIdCol,
                u => u.IdUsuario
            );
        }

        private void RefreshUserDvs()
        {
            var sql = "SELECT " + userPublicCols + " FROM " + userTable + ";";

            // No uso el resultado; el toolkit actualiza DVH por fila y DVV (con su DVH) de la tabla
            db.QueryListAndUpdateDv<BE.Usuario>(
                sql,
                null,
                userTable,
                userIdCol,
                u => u.IdUsuario
            );
        }
    }
}

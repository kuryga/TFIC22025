using CredencialesException = BE.Seguridad.CredencialesInvalidasException;
using BloqueadoException = BE.Seguridad.UsuarioBloqueadoException;
using UsuarioDAL = DAL.Seguridad.UsuarioDAL;
using SessionContext = DAL.Seguridad.SessionContext;

namespace BLL.Seguridad
{
    public sealed class LoginBLL
    {
        private static LoginBLL _instance;
        private LoginBLL() { }
        public static LoginBLL GetInstance()
        {
            if (_instance == null) _instance = new LoginBLL();
            return _instance;
        }

        private const int MaxIntentos = 3;

        public bool TryLogin(string correo, string password)
        {
            var dal = UsuarioDAL.GetInstance();
            var row = dal.GetLoginRowByCorreo(correo);

            if (row == null)
                throw new CredencialesException(0);

            if (row.Bloqueado)
                throw new BloqueadoException();

            bool ok = dal.VerificarHash(password, row.contrasenaHash);
            if (!ok)
            {
                int actuales = (row.contadorIntentosFallidos.HasValue ? row.contadorIntentosFallidos.Value : 0);
                int siguientes = actuales + 1;

                if (siguientes > MaxIntentos)
                {
                    dal.BloquearUsuario(row.idUsuario, siguientes);
                    throw new BloqueadoException();
                }
                else
                {
                    dal.IncrementarIntentosFallidos(row.idUsuario, siguientes);
                    throw new CredencialesException(siguientes);
                }
            }

            dal.ResetearIntentosFallidos(row.idUsuario);

            SessionContext.Current.UsuarioId = row.idUsuario;
            SessionContext.Current.UsuarioEmail = row.correoElectronico ?? string.Empty;
            SessionContext.Current.NombreCompleto =
                ((row.nombreUsuario ?? string.Empty) + " " + (row.apellidoUsuario ?? string.Empty)).Trim();

            return true;
        }

        public void Logout()
        {
            DAL.Audit.BitacoraDAL.GetInstance()
                .Log(BE.Audit.AuditEvents.CierreSesion, "Cierre de sesión del usuario actual");

            var ctx = DAL.Seguridad.SessionContext.Current;
            ctx.UsuarioId = null;
            ctx.UsuarioEmail = null;
            ctx.NombreCompleto = null;
        }
    }
}

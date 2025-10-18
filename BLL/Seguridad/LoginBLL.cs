using System;
using System.Security.Cryptography;
using System.Threading;
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
            {
                SleepRandomMs(3000, 7000);
                throw new CredencialesException(0);
            }

            if (row.Bloqueado)
            {
                SleepRandomMs(3000, 7000);
                throw new BloqueadoException();
            }

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

            SessionContext.Current.UsuarioId = row.idUsuario;
            SessionContext.Current.UsuarioEmail = row.correoElectronico ?? string.Empty;
            SessionContext.Current.NombreCompleto =
                ((row.nombreUsuario ?? string.Empty) + " " + (row.apellidoUsuario ?? string.Empty)).Trim();

            dal.ResetearIntentosFallidos(row.idUsuario);

            try
            {
                dal.VerifyAndRepairAllTablesAuto();
            }
            catch
            {
                // nada
            }

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

        private static void SleepRandomMs(int minInclusive, int maxInclusive)
        {
            if (minInclusive < 0 || maxInclusive < minInclusive) return;

            int range = maxInclusive - minInclusive + 1;
            int delayMs;
            // Random
            using (var rng = RandomNumberGenerator.Create())
            {
                var b = new byte[4];
                rng.GetBytes(b);
                uint v = BitConverter.ToUInt32(b, 0);
                delayMs = minInclusive + (int)(v % (uint)range);
            }
            Thread.Sleep(delayMs);
        }
    }
}

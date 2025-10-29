using System;

namespace BE.Seguridad
{
    public class LoginException : Exception
    {
        public LoginException(string message) : base(message) { }
    }

    public class UsuarioNoEncontradoException : LoginException
    {
        public UsuarioNoEncontradoException() : base("El usuario no existe.") { }
    }

    public class UsuarioDeshabilitadoException : LoginException
    {
        public UsuarioDeshabilitadoException()
            : base("El usuario está deshabilitado y no puede iniciar sesión.") { }
    }

    public class UsuarioBloqueadoException : LoginException
    {
        public UsuarioBloqueadoException()
            : base("El usuario está bloqueado. Por motivos de seguridad debe recuperar su contraseña o contactar con un administrador.") { }
    }

    public class CredencialesInvalidasException : LoginException
    {
        public int IntentosActuales { get; }

        public CredencialesInvalidasException(int intentosActuales)
            : base($"Credenciales inválidas. Intentos fallidos: {intentosActuales}.")
        {
            IntentosActuales = intentosActuales;
        }
    }
}

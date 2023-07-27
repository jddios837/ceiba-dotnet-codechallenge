using System;

namespace PruebaIngresoBibliotecario.Api.Exceptions
{
    public class UsuarioConPrestamoException : Exception
    {
        public UsuarioConPrestamoException(string message) : base(message)
        {
        }
    }
}
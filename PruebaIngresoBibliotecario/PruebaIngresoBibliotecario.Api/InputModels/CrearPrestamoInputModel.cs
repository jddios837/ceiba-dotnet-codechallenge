using System;
using PruebaIngresoBibliotecario.Core.Enums;

namespace PruebaIngresoBibliotecario.Api.InputModels
{
    public class CrearPrestamoInputModel
    {
        public Guid Isbn { get; set; }
        public string IdentificacionUsuario { get; set; }
        public TipoUsuarioPrestamo TipoUsuario { get; set; }
    }
}
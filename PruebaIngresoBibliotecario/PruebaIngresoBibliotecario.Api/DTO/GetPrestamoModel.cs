using System;

namespace PruebaIngresoBibliotecario.Api.DTO
{
    public class GetPrestamoModel
    {
        public Guid Id { get; set; }
        public Guid Isbn { get; set; }
        public String IdentificacionUsuario { get; set; }
        public int TipoUsuario { get; set; }
        public DateTime FechaMaximaDevolucion { get; set; }
    }
}
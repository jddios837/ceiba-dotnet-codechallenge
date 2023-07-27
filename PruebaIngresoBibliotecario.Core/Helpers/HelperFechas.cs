using System;
using System.Linq;
using PruebaIngresoBibliotecario.Core.Enums;

namespace PruebaIngresoBibliotecario.Core.Helpers
{
    public static class HelperFechas
    {
        public static DateTime CalcularFechaEntrega(TipoUsuarioPrestamo tipoUsuario)
        {
            var weekend = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var fechaDevolucion = DateTime.Now;
            int diasPrestamo = tipoUsuario switch
            {
                TipoUsuarioPrestamo.AFILIADO => 10,
                TipoUsuarioPrestamo.EMPLEADO => 8,
                TipoUsuarioPrestamo.INVITADO => 7,
                _ => -1,
            };

            for (int i = 0; i < diasPrestamo;)
            {
                fechaDevolucion = fechaDevolucion.AddDays(1);
                i = (!weekend.Contains(fechaDevolucion.DayOfWeek)) ? ++i : i;                
            }

            return fechaDevolucion;
        }
    }
}
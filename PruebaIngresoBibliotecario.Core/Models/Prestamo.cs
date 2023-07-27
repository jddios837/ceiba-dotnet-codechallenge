using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaIngresoBibliotecario.Core.Models
{
    public class Prestamo
    {
        [Key]
        public Guid Id { get; set; }

        public virtual Libro Libro { get; set; }
        
        public virtual Usuario Usuario { get; set; }

        public DateTime FechaMaximaDevolucion { get; set; }
    }
}
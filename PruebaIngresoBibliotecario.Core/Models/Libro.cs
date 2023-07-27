using System;
using System.ComponentModel.DataAnnotations;

namespace PruebaIngresoBibliotecario.Core.Models
{
    public class Libro
    {
        [Key]
        public Guid Isbn { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using PruebaIngresoBibliotecario.Core.Enums;

namespace PruebaIngresoBibliotecario.Core.Models
{
    public class Usuario
    {
        [Key]
        [StringLength(10)]
        public string IdentificacionUsuario { get; set; }
        
        [StringLength(15)]
        public string Nombre { get; set; }
        
        public TipoUsuarioPrestamo tipoUsuario { get; set; }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using PruebaIngresoBibliotecario.Core.Enums;
using PruebaIngresoBibliotecario.Core.Models;

namespace PruebaIngresoBibliotecario.Infrastructure
{
    public class PersistenceContext : DbContext
    {

        private readonly IConfiguration Config;

        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Libro> Libros { get; set; }

        public PersistenceContext(DbContextOptions<PersistenceContext> options) : base(options)
        {
        
        }
        public PersistenceContext(DbContextOptions<PersistenceContext> options, IConfiguration config) : base(options)
        {
            Config = config;
        }

        public async Task CommitAsync()
        {
            await SaveChangesAsync().ConfigureAwait(false);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema(Config.GetValue<string>("SchemaName"));

            //base.OnModelCreating(modelBuilder);
        }
        
        public void InitializeData()
        {
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                throw new InvalidOperationException("Este método solo debe usarse con la base de datos en memoria.");
            }

            if (!Libros.Any() && !Usuarios.Any())
            {
                // Agregar datos de prueba
                Libros.Add(new Libro { Isbn = new Guid("11f6a454-cc06-4385-8c5d-8d053449102a"), Nombre = "Libro de Prueba 1" });
                Libros.Add(new Libro {Isbn = new Guid("977ef918-f8dc-492c-98d6-66dfe90e8947"), Nombre = "Libro de Prueba 2" });
                Libros.Add(new Libro {Isbn = new Guid("520d6e5f-4a28-49d5-8e35-f6549c97eb4e"), Nombre = "Libro de Prueba 3" });
                
                Usuarios.Add(new Usuario { IdentificacionUsuario = "1234567891", Nombre = "Pepito Perez", tipoUsuario = TipoUsuarioPrestamo.INVITADO});
                Usuarios.Add(new Usuario { IdentificacionUsuario = "3568778897", Nombre = "Luisa Maria", tipoUsuario = TipoUsuarioPrestamo.INVITADO});
                Usuarios.Add(new Usuario { IdentificacionUsuario = "8877789999", Nombre = "Pepita Perez", tipoUsuario = TipoUsuarioPrestamo.INVITADO});
                Usuarios.Add(new Usuario { IdentificacionUsuario = "2345678543", Nombre = "Luis Angel", tipoUsuario = TipoUsuarioPrestamo.AFILIADO});
                Usuarios.Add(new Usuario { IdentificacionUsuario = "2336788956", Nombre = "Maria Estela", tipoUsuario = TipoUsuarioPrestamo.EMPLEADO});

                SaveChanges();
            }
        }
    }
}

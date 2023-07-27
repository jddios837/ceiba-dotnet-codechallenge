using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PruebaIngresoBibliotecario.Api.DTO;
using PruebaIngresoBibliotecario.Api.Exceptions;
using PruebaIngresoBibliotecario.Api.InputModels;
using PruebaIngresoBibliotecario.Api.Services.Interfaces;
using PruebaIngresoBibliotecario.Core.Enums;
using PruebaIngresoBibliotecario.Core.Helpers;
using PruebaIngresoBibliotecario.Core.Models;
using PruebaIngresoBibliotecario.Infrastructure;

namespace PruebaIngresoBibliotecario.Api.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly PersistenceContext _context;
        
        public PrestamoService(PersistenceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prestamo>> GetAll()
        {
            return await _context
                .Prestamos
                .Include(e => e.Libro)
                .Include(e => e.Usuario).ToListAsync();
        }

        public async Task<Prestamo> Create(CrearPrestamoInputModel prestamo)
        {
            // Verificar si el libro existe
            var libro = await _context.Libros.FindAsync(prestamo.Isbn);
            if (libro == null)
            {
                throw new Exception($"El libro con ISBN {prestamo.Isbn} no existe.");
            }
            
            // Verificar si el usuario existe
            var usuario = await _context.Usuarios.FindAsync(prestamo.IdentificacionUsuario);
            if (usuario == null)
            {
                throw new Exception($"El usuario con ID {prestamo.IdentificacionUsuario} no existe.");
            }
            
            // Verificar si el usuario ya tiene un libro prestado
            bool usuarioTienePrestamo = await _context.Prestamos
                .AnyAsync(p => 
                    p.Usuario.IdentificacionUsuario == prestamo.IdentificacionUsuario && 
                    p.Usuario.tipoUsuario == TipoUsuarioPrestamo.INVITADO);

            if (usuarioTienePrestamo)
            {
                throw new UsuarioConPrestamoException($"El usuario con identificacion {usuario.IdentificacionUsuario} ya tiene un libro prestado por lo cual no se le puede realizar otro prestamo");
            }
            
            // Creo modelo y guardo en la BD
            var newPrestamo = new Prestamo
            {
                Libro = libro,
                Usuario = usuario,
                FechaMaximaDevolucion = HelperFechas.CalcularFechaEntrega(prestamo.TipoUsuario)
            };

            _context.Prestamos.Add(newPrestamo);
            await _context.SaveChangesAsync();
            return newPrestamo;
        }

        public async Task<GetPrestamoModel> Get(Guid id)
        {
            // Verificar si el prestamo existe
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (prestamo == null)
            {
                throw new Exception($"El prestamo con id {id} no existe");
            }

            return new GetPrestamoModel()
            {
                Id = prestamo.Id,
                Isbn = prestamo.Libro.Isbn,
                IdentificacionUsuario = prestamo.Usuario.IdentificacionUsuario,
                TipoUsuario = (int)prestamo.Usuario.tipoUsuario,
                FechaMaximaDevolucion = prestamo.FechaMaximaDevolucion
            };
        }
    }
}
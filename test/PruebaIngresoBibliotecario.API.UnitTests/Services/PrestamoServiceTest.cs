using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PruebaIngresoBibliotecario.Api.Exceptions;
using PruebaIngresoBibliotecario.Api.InputModels;
using PruebaIngresoBibliotecario.Api.Services;
using PruebaIngresoBibliotecario.Core.Enums;
using PruebaIngresoBibliotecario.Core.Models;
using PruebaIngresoBibliotecario.Infrastructure;
using Xunit;

namespace PruebaIngresoBibliotecario.API.UnitTests.Services
{
    public class PrestamoServiceTest
    {
        private PersistenceContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<PersistenceContext>()
                .UseInMemoryDatabase(databaseName: "test_db")
                .Options;
            
            var context = new PersistenceContext(options);
            context.InitializeData();
            
            return context;
        }

        [Fact]
        public async void Should_Create_Prestamo()
        {
            // Arrange
            var dbContext = GetDbContext();
            var _service = new PrestamoService(dbContext);
            var prestamo = new CrearPrestamoInputModel()
            {
                Isbn = new Guid("11f6a454-cc06-4385-8c5d-8d053449102a"),
                IdentificacionUsuario = "1234567891",
                TipoUsuario = TipoUsuarioPrestamo.AFILIADO
            };

            // Act
            var result = await _service.Create(prestamo);

            // Assert
            result.Should().NotBeNull();
            result.FechaMaximaDevolucion.Should().BeAfter(DateTime.Now);
        }
        
        [Fact]
        public async void Should_Throw_Exception_When_Libro_Does_Not_Exist()
        {
            // Arrange
            var dbContext = GetDbContext();
            var _service = new PrestamoService(dbContext);
            var prestamo = new CrearPrestamoInputModel()
            {
                Isbn = Guid.NewGuid(),
                IdentificacionUsuario = "1234567891",
                TipoUsuario = TipoUsuarioPrestamo.AFILIADO
            };

            // Act
            Func<Task> action = async () => await _service.Create(prestamo);

            // Assert
            await action
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"El libro con ISBN {prestamo.Isbn} no existe.");
        }

        [Fact]
        public async void Should_Throw_Exception_When_Usuario_Does_Not_Exist()
        {
            // Arrange
            var dbContext = GetDbContext();
            var _service = new PrestamoService(dbContext);
            var prestamo = new CrearPrestamoInputModel()
            {
                Isbn = new Guid("11f6a454-cc06-4385-8c5d-8d053449102a"),
                IdentificacionUsuario = "999999999",
                TipoUsuario = TipoUsuarioPrestamo.AFILIADO
            };

            // Act
            Func<Task> action = async () => await _service.Create(prestamo);

            // Assert
            await action
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"El usuario con ID {prestamo.IdentificacionUsuario} no existe.");
        }

        [Fact]
        public async void Should_Throw_Exception_When_Usuario_Already_Has_Prestamo()
        {
            // Arrange
            var dbContext = GetDbContext();
            var _service = new PrestamoService(dbContext);
            var prestamo = new CrearPrestamoInputModel()
            {
                Isbn = new Guid("520d6e5f-4a28-49d5-8e35-f6549c97eb4e"),
                IdentificacionUsuario = "8877789999",
                TipoUsuario = TipoUsuarioPrestamo.INVITADO
            };

            // Create a prestamo for the user
            await _service.Create(prestamo);

            // Act
            Func<Task> action = async () => await _service.Create(prestamo);

            // Assert
            await action
                .Should()
                .ThrowAsync<UsuarioConPrestamoException>()
                .WithMessage($"El usuario con identificacion {prestamo.IdentificacionUsuario} ya tiene un libro prestado por lo cual no se le puede realizar otro prestamo");
        }
        
        [Fact]
        public async void Should_Get_Prestamo_By_Id()
        {
            // Arrange
            var dbContext = GetDbContext();
            var _service = new PrestamoService(dbContext);

            // Create a new prestamo
            var prestamo = new CrearPrestamoInputModel()
            {
                Isbn = new Guid("977ef918-f8dc-492c-98d6-66dfe90e8947"),
                IdentificacionUsuario = "3568778897",
                TipoUsuario = TipoUsuarioPrestamo.AFILIADO
            };
            var createdPrestamo = await _service.Create(prestamo);

            // Act
            var result = await _service.Get(createdPrestamo.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(createdPrestamo.Id);
            result.Isbn.Should().Be(createdPrestamo.Libro.Isbn);
            result.IdentificacionUsuario.Should().Be(createdPrestamo.Usuario.IdentificacionUsuario);
            result.TipoUsuario.Should().Be((int)createdPrestamo.Usuario.tipoUsuario);
            result.FechaMaximaDevolucion.Should().Be(createdPrestamo.FechaMaximaDevolucion);
        }

        [Fact]
        public async void Should_Throw_Exception_When_Prestamo_Not_Found()
        {
            // Arrange
            var dbContext = GetDbContext();
            var _service = new PrestamoService(dbContext);
            var nonExistingId = Guid.NewGuid();

            // Act
            Func<Task> action = async () => await _service.Get(nonExistingId);

            // Assert
            await action
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage($"El prestamo con id {nonExistingId} no existe");
        }
        
        // TODO: Para efectos de un proyecto real se pueden agregar mas pruebas
    }
}
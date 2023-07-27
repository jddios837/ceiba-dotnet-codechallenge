using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;
using PruebaIngresoBibliotecario.Api.InputModels;
using PruebaIngresoBibliotecario.Api.Validators;
using PruebaIngresoBibliotecario.Core.Enums;
using Xunit;

namespace PruebaIngresoBibliotecario.API.UnitTests.Validators
{
    public class CrearPrestamoInputModelValidatorTest
    {
        private readonly CrearPrestamoInputModelValidator _validator;
        private readonly CrearPrestamoInputModel _model;
        
        public CrearPrestamoInputModelValidatorTest()
        {
            _validator = new CrearPrestamoInputModelValidator();
            _model = new CrearPrestamoInputModel();
        }

        [Fact]
        public void CrearPrestamo_Isbn_Empty()
        {
            // Arrange
            _model.Isbn = Guid.Empty;
            
            // Act
            var result = _validator.TestValidate(_model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Isbn);
        }
        
        [Fact]
        public void CrearPrestamo_Isbn_Valid()
        {
            // Arrange
            _model.Isbn = new Guid("9e39e415-925d-4012-ad8b-a14b96fa31a1");
            
            // Act
            var result = _validator.TestValidate(_model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Isbn);
        }
        
        [Theory]
        [MemberData(nameof(EnumValues))]
        public void Should_Not_Have_Error_Enum_EmployeeRole(TipoUsuarioPrestamo value)
        {
            // Arrange
            _model.TipoUsuario = value;
        
            // Act
            var result = _validator.TestValidate(_model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TipoUsuario);
        }
        
        public static IEnumerable<object[]> EnumValues()
        {
            foreach (var number in Enum.GetValues(typeof(TipoUsuarioPrestamo)))
            {
                yield return new object[] { number };
            }
        }
        
        // TODO: Para efectos de un proyecto real se pueden agregar mas pruebas
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PruebaIngresoBibliotecario.Api.DTO;
using PruebaIngresoBibliotecario.Api.InputModels;
using PruebaIngresoBibliotecario.Core.Models;

namespace PruebaIngresoBibliotecario.Api.Services.Interfaces
{
    public interface IPrestamoService
    {
        public Task<IEnumerable<Prestamo>> GetAll();
        public Task<Prestamo> Create(CrearPrestamoInputModel prestamo);
        public Task<GetPrestamoModel> Get(Guid id);
    }
}
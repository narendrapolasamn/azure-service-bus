using carModel = Car.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Car.Domain.Repositories.Interfaces
{
    public interface ICarsCatalogRepository
    {
        Task<carModel.Car> GetByIdAsync(Guid id);
        Task<IReadOnlyList<carModel.Car>> ListAllAsync();
        carModel.Car Add(carModel.Car car);
        void Update(carModel.Car car);
        void Delete(carModel.Car car);
    }
}

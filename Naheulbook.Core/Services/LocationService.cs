using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface ILocationService
    {
        Task<List<Location>> GetAllLocationsAsync();
        Task<List<LocationMap>> GetLocationMapsAsync(int locationId);
        Task<List<Location>> SearchLocationAsync(string filter);
    }

    public class LocationService : ILocationService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public LocationService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<List<Location>> GetAllLocationsAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Locations.GetAllAsync();
            }
        }

        public async Task<List<LocationMap>> GetLocationMapsAsync(int locationId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Locations.GetLocationMapsByLocationIdAsync(locationId);
            }
        }

        public async Task<List<Location>> SearchLocationAsync(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return new List<Location>();
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Locations.SearchByNameAsync(filter, 10);
            }
        }
    }
}
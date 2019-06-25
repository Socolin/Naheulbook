using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services
{
    public interface IIconService
    {
        Task<List<Icon>> GetAllIconsAsync();
    }

    public class IconService : IIconService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public IconService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<List<Icon>> GetAllIconsAsync()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Icons.GetAllAsync();
            }
        }
    }
}
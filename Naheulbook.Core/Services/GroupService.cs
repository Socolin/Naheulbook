using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IGroupService
    {
        Task<Group> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request);
    }

    public class GroupService : IGroupService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public GroupService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<Group> CreateGroupAsync(NaheulbookExecutionContext executionContext, CreateGroupRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var location = await uow.Locations.GetNewGroupDefaultLocationAsync();

                var group = new Group
                {
                    Name = request.Name,
                    MasterId = executionContext.UserId,
                    Location = location
                };

                uow.Groups.Add(group);
                await uow.CompleteAsync();

                return group;
            }
        }
    }
}
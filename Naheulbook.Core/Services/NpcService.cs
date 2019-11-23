using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Services
{
    public interface INpcService
    {
        Task<List<Npc>> LoadNpcsAsync(NaheulbookExecutionContext executionContext, int groupId);
        Task<Npc> CreateNpcAsync(NaheulbookExecutionContext executionContext, int groupId, CreateNpcRequest request);
    }

    public class NpcService : INpcService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IJsonUtil _jsonUtil;

        public NpcService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            IJsonUtil jsonUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _jsonUtil = jsonUtil;
        }

        public async Task<List<Npc>> LoadNpcsAsync(NaheulbookExecutionContext executionContext, int groupId)
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var group = await uow.Groups.GetAsync(groupId);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            return await uow.Npcs.GetByGroupIdAsync(groupId);
        }

        public async Task<Npc> CreateNpcAsync(NaheulbookExecutionContext executionContext, int groupId, CreateNpcRequest request)
        {
            using var uow = _unitOfWorkFactory.CreateUnitOfWork();

            var group = await uow.Groups.GetAsync(groupId);
            if (group == null)
                throw new GroupNotFoundException(groupId);

            _authorizationUtil.EnsureIsGroupOwner(executionContext, group);

            var npc = new Npc
            {
                GroupId = groupId,
                Name = request.Name,
                Data = _jsonUtil.Serialize(request.Data)
            };

            uow.Npcs.Add(npc);

            await uow.SaveChangesAsync();

            return npc;
        }
    }
}
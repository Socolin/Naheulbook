#pragma warning disable 8619
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IGroupRepository : IRepository<GroupEntity>
{
    Task<List<GroupEntity>> GetGroupsOwnedByAsync(int userId);
    Task<GroupEntity?> GetGroupsWithDetailsAsync(int groupId);
    Task<GroupEntity?> GetGroupsWithCharactersAsync(int groupId);
}

public class GroupRepository(NaheulbookDbContext context) : Repository<GroupEntity, NaheulbookDbContext>(context), IGroupRepository
{
    public Task<List<GroupEntity>> GetGroupsOwnedByAsync(int userId)
    {
        return Context.Groups
            .Include(g => g.Characters)
            .Where(g => g.MasterId == userId)
            .ToListAsync();
    }

    public Task<GroupEntity?> GetGroupsWithDetailsAsync(int groupId)
    {
        return Context.Groups
            .Include(g => g.Characters)
            .Include(g => g.Invites)
            .ThenInclude(g => g.Character)
            .ThenInclude(g => g.Origin)
            .Include(g => g.Invites)
            .ThenInclude(g => g.Character)
            .ThenInclude(g => g.Jobs)
            .ThenInclude(g => g.Job)
            .Where(g => g.Id == groupId)
            .SingleOrDefaultAsync();
    }

    public Task<GroupEntity?> GetGroupsWithCharactersAsync(int groupId)
    {
        return Context.Groups
            .Include(g => g.Characters)
            .Where(g => g.Id == groupId)
            .SingleOrDefaultAsync();
    }
}
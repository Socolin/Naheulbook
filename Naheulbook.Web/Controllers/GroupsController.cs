using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Filters;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{

    [Route("api/v2/groups")]
    [ApiController]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GroupsController(IGroupService groupService, IMapper mapper)
        {
            _groupService = groupService;
            _mapper = mapper;
        }

        [ServiceFilter(typeof(JwtAuthorizationFilter))]
        [HttpPost]
        public async Task<CreatedActionResult<GroupResponse>> PostCreateTypeAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            CreateGroupRequest request
        )
        {
            var group = await _groupService.CreateGroupAsync(executionContext, request);
            return _mapper.Map<GroupResponse>(group);
        }
    }
}
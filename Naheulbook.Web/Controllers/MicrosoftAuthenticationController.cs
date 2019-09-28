using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Shared.Clients.MicrosoftGraph;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/authentications/microsoft")]
    public class MicrosoftAuthenticationController : Controller
    {
        private const string MicrosoftLoginTokenKey = "microsoftLoginToken";

        private readonly MicrosoftGraphConfiguration _configuration;
        private readonly IMicrosoftGraphClient _microsoftGraphClient;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRngUtil _rngUtil;
        private readonly ISocialMediaUserLinkService _socialMediaUserLinkService;

        public MicrosoftAuthenticationController(
            MicrosoftGraphConfiguration configuration,
            IMicrosoftGraphClient microsoftGraphClient,
            IJwtService jwtService,
            IMapper mapper,
            IRngUtil rngUtil,
            ISocialMediaUserLinkService socialMediaUserLinkService
        )
        {
            _configuration = configuration;
            _microsoftGraphClient = microsoftGraphClient;
            _jwtService = jwtService;
            _mapper = mapper;
            _rngUtil = rngUtil;
            _socialMediaUserLinkService = socialMediaUserLinkService;
        }

        [HttpGet("initOAuthAuthentication")]
        public ActionResult<AuthenticationInitResponse> PostInitOauthAuthentication()
        {
            var loginToken = _rngUtil.GetRandomHexString(64);

            HttpContext.Session.SetString(MicrosoftLoginTokenKey, loginToken);

            return new AuthenticationInitResponse
            {
                LoginToken = loginToken,
                AppKey = _configuration.AppId
            };
        }

        [HttpPost("completeAuthentication")]
        public async Task<ActionResult<UserJwtResponse>> CompleteMicrosoftAuthenticationAsync(
            [FromBody] CompleteMicrosoftAuthenticationRequest request
        )
        {
            var loginToken = HttpContext.Session.GetString(MicrosoftLoginTokenKey);

            if (loginToken != request.LoginToken)
                return BadRequest();

            var accessToken = await _microsoftGraphClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
            var profile = await _microsoftGraphClient.GetUserProfileAsync(accessToken);

            var currentUserId = HttpContext.Session.GetCurrentUserId();
            if (currentUserId.HasValue)
                await _socialMediaUserLinkService.AssociateUserToMicrosoftIdAsync(currentUserId.Value, profile.Id);

            var user = await _socialMediaUserLinkService.GetOrCreateUserFromMicrosoftAsync(profile.Name, profile.Id);

            HttpContext.Session.SetCurrentUserId(user.Id);
            var token = _jwtService.GenerateJwtToken(user.Id);

            return new UserJwtResponse
            {
                Token = token,
                UserInfo = _mapper.Map<UserInfoResponse>(user)
            };
        }
    }
}
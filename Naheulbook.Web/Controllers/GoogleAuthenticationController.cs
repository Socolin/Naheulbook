using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Shared.Clients.Google;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/authentications/google")]
    public class GoogleAuthenticationController : Controller
    {
        private const string GoogleLoginTokenKey = "googleLoginToken";

        private readonly GoogleConfiguration _configuration;
        private readonly IGoogleClient _googleClient;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRngUtil _rngUtil;
        private readonly ISocialMediaUserLinkService _socialMediaUserLinkService;

        public GoogleAuthenticationController(
            GoogleConfiguration configuration,
            IGoogleClient googleClient,
            IJwtService jwtService,
            IMapper mapper,
            IRngUtil rngUtil,
            ISocialMediaUserLinkService socialMediaUserLinkService
        )
        {
            _configuration = configuration;
            _googleClient = googleClient;
            _jwtService = jwtService;
            _mapper = mapper;
            _rngUtil = rngUtil;
            _socialMediaUserLinkService = socialMediaUserLinkService;
        }

        [HttpGet("initOAuthAuthentication")]
        public ActionResult<AuthenticationInitResponse> PostInitOauthAuthentication()
        {
            var loginToken = _rngUtil.GetRandomHexString(64);

            HttpContext.Session.SetString(GoogleLoginTokenKey, loginToken);

            return new AuthenticationInitResponse
            {
                LoginToken = loginToken,
                ServiceName = "google",
                AppKey = _configuration.AppId
            };
        }

        [HttpPost("completeAuthentication")]
        public async Task<ActionResult<UserJwtResponse>> CompleteGoogleAuthenticationAsync(
            [FromBody] CompleteGoogleAuthenticationRequest request
        )
        {
            var loginToken = HttpContext.Session.GetString(GoogleLoginTokenKey);

            if (loginToken != request.LoginToken)
                return BadRequest();

            var accessToken = await _googleClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
            var profile = await _googleClient.GetUserProfileAsync(accessToken);

            var currentUserId = HttpContext.Session.GetCurrentUserId();
            if (currentUserId.HasValue)
                await _socialMediaUserLinkService.AssociateUserToGoogleIdAsync(currentUserId.Value, profile.Id);

            var user = await _socialMediaUserLinkService.GetOrCreateUserFromGoogleAsync(profile.DisplayName, profile.Id);

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
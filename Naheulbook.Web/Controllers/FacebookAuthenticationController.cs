using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Shared.Clients.Facebook;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/authentications/facebook")]
    public class FacebookAuthenticationController : Controller
    {
        private const string FacebookLoginTokenKey = "facebookLoginToken";

        private readonly FacebookConfiguration _configuration;
        private readonly IFacebookClient _facebookClient;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRngUtil _rngUtil;
        private readonly ISocialMediaUserLinkService _socialMediaUserLinkService;

        public FacebookAuthenticationController(
            FacebookConfiguration configuration,
            IFacebookClient facebookClient,
            IJwtService jwtService,
            IMapper mapper,
            IRngUtil rngUtil,
            ISocialMediaUserLinkService socialMediaUserLinkService
        )
        {
            _configuration = configuration;
            _facebookClient = facebookClient;
            _jwtService = jwtService;
            _mapper = mapper;
            _rngUtil = rngUtil;
            _socialMediaUserLinkService = socialMediaUserLinkService;
        }

        [HttpGet("initOAuthAuthentication")]
        public ActionResult<AuthenticationInitResponse> PostInitOauthAuthentication()
        {
            var loginToken = _rngUtil.GetRandomHexString(64);

            HttpContext.Session.SetString(FacebookLoginTokenKey, loginToken);

            return new AuthenticationInitResponse
            {
                LoginToken = loginToken,
                AppKey = _configuration.AppId
            };
        }

        [HttpPost("completeAuthentication")]
        public async Task<ActionResult<UserJwtResponse>> CompleteFacebookAuthenticationAsync(
            [FromBody] CompleteFacebookAuthenticationRequest request
        )
        {
            var loginToken = HttpContext.Session.GetString(FacebookLoginTokenKey);

            if (loginToken != request.LoginToken)
                return BadRequest();

            var accessToken = await _facebookClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
            var profile = await _facebookClient.GetUserProfileAsync(accessToken);

            var currentUserId = HttpContext.Session.GetCurrentUserId();
            if (currentUserId.HasValue)
                await _socialMediaUserLinkService.AssociateUserToFacebookIdAsync(currentUserId.Value, profile.Id);

            var user = await _socialMediaUserLinkService.GetOrCreateUserFromFacebookAsync(profile.Name, profile.Id);

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
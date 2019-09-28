using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Shared.Clients.Twitter;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers
{
    [ApiController]
    [Route("api/v2/authentications/twitter")]
    public class TwitterAuthenticationController : Controller
    {
        private const string TwitterOauthTokenKey = "twitterOauthToken";

        private readonly ITwitterClient _twitterClient;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ISocialMediaUserLinkService _socialMediaUserLinkService;

        public TwitterAuthenticationController(
            ITwitterClient twitterClient,
            IJwtService jwtService,
            IMapper mapper,
            ISocialMediaUserLinkService socialMediaUserLinkService
        )
        {
            _twitterClient = twitterClient;
            _jwtService = jwtService;
            _mapper = mapper;
            _socialMediaUserLinkService = socialMediaUserLinkService;
        }

        [HttpGet("initOAuthAuthentication")]
        public async Task<ActionResult<AuthenticationInitResponse>> PostInitOauthAuthenticationAsync()
        {
            var requestToken =  await _twitterClient.GetRequestTokenAsync();

            HttpContext.Session.SetString(TwitterOauthTokenKey, requestToken.OAuthToken);

            return new AuthenticationInitResponse
            {
                LoginToken = requestToken.OAuthToken
            };
        }

        [HttpPost("completeAuthentication")]
        public async Task<ActionResult<UserJwtResponse>> CompleteTwitterAuthenticationAsync(
            [FromBody] CompleteTwitterAuthenticationRequest request
        )
        {
            var loginToken = HttpContext.Session.GetString(TwitterOauthTokenKey);
            if (loginToken == null)
                return BadRequest();


            var accessToken = await _twitterClient.GetAccessTokenAsync(loginToken, request.OAuthToken, request.OauthVerifier);

            var currentUserId = HttpContext.Session.GetCurrentUserId();
            if (currentUserId.HasValue)
                await _socialMediaUserLinkService.AssociateUserToTwitterIdAsync(currentUserId.Value, accessToken.UserId);

            var user = await _socialMediaUserLinkService.GetOrCreateUserFromTwitterAsync(accessToken.ScreenName, accessToken.UserId);

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
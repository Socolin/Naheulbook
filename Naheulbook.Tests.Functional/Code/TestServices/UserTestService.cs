using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Steps;
using Naheulbook.Tests.Functional.Code.Utils;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.FakeSmtp;

namespace Naheulbook.Tests.Functional.Code.TestServices
{
    public class UserTestService
    {
        private readonly NaheulbookHttpClient _naheulbookHttpClient;
        private readonly IMailReceiver _mailReceiver;
        private readonly DbContextOptions<NaheulbookDbContext> _dbContextOptions;

        public UserTestService(NaheulbookHttpClient naheulbookHttpClient, IMailReceiver mailReceiver, DbContextOptions<NaheulbookDbContext> dbContextOptions)
        {
            _naheulbookHttpClient = naheulbookHttpClient;
            _mailReceiver = mailReceiver;
            _dbContextOptions = dbContextOptions;
        }

        public async Task CreateUserAsync(string username, string password)
        {
            await _naheulbookHttpClient.PostAsync("/api/v2/users/", new {username, password});

            var activationCode = MailSteps.ParseActivationCodeFromActivationMail(_mailReceiver.Mails.LastOrDefault(m => m.To.Contains(username)));

            await _naheulbookHttpClient.PostAsync($"/api/v2/users/{username}/validate", new {activationCode});
        }

        public async Task<(string username, string password)> CreateUserAsync()
        {
            var username = $"user.{RngUtils.GetRandomHexString(16)}@test.ca";
            var password = RngUtils.GetRandomHexString(32);

            await CreateUserAsync(username, password);

            return (username, password);
        }

        public async Task SetUserAdminAsync(string username)
        {
            using (var dbContext = new NaheulbookDbContext(_dbContextOptions))
            {
                var user = await dbContext.Users.FirstAsync(u => u.Username == username);
                user.Admin = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<string> GenerateJwtAsync(string username, string password)
        {
            var response = await _naheulbookHttpClient.PostAndParseJsonResultAsync<JObject>($"/api/v2/users/{username}/jwt", new {password});
            var jwt = response.Value<string>("token");
            return jwt;
        }
    }
}
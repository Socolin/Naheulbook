#nullable enable
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Steps;
using Naheulbook.TestUtils;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Socolin.TestUtils.FakeSmtp;

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

        public async Task<int> CreateUserAsync(string username, string password)
        {
            var responseMessage = await _naheulbookHttpClient.PostAsync("/api/v2/users/", new {username, password});
            if (!responseMessage.IsSuccessStatusCode)
            {
                var content = await responseMessage.Content.ReadAsStringAsync();
                Assert.Fail($"Failed to create user {username}. StatusCode: {responseMessage.StatusCode} Content:'\n{content}'\n");
            }

            var mail = await GetMailAndWaitIfNotReady(username, 2000);
            if (mail == null)
                Assert.Fail($"No mail received for user: {username}");
            var activationCode = MailSteps.ParseActivationCodeFromActivationMail(mail!);

            await _naheulbookHttpClient.PostAsync($"/api/v2/users/{username}/validate", new {activationCode});

            using (var dbContext = new NaheulbookDbContext(_dbContextOptions))
            {
                var user = await dbContext.Users.FirstAsync(u => u.Username == username);
                return user.Id;
            }
        }

        private async Task<FakeSmtpMail?> GetMailAndWaitIfNotReady(string email, int delay)
        {
            var sw = Stopwatch.StartNew();
            FakeSmtpMail? mail;
            do
            {
                mail = _mailReceiver.Mails.LastOrDefault(m => m.To.Contains(email));
                if (mail == null)
                    await Task.Delay(100);
            } while (mail == null && sw.ElapsedMilliseconds < delay);

            return mail;
        }

        public async Task<(string username, string password, int userId)> CreateUserAsync()
        {
            var username = $"user.{RngUtil.GetRandomHexString(16)}@test.ca";
            var password = RngUtil.GetRandomHexString(32);

            var userId = await CreateUserAsync(username, password);

            return (username, password, userId);
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
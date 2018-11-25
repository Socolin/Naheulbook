using System.Linq;
using System.Text.RegularExpressions;
using Naheulbook.Tests.Functional.Code.Extensions;
using NUnit.Framework;
using Socolin.TestsUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
    [Binding]
    public class MailSteps
    {
        private readonly IMailReceiver _mailReceiver;
        private readonly ScenarioContext _scenarioContext;

        public MailSteps(IMailReceiver mailReceiver, ScenarioContext scenarioContext)
        {
            _mailReceiver = mailReceiver;
            _scenarioContext = scenarioContext;
        }

        [Then(@"a mail validation mail has been sent to ""(.*)""")]
        public void ThenAMailValidationMailHasBeenSentTo(string email)
        {
            var mail = _mailReceiver.Mails.LastOrDefault(m => m.To.Contains(email));

            if (mail == null)
            {
                var mailsDetails = string.Join("\n", _mailReceiver.Mails.Select(m => "'" + string.Join("','", m.To) + "' - " + m.Subject.FirstOrDefault()));
                Assert.Fail($"Did not received any mail with destination: {email}. Received {_mailReceiver.Mails.Count} mails:\n{mailsDetails}");
            }
            _scenarioContext.SetLastReceivedMail(mail);

            var match = Regex.Match(mail.Data, "ActivationCode: (?<activationCode>[a-f0-9]+)");
            if (!match.Success)
            {
                Assert.Fail($"Fail to find activation code in mail body:\n'{mail.Data}'");
            }

            var activationCode = match.Groups["activationCode"].Value;
            _scenarioContext.SetActivationCode(activationCode);
        }
    }
}
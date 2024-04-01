#nullable enable
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using NUnit.Framework;
using Socolin.TestUtils.FakeSmtp;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps;

[Binding]
public class MailSteps(ScenarioContext scenarioContext, IMailReceiver mailReceiver)
{
    [Then(@"a mail validation mail has been sent to ""(.*)""")]
    public async Task ThenAMailValidationMailHasBeenSentTo(string email)
    {
        var mail = await GetMailAndWaitIfNotReady(email, 2000);
        if (mail == null)
        {
            var mailsDetails = string.Join("\n", mailReceiver.Mails.Select(m => "'" + string.Join("','", m.To) + "' - " + m.Subject.FirstOrDefault()));
            throw new Exception($"Did not received any mail with destination: {email}. Received {mailReceiver.Mails.Count} mails:\n{mailsDetails}");
        }

        scenarioContext.SetLastReceivedMail(mail);

        var activationCode = ParseActivationCodeFromActivationMail(mail);
        if (activationCode == null)
            Assert.Fail($"Fail to find activation code in mail body:\n'{mail.Data}'");

        scenarioContext.SetActivationCode(activationCode);
    }

    private async Task<FakeSmtpMail?> GetMailAndWaitIfNotReady(string email, int delay)
    {
        var sw = Stopwatch.StartNew();
        FakeSmtpMail? mail;
        do
        {
            mail = mailReceiver.Mails.LastOrDefault(m => m.To.Contains(email));
            if (mail == null)
                await Task.Delay(100);
        } while (mail == null && sw.ElapsedMilliseconds < delay);

        return mail;
    }

    public static string? ParseActivationCodeFromActivationMail(FakeSmtpMail mail)
    {
        var match = Regex.Match(mail.Data, "ActivationCode: (?<activationCode>[a-f0-9]+)");
        if (!match.Success)
            return null;

        var activationCode = match.Groups["activationCode"].Value;
        return activationCode;
    }
}
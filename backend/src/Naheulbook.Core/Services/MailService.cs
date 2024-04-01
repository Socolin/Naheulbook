using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Naheulbook.Core.Configurations;

namespace Naheulbook.Core.Services;

public interface IMailService
{
    Task SendCreateUserMailAsync(string email, string activationCode);
}

public class MailService(IMailConfiguration mailConfiguration) : IMailService
{
    public async Task SendCreateUserMailAsync(string email, string activationCode)
    {
        var client = new SmtpClient(mailConfiguration.Smtp.Host, mailConfiguration.Smtp.Port)
        {
            UseDefaultCredentials = false,
            EnableSsl = mailConfiguration.Smtp.Ssl,
            Credentials = new NetworkCredential(mailConfiguration.Smtp.Username, mailConfiguration.Smtp.Password),
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(mailConfiguration.FromAddress),
            Body = $"ActivationCode: {activationCode}",
            Subject = "Activate naheulbook account",
        };
        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage);
    }
}
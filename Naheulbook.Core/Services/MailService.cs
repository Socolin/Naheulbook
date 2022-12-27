using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Naheulbook.Core.Configurations;

namespace Naheulbook.Core.Services;

public interface IMailService
{
    Task SendCreateUserMailAsync(string email, string activationCode);
}

public class MailService : IMailService
{
    private readonly IMailConfiguration _mailConfiguration;

    public MailService(IMailConfiguration mailConfiguration)
    {
        _mailConfiguration = mailConfiguration;
    }

    public async Task SendCreateUserMailAsync(string email, string activationCode)
    {
        var client = new SmtpClient(_mailConfiguration.Smtp.Host, _mailConfiguration.Smtp.Port)
        {
            UseDefaultCredentials = false,
            EnableSsl = _mailConfiguration.Smtp.Ssl,
            Credentials = new NetworkCredential(_mailConfiguration.Smtp.Username, _mailConfiguration.Smtp.Password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_mailConfiguration.FromAddress),
            Body = $"ActivationCode: {activationCode}",
            Subject = "Activate naheulbook account"
        };
        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage);
    }
}
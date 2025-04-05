using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Naheulbook.Core.Features.Users;

public interface IMailService
{
    Task SendCreateUserMailAsync(string email, string activationCode);
}

public class MailService(IOptions<MailOptions> mailOptions) : IMailService
{
    public async Task SendCreateUserMailAsync(string email, string activationCode)
    {
        var client = new SmtpClient(mailOptions.Value.Smtp.Host, mailOptions.Value.Smtp.Port)
        {
            UseDefaultCredentials = false,
            EnableSsl = mailOptions.Value.Smtp.Ssl,
            Credentials = new NetworkCredential(mailOptions.Value.Smtp.Username, mailOptions.Value.Smtp.Password),
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(mailOptions.Value.FromAddress),
            Body = $"ActivationCode: {activationCode}",
            Subject = "Activate naheulbook account",
        };
        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage);
    }
}
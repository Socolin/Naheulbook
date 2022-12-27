namespace Naheulbook.Core.Configurations;

public interface IMailConfiguration
{
    SmtpConfiguration Smtp { get; }
    string FromAddress { get; }
}

public class MailConfiguration : IMailConfiguration
{
    public SmtpConfiguration Smtp { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
}

public class SmtpConfiguration
{
    public string Password { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool Ssl { get; set; } = true;
}
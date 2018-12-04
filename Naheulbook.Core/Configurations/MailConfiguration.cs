namespace Naheulbook.Core.Configurations
{
    public interface IMailConfiguration
    {
        SmtpConfiguration Smtp { get; }
        string FromAddress { get; }
    }

    public class MailConfiguration : IMailConfiguration
    {
        public SmtpConfiguration Smtp { get; set; }
        public string FromAddress { get; set; }
    }

    public class SmtpConfiguration
    {
        public string Password { get; set; }
        public string Username { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; } = true;
    }
}
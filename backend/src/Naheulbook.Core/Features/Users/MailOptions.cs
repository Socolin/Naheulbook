using System;

namespace Naheulbook.Core.Features.Users;

[Serializable]
public class MailOptions
{
    public SmtpOptions Smtp { get; set; } = null!;
    public string FromAddress { get; set; } = null!;
}

[Serializable]
public class SmtpOptions
{
    public string Password { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool Ssl { get; set; } = true;
}
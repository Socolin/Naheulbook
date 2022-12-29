using CliWrap;
using CliWrap.Buffered;
using Spectre.Console;

namespace Naheulbook.GenerateDevCertificate;

public static class CertificateAuthorityInstaller
{
    public static async Task InstallCertificateAuthorityAsync(string caPath)
    {
        if (OperatingSystem.IsLinux())
        {
            AnsiConsole.WriteLine("Installing Certificate Authority. `root` permissions are required for this");
            var targetCaPath = Path.Combine("/", "usr", "local", "share", "ca-certificates", Path.GetFileName(caPath));
            await Cli.Wrap("sudo")
                .WithArguments(builder =>
                {
                    builder.Add("cp");
                    builder.Add(caPath);
                    builder.Add(targetCaPath);
                })
                .ExecuteBufferedAsync();
            await Cli.Wrap("sudo")
                .WithArguments(builder =>
                {
                    builder.Add("update-ca-certificates");
                    builder.Add("--fresh");
                })
                .ExecuteBufferedAsync();
            await Cli.Wrap("certutil")
                .WithArguments(builder =>
                {
                    builder.Add("-d").Add($"sql:/home/{Environment.UserName}/.pki/nssdb");
                    builder.Add("-A");
                    builder.Add("-t").Add("TC");
                    builder.Add("-n").Add("Naheulbook Dev Certificate Authority");
                    builder.Add("-i").Add(caPath);
                })
                .ExecuteBufferedAsync();
            AnsiConsole.Write(new Rows(new Text("Certificate Authority was successfully installed", new Style(Color.Green))));
        }
        else
        {
            // Windows (powershell):
            // Import-Certificate -FilePath root.crt -CertStoreLocation cert:\LocalMachine\Root
            // And to make it available in IIS:
            // $Pfx = Import-PfxCertificate -FilePath cert.pfx -CertStoreLocation cert:\LocalMachine\My
            // (Get-ChildItem -Path "Cert:\LocalMachine\My\$($Pfx.Thumbprint)").FriendlyName = 'Naheulbook Tls Certificate'
            throw new NotSupportedException("Automatic installation of current os is not supported yet");
        }
    }
    public static async Task<bool> IsCetificateAuthorityInstalledAsync(string certPath)
    {
        if (OperatingSystem.IsLinux())
        {
            var result = await Cli.Wrap("openssl")
                .WithArguments(builder => { builder.Add("verify").Add(certPath); })
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();
            return result.ExitCode == 0;
        }

        throw new NotSupportedException("Automatic installation of current os is not supported yet");
    }
}
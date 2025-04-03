using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
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
        else if (OperatingSystem.IsWindows())
        {
            var proc = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = "pwsh",
            };

            var fileName = Path.GetTempFileName().Replace(".tmp", ".ps1");
            // If we need to make it available in IIS:
            // $Pfx = Import-PfxCertificate -FilePath cert.pfx -CertStoreLocation cert:\LocalMachine\My
            // (Get-ChildItem -Path "Cert:\LocalMachine\My\$($Pfx.Thumbprint)").FriendlyName = 'Naheulbook Tls Certificate'
            await File.WriteAllTextAsync(fileName,
                $"""
                Import-Certificate -FilePath {caPath} -CertStoreLocation cert:\LocalMachine\Root;
                """);

            proc.Arguments = $"-NoProfile -ExecutionPolicy Bypass -File {fileName}";
            proc.Verb = "runas";

            var process = Process.Start(proc);
            if (process == null)
                throw new Exception("Failed to start process to install certificate");
            await process.WaitForExitAsync();
        }
        else
        {
            throw new NotSupportedException("Automatic installation of current os is not supported yet");
        }
    }
    public static async Task<bool> IsCertificateAuthorityInstalledAsync(string certPath, string caPath)
    {
        if (OperatingSystem.IsLinux())
        {
            var result = await Cli.Wrap("openssl")
                .WithArguments(builder => { builder.Add("verify").Add(certPath); })
                .WithValidation(CommandResultValidation.None)
                .ExecuteBufferedAsync();
            return result.ExitCode == 0;
        }

        if (OperatingSystem.IsWindows())
        {
            var caCert = X509CertificateLoader.LoadCertificateFromFile(caPath);
            using var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            foreach (var cert in store.Certificates)
            {
                if (cert.GetSerialNumberString() == caCert.GetSerialNumberString())
                    return true;
            }

            return false;
        }

        throw new NotSupportedException("Automatic installation of current os is not supported yet");
    }
}
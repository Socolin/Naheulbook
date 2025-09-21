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
                    }
                )
                .ExecuteBufferedAsync();
            await Cli.Wrap("sudo")
                .WithArguments(builder =>
                    {
                        builder.Add("update-ca-certificates");
                        builder.Add("--fresh");
                    }
                )
                .ExecuteBufferedAsync();
            await Cli.Wrap("certutil")
                .WithArguments(builder =>
                    {
                        builder.Add("-d").Add($"sql:/home/{Environment.UserName}/.pki/nssdb");
                        builder.Add("-A");
                        builder.Add("-t").Add("TC");
                        builder.Add("-n").Add("Naheulbook Dev Certificate Authority");
                        builder.Add("-i").Add(caPath);
                    }
                )
                .ExecuteBufferedAsync();
            AnsiConsole.Write(new Rows(new Text("Certificate Authority was successfully installed", new Style(Color.Green))));
        }
        else if (OperatingSystem.IsWindows())
        {
            var certPem = await File.ReadAllTextAsync(caPath);
            using var x509Certificate2 = X509Certificate2.CreateFromPem(certPem);
            using var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            var oldCertificates = store.Certificates.Find(X509FindType.FindBySubjectName, x509Certificate2.SubjectName.Name, false);
            foreach (var oldCert in oldCertificates)
                store.Remove(oldCert);
            store.Add(X509CertificateLoader.LoadCertificate(x509Certificate2.RawData));
            AnsiConsole.WriteLine("  Root Certificate installed");
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
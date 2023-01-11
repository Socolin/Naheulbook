using Naheulbook.GenerateDevCertificate;
using Naheulbook.Tools.Shared;
using Spectre.Console;

var certHostnames = new[]
{
    "127.0.0.1",
    "localhost",
    "local.naheulbook.fr",
    "*.local.naheulbook.fr",
    "*.localhost",
};
var outputDirectory = Path.Combine(FileSystemHelper.FindDirectoryContaining("Naheulbook.sln"), "tls");
if (!Directory.Exists(outputDirectory))
    Directory.CreateDirectory(outputDirectory);

var (caPath, certPath, certPrivateKeyPath, certPfxPath) = SelfSignedCertificateAuthorityGenerator.GenerateCertificateAuthorityWithOneCertificate(
    outputDirectory,
    "Naheulbook",
    certHostnames
);

if (!await CertificateAuthorityInstaller.IsCertificateAuthorityInstalledAsync(certPath, caPath))
    if (AnsiConsole.Confirm("Do you want to install the Certificate Authority (will require administrator / root permission) ?"))
        await CertificateAuthorityInstaller.InstallCertificateAuthorityAsync(caPath);

AnsiConsole.WriteLine();
AnsiConsole.MarkupLine($"Certificate Authority: [cyan]{caPath}[/]");
AnsiConsole.MarkupLine($"Certificate: [cyan]{certPath}[/]");
AnsiConsole.MarkupLine($"Certificate private key: [cyan]{certPrivateKeyPath}[/]");
AnsiConsole.MarkupLine($"Pfx (Certificate + Private Key): [cyan]{certPfxPath}[/]");



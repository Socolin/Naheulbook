using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Spectre.Console;

namespace Naheulbook.GenerateDevCertificate;

public static class SelfSignedCertificateAuthorityGenerator
{
    public static (string caPath, string certPath, string privateKeyPath, string certPfxPath) GenerateCertificateAuthorityWithOneCertificate(
        string targetDirectory,
        string name,
        IEnumerable<string> certHostnames
    )
    {
        var caPath = Path.Combine(targetDirectory, name + "RootCa.crt");
        var certPath = Path.Combine(targetDirectory, name + ".crt");
        var certKeyPath = Path.Combine(targetDirectory, name + ".key");
        var certPfxPath = Path.Combine(targetDirectory, name + ".pfx");

        var generateCertificate = true;
        if (File.Exists(caPath))
            generateCertificate = AnsiConsole.Confirm($"A certificate already exists: [green]{caPath}[/]. Do you want to overwrite it ?", false);
        if (generateCertificate)
        {
            AnsiConsole.Status()
                .Start("Generating...",
                    _ =>
                    {
                        var rootCert = GenerateSelfSignedCertificateAuthorityRootCert(name);
                        var (certificate, certificateKey) = GenerateCertificate(name, certHostnames, rootCert);

                        File.WriteAllText(caPath, new string(PemEncoding.Write("CERTIFICATE", rootCert.Export(X509ContentType.Cert))));
                        File.WriteAllText(certPath, new string(PemEncoding.Write("CERTIFICATE", certificate.Export(X509ContentType.Cert))));
                        File.WriteAllText(certKeyPath, new string(PemEncoding.Write("RSA PRIVATE KEY", certificateKey.ExportRSAPrivateKey())));
                        File.WriteAllBytes(certPfxPath, certificate.CopyWithPrivateKey(certificateKey).Export(X509ContentType.Pkcs12));
                    });
        }

        return (caPath, certPath, certKeyPath, certPfxPath);
    }

    private static (X509Certificate2 certificate, RSA certificateKey) GenerateCertificate(string name, IEnumerable<string> certHostnames, X509Certificate2 rootCert)
    {
        var serial = new byte[sizeof(long)];
        RandomNumberGenerator.Fill(serial);

        var certificateKey = RSA.Create(2048);
        var certificateRequest = new CertificateRequest(
            $"CN={name} Dev Certificate",
            certificateKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );

        certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
        certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyEncipherment, false));
        var subjectAlternativeNameBuilder = new SubjectAlternativeNameBuilder();
        foreach (var certHostname in certHostnames)
            subjectAlternativeNameBuilder.AddDnsName(certHostname);
        certificateRequest.CertificateExtensions.Add(subjectAlternativeNameBuilder.Build());

        var certificate = certificateRequest.Create(rootCert, DateTimeOffset.Now.AddDays(-10), DateTimeOffset.Now.AddYears(19), serial);
        return (certificate, certificateKey);
    }


    private static X509Certificate2 GenerateSelfSignedCertificateAuthorityRootCert(string name)
    {
        var rootKey = RSA.Create(4096);
        var certificateRequest = new CertificateRequest(
            $"CN = {name} Dev Certificate Authority",
            rootKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        );
        var subjectKeyIdentifier = SHA1.HashData(certificateRequest.PublicKey.EncodedKeyValue.RawData);

        certificateRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));
        certificateRequest.CertificateExtensions.Add(X509AuthorityKeyIdentifierExtension.CreateFromSubjectKeyIdentifier(subjectKeyIdentifier));
        certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature, true));

        return certificateRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-20), DateTimeOffset.Now.AddYears(20));
    }
}
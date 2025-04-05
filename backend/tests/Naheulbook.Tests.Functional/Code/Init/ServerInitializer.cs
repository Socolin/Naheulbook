using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.HttpClients;
using Naheulbook.Tests.Functional.Code.Servers;
using Reqnroll;
using Reqnroll.BoDi;
using Socolin.TestUtils.FakeSmtp;

namespace Naheulbook.Tests.Functional.Code.Init;

[Binding]
public class ServerInitializer(
    IObjectContainer objectContainer,
    ScenarioContext scenarioContext
)
{
    private static NaheulbookApiServer _naheulbookApiServer;
    private static LaPageAMelkorStub _laPageAMelkorStub;
    private static FakeSmtpServer _fakeSmtpServer;
    private static string _mapImageOutputDirectory;

    [BeforeTestRun]
    public static void InitializeServices()
    {
        using var _ = InitializersProfiler.Profile(nameof(InitializeServices));

        _fakeSmtpServer = new FakeSmtpServer();
        var mailConfig = _fakeSmtpServer.Start();
        _mapImageOutputDirectory = Path.Combine(Path.GetTempPath(), "map-" + Guid.NewGuid());
        Directory.CreateDirectory(_mapImageOutputDirectory);

        _laPageAMelkorStub = new LaPageAMelkorStub();
        _laPageAMelkorStub.Start();
        _naheulbookApiServer = new NaheulbookApiServer(
            mailConfig,
            _laPageAMelkorStub.ListenUrls.First(),
            _mapImageOutputDirectory
        );
        _naheulbookApiServer.Start();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        _laPageAMelkorStub?.Stop();
        _naheulbookApiServer?.Stop();
        _fakeSmtpServer?.Stop();
        _fakeSmtpServer?.Dispose();
        if (_mapImageOutputDirectory != null)
            Directory.Delete(_mapImageOutputDirectory, true);
    }

    [BeforeScenario(Order = 0)]
    public void InitializeIoc()
    {
        objectContainer.RegisterInstanceAs(_fakeSmtpServer, typeof(IMailReceiver));
        objectContainer.RegisterInstanceAs(_naheulbookApiServer, typeof(NaheulbookApiServer));
        objectContainer.RegisterInstanceAs(new NaheulbookHttpClient(_naheulbookApiServer.ListenUrls.First()));
        scenarioContext.SetMapImageOutputDirectory(_mapImageOutputDirectory);
    }
}
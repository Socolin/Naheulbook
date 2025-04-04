using System.IO;
using System.Text;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Naheulbook.DatabaseMigrator.Tools;

public class FluentMigratorFileLoggerProvider(string path, IOptions<FluentMigratorLoggerOptions> options) : ILoggerProvider
{
    private readonly FluentMigratorLoggerOptions _options = options.Value;
    private readonly TextWriter _output = new StreamWriter(path, false, Encoding.UTF8, 1);

    public ILogger CreateLogger(string categoryName)
    {
        return new FluentMigratorRunnerLogger(_output, _output, _options);
    }

    public void Dispose()
    {
        _output.Flush();
        _output.Close();
        _output.Dispose();
    }
}
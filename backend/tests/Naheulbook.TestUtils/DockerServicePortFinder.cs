using CliWrap;
using CliWrap.Buffered;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.TestUtils;

public static class DockerServicePortFinder
{
    public static int? GetPortForServiceInDocker(string containerName, string port)
    {
        var result = Cli.Wrap("docker").WithArguments($"container inspect {containerName}")
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync().Task.Result;

        if (result.ExitCode == 0)
        {
            var containerInfo = JsonConvert.DeserializeObject<List<DockerContainerInformation>>(result.StandardOutput)!.Single();
            return int.Parse(containerInfo.NetworkSettings.Ports[port].First().HostPort);
        }

        return null;
    }

    [PublicAPI]
    private class DockerContainerInformation
    {
        [PublicAPI]
        public class NetworkSettingsObject
        {
            [PublicAPI]
            public class PortObject
            {
                public string HostIp { get; set; } = null!;
                public string HostPort { get; set; } = null!;
            }

            public Dictionary<string, IList<PortObject>> Ports = new();
        }

        public NetworkSettingsObject NetworkSettings { get; set; } = new();
    }
}
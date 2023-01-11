using CliWrap;
using CliWrap.Buffered;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.Tools.Shared;

public class ConfigResolver
{
    async Task<List<string>> ListContainerNames()
    {
        var dockerPsResult = await Cli.Wrap("docker")
            .WithArguments("ps --format '{{.Names}}'")
            .ExecuteBufferedAsync();
        var list = dockerPsResult.StandardOutput.Split("\n").Select(n => n.Trim('\'')).ToList();
        return list;
    }
    public async Task<int> GetMysqlPortAsync()
    {
        var mysqlContainerName = await GetContainerNameEndingWithAsync("-naheulbook_dev_env_mysql-1");
        var dockerInspectMysqlResult = await Cli.Wrap("docker")
            .WithArguments($"inspect {mysqlContainerName}")
            .ExecuteBufferedAsync();
        var mysqlInspect = JsonConvert.DeserializeObject<DockerInspectModel[]>(dockerInspectMysqlResult.StandardOutput);
        return int.Parse(mysqlInspect!.Single().NetworkSettings.Ports["3306/tcp"].First().HostPort);
    }

    public async Task<int> GetRedisPortAsync()
    {
        var redisContainerName = await GetContainerNameEndingWithAsync("-naheulbook_dev_env_redis-1");
        var dockerInspectRedisResult = await Cli.Wrap("docker")
            .WithArguments($"inspect {redisContainerName}")
            .ExecuteBufferedAsync();
        var redisInspect = JsonConvert.DeserializeObject<DockerInspectModel[]>(dockerInspectRedisResult.StandardOutput);
        return int.Parse(redisInspect!.Single().NetworkSettings.Ports["6379/tcp"].First().HostPort);
    }

    private async Task<string> GetContainerNameEndingWithAsync(string suffix)
    {
        var mysqlContainerName = (await ListContainerNames()).FirstOrDefault(x => x.EndsWith(suffix));
        if (mysqlContainerName == null)
            throw new Exception("Failed to find MySQL container. Is docker running ?");

        return mysqlContainerName;
    }


    [PublicAPI]
    public class DockerInspectModel
    {
        [PublicAPI]
        public class NetworkSettingsModel
        {
            [PublicAPI]
            public class PortModel
            {
                public required string HostPort { get; set; }
            }

            public Dictionary<string, PortModel[]> Ports = new();
        }

        public NetworkSettingsModel NetworkSettings { get; set; } = new();
    }
}
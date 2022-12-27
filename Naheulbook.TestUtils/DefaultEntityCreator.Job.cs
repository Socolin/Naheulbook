using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public JobEntity CreateJob(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new JobEntity
        {
            Name = $"some-job-name-{suffix}",
            Flags = @"[{""type"": ""value""}]",
            PlayerDescription = $"some-player-description-{suffix}",
            Information = $"some-information-{suffix}",
            PlayerSummary = $"some-player-summary-{suffix}",
            Data = @"{""forOrigin"": {""all"": {""baseEa"": 20, ""diceEaLevelUp"": 6}}}",
            IsMagic = true,
        };
    }

    public SpecialityEntity CreateSpeciality(JobEntity job, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new SpecialityEntity
        {
            Name = $"some-speciality-name-{suffix}",
            Description = $"some-speciality-description-{suffix}",
            Flags = @"[{""type"": ""value""}]",
            Job = job
        };
    }
}
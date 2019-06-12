using System.Collections.Generic;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Job CreateJob(string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Job
            {
                Name = $"some-job-name-{suffix}",
                Flags = @"[{""type"": ""value""}]",
                PlayerDescription = $"some-player-description-{suffix}",
                Information = $"some-information-{suffix}",
                PlayerSummary = $"some-player-summary-{suffix}",
                DiceEaLevelUp = 20,
                BaseAt = 2,
                BasePrd = 8,
                BaseEa = 10,
                BaseEv = 12,
                MaxLoad = 15,
                IsMagic = true,
                BonusEv = 7,
                MaxArmorPr = 6,
                FactorEv = 0.7f,
            };
        }

        public Speciality CreateSpeciality(Job job, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Speciality
            {
                Name = $"some-speciality-name-{suffix}",
                Description = $"some-speciality-description-{suffix}",
                Flags = @"[{""type"": ""value""}]",
                Job = job
            };
        }
    }
}
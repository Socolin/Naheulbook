using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class JobSkillEntity
    {
        public bool Default { get; set; }

        public Guid JobId { get; set; }
        private JobEntity? _job;
        public JobEntity Job { get => _job.ThrowIfNotLoaded(); set => _job = value; }

        public Guid SkillId { get; set; }
        private SkillEntity? _skill;
        public SkillEntity Skill { get => _skill.ThrowIfNotLoaded(); set => _skill = value; }
    }
}
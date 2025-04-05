namespace Naheulbook.Core.Features.Character.Backup.V1;

public class BackupItemTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? TechName { get; set; }
    public string? SourceUserNameCache { get; set; }
    public string? Data { get; set; }

    // FIXME: Guid
    public int SubCategoryId { get; set; }

    public class BackupItemTemplateModifier
    {
        public int Value { get; set; }
        public string? Special { get; set; }
        public string Type { get; set; } = null!;

        public Guid? RequiredJobId { get; set; }
        public Guid? RequiredOriginId { get; set; }
        public string StatName { get; set; } = null!;
    }

    public class BackupItemTemplateRequirement
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string StatName { get; set; } = null!;
    }

    public class BackupItemTemplateSkillModifier
    {
        public Guid SkillId { get; set; }
        public short Value { get; set; }
    }

    public ICollection<BackupItemTemplateModifier> Modifiers { get; set; } = null!;
    public ICollection<BackupItemTemplateRequirement> Requirements { get; set; } = null!;
    public ICollection<BackupItemTemplateSkillModifier> SkillModifiers { get; set; } = null!;
    public ICollection<string> Slots { get; set; } = null!;
    public ICollection<Guid> Skills { get; set; } = null!;
    public ICollection<Guid> UnSkills { get; set; } = null!;
}
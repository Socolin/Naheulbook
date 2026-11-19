using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class TalentEntity
{
    public Guid Id { get; set; }
    public int DisplayOrder { get; set; }
    public string Name { get; set; } = null!;
    public string Data { get; set; } = null!;

    public Guid TalentGroupId { get; set; }
    private TalentGroupEntity? _talentGroup;
    public TalentGroupEntity TalentGroup { get => _talentGroup.ThrowIfNotLoaded(); set => _talentGroup = value; }
}

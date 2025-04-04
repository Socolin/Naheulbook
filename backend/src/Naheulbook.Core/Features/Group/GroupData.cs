using System;

namespace Naheulbook.Core.Features.Group;

[Serializable]
public class GroupData
{
    public int? Mankdebol { get; set; }
    public int? Debilibeuk { get; set; }
    public NhbkDate? Date { get; set; }
    public bool? InCombat { get; set; }
    public int? CurrentFighterIndex { get; set; }
}
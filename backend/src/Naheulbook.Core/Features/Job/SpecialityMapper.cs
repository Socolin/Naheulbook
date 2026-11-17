using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Job;

public interface ISpecialityMapper
{
    SpecialityResponse ToResponse(SpecialityEntity speciality);
    StatModifierResponse ToResponse(SpecialityModifierEntity specialityModifier);
    SpecialitySpecialResponse ToResponse(SpecialitySpecialEntity specialitySpecial);
}

public class SpecialityMapper : ISpecialityMapper
{
    public SpecialityResponse ToResponse(SpecialityEntity speciality)
    {
        return new SpecialityResponse
        {
            Id = speciality.Id,
            Name = speciality.Name,
            Description = speciality.Description,
            Modifiers = speciality.Modifiers.OrderBy(m => m.Id).Select(ToResponse).ToList(),
            Specials = speciality.Specials.OrderBy(s => s.Id).Select(ToResponse).ToList(),
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(speciality.Flags) ?? [],
        };
    }

    public StatModifierResponse ToResponse(SpecialityModifierEntity specialityModifier)
    {
        return new StatModifierResponse
        {
            Stat = specialityModifier.Stat,
            Type = "ADD",
            Value = specialityModifier.Value,
        };
    }

    public SpecialitySpecialResponse ToResponse(SpecialitySpecialEntity specialitySpecial)
    {
        return new SpecialitySpecialResponse
        {
            Id = specialitySpecial.Id,
            IsBonus = specialitySpecial.IsBonus,
            Description = specialitySpecial.Description,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(specialitySpecial.Flags) ?? [],
        };
    }
}

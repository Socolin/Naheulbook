using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Aptitude;

public interface IAptitudeMapper
{
    AptitudeResponse ToResponse(AptitudeEntity aptitude);
    SummaryAptitudeGroupResponse ToSummaryResponse(AptitudeGroupEntity aptitudeGroup);
    AptitudeGroupResponse ToResponse(AptitudeGroupEntity aptitudeGroup);
    CharacterAptitudeResponse ToResponse(CharacterAptitudeEntity characterAptitude);
    CharacterAddAptitudeResponse ToAddResponse(CharacterAptitudeEntity characterAptitude);
}

public class AptitudeMapper : IAptitudeMapper
{
    public AptitudeResponse ToResponse(AptitudeEntity aptitude)
    {
        return new AptitudeResponse
        {
            Id = aptitude.Id,
            Roll = aptitude.Roll,
            Type = aptitude.Type,
            Name = aptitude.Name,
            Description = aptitude.Description,
            Effect = aptitude.Effect,
        };
    }

    public SummaryAptitudeGroupResponse ToSummaryResponse(AptitudeGroupEntity aptitudeGroup)
    {
        return new SummaryAptitudeGroupResponse
        {
            Id = aptitudeGroup.Id,
            Name = aptitudeGroup.Name,
        };
    }

    public AptitudeGroupResponse ToResponse(AptitudeGroupEntity aptitudeGroup)
    {
        return new AptitudeGroupResponse
        {
            Id = aptitudeGroup.Id,
            Name = aptitudeGroup.Name,
            Aptitudes = aptitudeGroup.Aptitudes.Select(ToResponse).ToList(),
        };
    }

    public CharacterAptitudeResponse ToResponse(CharacterAptitudeEntity characterAptitude)
    {
        return new CharacterAptitudeResponse
        {
            Aptitude = ToResponse(characterAptitude.Aptitude),
            Count = characterAptitude.Count,
            Active = characterAptitude.Active,
        };
    }

    public CharacterAddAptitudeResponse ToAddResponse(CharacterAptitudeEntity characterAptitude)
    {
        return new CharacterAddAptitudeResponse
        {
            Aptitude = ToResponse(characterAptitude.Aptitude),
            Count = characterAptitude.Count,
            Active = characterAptitude.Active,
        };
    }
}

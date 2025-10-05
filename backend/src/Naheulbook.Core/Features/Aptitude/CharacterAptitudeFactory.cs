using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Core.Features.Aptitude;

public interface ICharacterAptitudeFactory
{
    public CharacterAptitudeEntity Create(int characterId, Guid aptitudeId);
}

public class CharacterAptitudeFactory : ICharacterAptitudeFactory
{
    public CharacterAptitudeEntity Create(int characterId, Guid aptitudeId)
    {
        return new CharacterAptitudeEntity
        {
            CharacterId = characterId,
            AptitudeId = aptitudeId,
            Count = 1,
            Active = false,
        };
    }
}
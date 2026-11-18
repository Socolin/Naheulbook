namespace Naheulbook.Core.Features.Aptitude;

[Serializable]
public class AptitudeGroupAlreadyUsedException(int characterId) : Exception
{
    public int CharacterId { get; } = characterId;
}

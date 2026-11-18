using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterSpecialAttackResponse
{
    public SpecialAttackResponse SpecialAttack { get; set; } = null!;
    public CharacterSpecialAttackData Data { get; set; } = null!;
}

[PublicAPI]
public class UseCharacterSpecialAttackResponse : CharacterSpecialAttackResponse
{
    /// <summary>
    /// Experience of the character after the use (a success gives the XP of the special attack)
    /// </summary>
    public int Experience { get; set; }
}

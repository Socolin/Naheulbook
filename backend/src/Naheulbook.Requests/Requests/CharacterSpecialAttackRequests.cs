namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class AddCharacterSpecialAttackRequest
{
    public Guid SpecialAttackId { get; set; }
}

[PublicAPI]
public class UseCharacterSpecialAttackRequest
{
    public bool Success { get; set; }
}

[PublicAPI]
public class UpdateCharacterSpecialAttackRequest
{
    public int? SuccessCount { get; set; }
    public bool ResetCooldowns { get; set; }
}

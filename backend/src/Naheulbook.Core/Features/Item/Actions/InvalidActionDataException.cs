namespace Naheulbook.Core.Features.Item.Actions;

[Serializable]
public class InvalidActionDataException : Exception
{
    public string ActionType { get; }

    public InvalidActionDataException(string actionType)
    {
        ActionType = actionType;
        throw new NotImplementedException();
    }
}
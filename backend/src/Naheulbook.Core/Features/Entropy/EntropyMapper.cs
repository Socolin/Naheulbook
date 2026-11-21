using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Entropy;

public interface IEntropyMapper
{
    EntropyEventResponse ToResponse(IEntropyEventEntity entropyEvent);
}

public class EntropyMapper : IEntropyMapper
{
    public EntropyEventResponse ToResponse(IEntropyEventEntity entropyEvent)
    {
        return new EntropyEventResponse
        {
            Id = entropyEvent.Id,
            DiceRoll = entropyEvent.DiceRoll,
            Name = entropyEvent.Name,
            Effect = entropyEvent.Effect,
        };
    }
}

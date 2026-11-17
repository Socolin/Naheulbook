using Naheulbook.Core.Features.Origin;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterRandomNameService
{
    Task<string> GenerateRandomCharacterNameAsync(Guid originId, string sex, CancellationToken cancellationToken = default);
}

public class CharacterRandomNameService(
    IUnitOfWorkFactory unitOfWorkFactory,
    ILaPageAMelkorClient httpClient
) : ICharacterRandomNameService
{
    public async Task<string> GenerateRandomCharacterNameAsync(Guid originId, string sex, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var origin = await uow.Origins.GetAsync(originId, cancellationToken);
        if (origin == null)
            throw new OriginNotFoundException(originId);

        var originRandomNameUrl = await uow.OriginRandomNameUrls.GetByOriginIdAndSexAsync(sex, originId, cancellationToken);
        if (originRandomNameUrl == null)
            throw new RandomNameGeneratorNotFoundException(sex, originId);

        var result = await httpClient.GetRandomNameAsync(originRandomNameUrl.Url, cancellationToken);

        return result.First();
    }
}
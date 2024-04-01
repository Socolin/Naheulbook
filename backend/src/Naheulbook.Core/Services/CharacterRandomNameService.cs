using System;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Clients;
using Naheulbook.Core.Exceptions;
using Naheulbook.Data.Factories;

namespace Naheulbook.Core.Services;

public interface ICharacterRandomNameService
{
    Task<string> GenerateRandomCharacterNameAsync(Guid originId, string sex);
}

public class CharacterRandomNameService(
    IUnitOfWorkFactory unitOfWorkFactory,
    ILaPageAMelkorClient httpClient
) : ICharacterRandomNameService
{
    public async Task<string> GenerateRandomCharacterNameAsync(Guid originId, string sex)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var origin = await uow.Origins.GetAsync(originId);
            if (origin == null)
                throw new OriginNotFoundException(originId);

            var originRandomNameUrl = await uow.OriginRandomNameUrls.GetByOriginIdAndSexAsync(sex, originId);
            if (originRandomNameUrl == null)
                throw new RandomNameGeneratorNotFound(sex, originId);

            var result = await httpClient.GetRandomNameAsync(originRandomNameUrl.Url);

            return result.First();
        }
    }
}
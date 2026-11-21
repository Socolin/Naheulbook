using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Spell;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/spellBooks")]
[ApiController]
public class SpellBookController(
    ISpellService spellService,
    ISpellMapper spellMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SpellBooksResponse>> GetSpellBooksAsync(
        CancellationToken cancellationToken
    )
    {
        var spellBooks = await spellService.GetSpellBooksAsync(cancellationToken);
        return new SpellBooksResponse
        {
            SpellBooks = spellBooks.Select(spellMapper.ToResponse).ToList(),
        };
    }
}

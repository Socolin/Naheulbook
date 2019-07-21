using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Naheulbook.Tests.Functional.Code.Stubs.Melkor.Controllers
{
    [ApiController]
    public class NameGeneratorController : ControllerBase
    {
        [HttpGet("generateurs/noms/{Type}/{Sex}/{Origin}")]
        public ActionResult<List<string>> GenerateName(
            [FromRoute] string type,
            [FromRoute] string sex,
            [FromRoute] string origin
        )
        {
            return new List<string>
            {
                "some-random-name-1",
                "some-random-name-2"
            };
        }
    }
}
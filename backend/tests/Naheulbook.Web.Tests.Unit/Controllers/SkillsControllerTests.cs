using AwesomeAssertions;
using NSubstitute;
using NUnit.Framework;
using Naheulbook.Core.Features.Skill;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Controllers;

namespace Naheulbook.Web.Tests.Unit.Controllers;

public class SkillsControllerTests
{
    private ISkillService _skillService;
    private ISkillMapper _skillMapper;
    private SkillsController _skillsController;

    [SetUp]
    public void SetUp()
    {
        _skillService = Substitute.For<ISkillService>();
        _skillMapper = Substitute.For<ISkillMapper>();
        _skillsController = new SkillsController(_skillService, _skillMapper);
    }

    [Test]
    public async Task CanGetSkills()
    {
        var entity1 = new SkillEntity();
        var entity2 = new SkillEntity();
        var skills = new List<SkillEntity> {entity1, entity2};
        var response1 = new SkillResponse();
        var response2 = new SkillResponse();

        _skillService.GetSkillsAsync()
            .Returns(skills);
        _skillMapper.ToResponse(entity1)
            .Returns(response1);
        _skillMapper.ToResponse(entity2)
            .Returns(response2);

        var result = await _skillsController.GetAsync();

        result.Value.Should().Equal(response1, response2);
    }
}
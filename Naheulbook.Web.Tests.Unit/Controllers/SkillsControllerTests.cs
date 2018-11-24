using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class SkillsControllerTests
    {
        private ISkillService _skillService;
        private IMapper _mapper;
        private SkillsController _skillsController;

        [SetUp]
        public void SetUp()
        {
            _skillService = Substitute.For<ISkillService>();
            _mapper = Substitute.For<IMapper>();
            _skillsController = new SkillsController(_skillService, _mapper);
        }

        [Test]
        public async Task CanGetSkills()
        {
            var skills = new List<Skill>();
            var expectedResponse = new List<SkillResponse>();

            _skillService.GetSkillsAsync()
                .Returns(skills);
            _mapper.Map<List<SkillResponse>>(skills)
                .Returns(expectedResponse);

            var result = await _skillsController.GetAsync();

            result.Value.Should().BeSameAs(expectedResponse);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
    public class LocationRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
    {
        private LocationRepository _locationRepository;

        [SetUp]
        public void SetUp()
        {
            _locationRepository = new LocationRepository(RepositoryDbContext);
        }

        [Test]
        public async Task GetByIdsAsync_LoadAllEntitiesMatchingGivenIds()
        {
            TestDataUtil
                .AddLocation()
                .AddLocation()
                .AddLocation();

            var locationIds = TestDataUtil.GetAll<Location>().Select(x => x.Id);

            var actual = await _locationRepository.GetByIdsAsync(locationIds);

            actual.Should().BeEquivalentTo(TestDataUtil.GetAll<Location>());
        }
    }
}
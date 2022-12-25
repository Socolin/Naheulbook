using System;
using System.Collections.Generic;
using FluentAssertions;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils
{
    public class ItemTemplateUtilTests
    {
        private ItemTemplateUtil _util;
        private IStringCleanupUtil _stringCleanupUtil;
        private IJsonUtil _jsonUtil;

        [SetUp]
        public void SetUp()
        {
            _stringCleanupUtil = Substitute.For<IStringCleanupUtil>();
            _jsonUtil = Substitute.For<IJsonUtil>();

            _util = new ItemTemplateUtil(
                _stringCleanupUtil,
                _jsonUtil
            );
        }

        [Test]
        public void ApplyChangesFromRequest_ChangeBasicInfo()
        {
            var itemTemplate = new ItemTemplateEntity
            {
                Name = "some-name",
                Data = @"{""key"": ""value""}",
                Source = "some-source",
                TechName = "some-tech-name",
                SubCategoryId = 1
            };

            var request = new ItemTemplateRequest
            {
                Name = "some-new-name",
                Data = JObject.FromObject(new {newKey = "newValue"}),
                Source = "some-new-source",
                TechName = "some-new-tech-name",
                SubCategoryId = 2
            };

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.Name.Should().BeEquivalentTo("some-new-name");
            itemTemplate.Data.Should().Be(@"{""newKey"":""newValue""}");
            itemTemplate.Source.Should().Be("some-new-source");
            itemTemplate.TechName.Should().Be("some-new-tech-name");
            itemTemplate.SubCategoryId.Should().Be(2);
        }

        [Test]
        public void ApplyChangesFromRequest_UpdateCleanName()
        {
            var itemTemplate = new ItemTemplateEntity {Name = "some-name"};
            var request = new ItemTemplateRequest {Name = "some-new-name"};

            _stringCleanupUtil.RemoveAccents("some-new-name")
                .Returns("some-new-clean-name");

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.CleanName.Should().BeEquivalentTo("some-new-clean-name");
        }

        [Test]
        public void ApplyChangesFromRequest_UpdateSlotList()
        {
            var slot1 = CreateSlot(1);
            var slot2 = CreateSlot(1);

            var itemTemplate = new ItemTemplateEntity
            {
                Slots = new List<ItemTemplateSlotEntity>
                {
                    new ItemTemplateSlotEntity
                    {
                        SlotId = 1,
                        Slot = slot1
                    },
                    new ItemTemplateSlotEntity
                    {
                        SlotId = 2,
                        Slot = slot2
                    }
                }
            };

            var request = new ItemTemplateRequest
            {
                Slots = new List<IdRequest>
                {
                    new IdRequest {Id = 2},
                    new IdRequest {Id = 3},
                }
            };

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.Slots.Should().BeEquivalentTo(new [] {
                new ItemTemplateSlotEntity
                {
                    SlotId = 2
                },
                new ItemTemplateSlotEntity
                {
                    SlotId = 3
                }
            });
        }

        [Test]
        public void ApplyChangesFromRequest_UpdateRequirements()
        {
            var itemTemplate = new ItemTemplateEntity
            {
                Requirements = new List<ItemTemplateRequirementEntity>
                {
                    new ItemTemplateRequirementEntity
                    {
                        Id = 1,
                        StatName = "some-stat-name-1",
                        MinValue = 2,
                        MaxValue = 3
                    },
                    new ItemTemplateRequirementEntity
                    {
                        Id = 2,
                        StatName = "some-stat-name-2",
                        MinValue = 4,
                        MaxValue = 7
                    }
                }
            };

            var request = new ItemTemplateRequest
            {
                Requirements = new List<ItemTemplateRequirementRequest>
                {
                    new ItemTemplateRequirementRequest
                    {
                        Stat = "some-stat-name-2",
                        Min = 3,
                        Max = 8
                    },
                    new ItemTemplateRequirementRequest
                    {
                        Stat = "some-stat-name-3",
                        Min = 4,
                        Max = 12
                    }
                }
            };

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.Requirements.Should().BeEquivalentTo(new [] {
                new ItemTemplateRequirementEntity
                {
                    StatName = "some-stat-name-2",
                    MinValue = 3,
                    MaxValue = 8
                },
                new ItemTemplateRequirementEntity
                {
                    StatName = "some-stat-name-3",
                    MinValue = 4,
                    MaxValue = 12
                }
            });
        }


        [Test]
        public void ApplyChangesFromRequest_UpdateModiifers()
        {
            var itemTemplate = new ItemTemplateEntity
            {
                Modifiers = new List<ItemTemplateModifierEntity>
                {
                    new ItemTemplateModifierEntity
                    {
                        Id = 1,
                        StatName = "some-stat-name-1",
                        Value = 2,
                        Special = "some-special-1"
                    },
                    new ItemTemplateModifierEntity
                    {
                        Id = 2,
                        StatName = "some-stat-name-2",
                        Value = 3,
                        Special = "some-special-2",
                    }
                }
            };

            var request = new ItemTemplateRequest
            {
                Modifiers = new List<ItemTemplateModifierRequest>
                {
                    new ItemTemplateModifierRequest
                    {
                        Stat = "some-stat-name-2",
                        JobId = new Guid("00000000-0000-0000-0000-000000000011"),
                        Type = "ADD",
                        Value = 3
                    },
                    new ItemTemplateModifierRequest
                    {
                        Stat = "some-stat-name-3",
                        OriginId = new Guid("00000000-0000-0000-0000-000000000001"),
                        Type = "ADD",
                        Value = 3,
                        Special = new List<string> {"some-special-3"}
                    }
                }
            };

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.Modifiers.Should().BeEquivalentTo(new [] {
                new ItemTemplateModifierEntity
                {
                    StatName = "some-stat-name-2",
                    RequiredJobId = new Guid("00000000-0000-0000-0000-000000000011"),
                    Type = "ADD",
                    Value = 3,
                    Special = ""
                },
                new ItemTemplateModifierEntity
                {
                    StatName = "some-stat-name-3",
                    RequiredOriginId = new Guid("00000000-0000-0000-0000-000000000001"),
                    Type = "ADD",
                    Value = 3,
                    Special = "some-special-3"
                }
            });
        }

        [Test]
        public void ApplyChangesFromRequest_UpdateSkills()
        {
            var itemTemplate = new ItemTemplateEntity
            {
                Skills = new List<ItemTemplateSkillEntity>
                {
                    new ItemTemplateSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000001")},
                    new ItemTemplateSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000002")}
                },
                UnSkills = new List<ItemTemplateUnSkillEntity>
                {
                    new ItemTemplateUnSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000003")},
                    new ItemTemplateUnSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000004")}
                }
            };

            var request = new ItemTemplateRequest
            {
                SkillIds = new List<Guid>
                {
                    new Guid("00000000-0000-0000-0000-000000000004"),
                    new Guid("00000000-0000-0000-0000-000000000005")
                },
                UnSkillIds = new List<Guid>
                {
                    new Guid("00000000-0000-0000-0000-000000000006"),
                    new Guid("00000000-0000-0000-0000-000000000007")
                }
            };

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.Skills.Should().BeEquivalentTo(new [] {
                new ItemTemplateSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000004")},
                new ItemTemplateSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000005")}
            });
            itemTemplate.UnSkills.Should().BeEquivalentTo(new [] {
                new ItemTemplateSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000006")},
                new ItemTemplateSkillEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000007")}
            });
        }


        [Test]
        public void ApplyChangesFromRequest_UpdateSkillModifiers()
        {
            var itemTemplate = new ItemTemplateEntity
            {
                SkillModifiers = new List<ItemTemplateSkillModifierEntity>
                {
                    new ItemTemplateSkillModifierEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000001"), Value = 1},
                    new ItemTemplateSkillModifierEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000002"), Value = 2}
                }
            };

            var request = new ItemTemplateRequest
            {
                SkillModifiers = new List<ItemTemplateSkillModifierRequest>
                {
                    new ItemTemplateSkillModifierRequest {SkillId = new Guid("00000000-0000-0000-0000-000000000004"), Value = 3},
                    new ItemTemplateSkillModifierRequest {SkillId = new Guid("00000000-0000-0000-0000-000000000005"), Value = 4},
                }
            };

            _util.ApplyChangesFromRequest(itemTemplate, request);

            itemTemplate.SkillModifiers.Should().BeEquivalentTo(new [] {
                new ItemTemplateSkillModifierEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000004"), Value = 3},
                new ItemTemplateSkillModifierEntity {SkillId = new Guid("00000000-0000-0000-0000-000000000005"), Value = 4}
            });
        }

        [Test]
        public void FilterItemTemplatesBySource_ShouldKeepItemTemplateWithSourceOfficial()
        {
            var itemTemplate = CreateItemTemplateSource("official");
            var iteTemplates = new List<ItemTemplateEntity>
            {
                itemTemplate
            };

            var actualItemTemplates = _util.FilterItemTemplatesBySource(iteTemplates, null, false);

            actualItemTemplates.Should().Contain(itemTemplate);
        }

        [Test]
        public void FilterItemTemplatesBySource_ShouldKeepItemTemplateWithSourceCommunity_WhenIncludeCommunityItem()
        {
            var itemTemplate = CreateItemTemplateSource("community");
            var iteTemplates = new List<ItemTemplateEntity>
            {
                itemTemplate
            };

            var actualItemTemplates = _util.FilterItemTemplatesBySource(iteTemplates, null, true);

            actualItemTemplates.Should().Contain(itemTemplate);
        }

        [Test]
        public void FilterItemTemplatesBySource_ShouldFilterOutItemTemplateWithSourceCommunity_WhenDoNotIncludeSource()
        {
            var itemTemplate = CreateItemTemplateSource("community");
            var iteTemplates = new List<ItemTemplateEntity>
            {
                itemTemplate
            };

            var actualItemTemplates = _util.FilterItemTemplatesBySource(iteTemplates, null, false);

            actualItemTemplates.Should().BeEmpty();
        }

        [Test]
        public void FilterItemTemplatesBySource_ShouldFilterOutPrivateItemTemplateThatDoesNotBelongToTheCurrentUser()
        {
            var itemTemplate = CreateItemTemplateSource("private", 42);
            var iteTemplates = new List<ItemTemplateEntity>
            {
                itemTemplate
            };

            var actualItemTemplates = _util.FilterItemTemplatesBySource(iteTemplates, 12, false);

            actualItemTemplates.Should().BeEmpty();
        }

        [Test]
        public void FilterItemTemplatesBySource_ShouldKeepPrivateItemTemplate_ThatBelongToCurrentUser()
        {
            var itemTemplate = CreateItemTemplateSource("private", 42);
            var iteTemplates = new List<ItemTemplateEntity>
            {
                itemTemplate
            };

            var actualItemTemplates = _util.FilterItemTemplatesBySource(iteTemplates, 42, false);

            actualItemTemplates.Should().Contain(itemTemplate);
        }

        private ItemTemplateEntity CreateItemTemplateSource(string source, int? sourceUserId = null)
        {
            return new ItemTemplateEntity
            {
                Source = source,
                SourceUserId = sourceUserId
            };
        }

        private static SlotEntity CreateSlot(int idx)
        {
            return new SlotEntity()
            {
                Name = $"some-slot-name-{idx}"
            };
        }
    }
}
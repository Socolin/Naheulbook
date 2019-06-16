using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddCharacter(int ownerId, Action<Character> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateCharacter(ownerId, GetLast<Origin>()), customizer);
        }

        public TestDataUtil AddCharacterWithAllData(int ownerId, Action<Character> customizer = null)
        {
            AddStat().AddStat().AddStat().AddStat();
            AddSkill().AddSkill();
            AddOrigin();
            AddJob().GetLast<Job>();
            AddJob().GetLast<Job>();
            AddLocation().AddGroup(ownerId);

            AddSpeciality();

            var character = _defaultEntityCreator.CreateCharacter(ownerId, GetLast<Origin>());

            character.Jobs = new List<CharacterJob>
            {
                new CharacterJob {Job = GetFromEnd<Job>(0)},
                new CharacterJob {Job = GetFromEnd<Job>(1)},
            };

            var characterModifier1 = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValue>
            {
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<Stat>(0), 1),
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<Stat>(1), 2),
            });
            characterModifier1.DurationType = "combat";
            characterModifier1.CombatCount = 2;
            characterModifier1.CurrentCombatCount = 1;
            var characterModifier2 = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValue>
            {
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<Stat>(2), 4),
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<Stat>(3), 6),
            });
            var characterModifier3 = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValue>());
            characterModifier3.DurationType = "lap";
            characterModifier3.LapCount = 4;
            characterModifier3.CurrentLapCount = 2;
            characterModifier3.LapCountDecrement = @"{""when"":""BEFORE"", ""fighterId"": 1, ""fighterIsMonster"": true}";

            character.Modifiers = new List<CharacterModifier>
            {
                characterModifier1,
                characterModifier2,
                characterModifier3,
            };

            character.Specialities = new List<CharacterSpeciality>
            {
                new CharacterSpeciality {Speciality = GetLast<Speciality>()}
            };

            character.Skills = new List<CharacterSkill>
            {
                new CharacterSkill {Skill = GetFromEnd<Skill>(0)},
                new CharacterSkill {Skill = GetFromEnd<Skill>(1)}
            };

            character.Group = GetLast<Group>();

            return SaveEntity(character, customizer);
        }
    }
}
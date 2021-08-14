using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Factories
{
    public interface ICharacterFactory
    {
        Character CreateCharacter(CreateCharacterRequest characterRequest);
        Character CreateCustomCharacter(CreateCustomCharacterRequest request);
    }

    public class CharacterFactory : ICharacterFactory
    {
        public Character CreateCharacter(CreateCharacterRequest characterRequest)
        {
            var character = new Character
            {
                Name = characterRequest.Name,
                Sex = characterRequest.Sex,
                IsActive = true,
                IsNpc = characterRequest.IsNpc,

                Ad = characterRequest.Stats.Ad,
                Cha = characterRequest.Stats.Cha,
                Cou = characterRequest.Stats.Cou,
                Fo = characterRequest.Stats.Fo,
                Int = characterRequest.Stats.Int,

                Notes = characterRequest.Notes,

                Level = 1,
                Experience = 0,

                FatePoint = characterRequest.FatePoint,

                OriginId = characterRequest.OriginId,

                Modifiers = new List<CharacterModifier>(),
            };

            if (characterRequest.JobId.HasValue)
                character.Jobs = new List<CharacterJob>
                {
                    new CharacterJob
                    {
                        JobId = characterRequest.JobId.Value
                    }
                };

            character.Skills = characterRequest.SkillIds
                .Select(x => new CharacterSkill
                {
                    SkillId = x
                })
                .ToList();

            foreach (var modifiedStat in characterRequest.ModifiedStat.Values)
            {
                character.Modifiers.Add(new CharacterModifier
                {
                    Name = modifiedStat.Name,
                    IsActive = true,
                    Permanent = true,
                    DurationType = "forever",
                    Values = modifiedStat.Stats.Select(s => new CharacterModifierValue
                    {
                        StatName = s.Key,
                        Value = (short) s.Value,
                        Type = "ADD",
                    }).ToList()
                });
            }

            if (characterRequest.SpecialityId.HasValue)
                character.Specialities = new List<CharacterSpeciality>
                {
                    new CharacterSpeciality
                    {
                        SpecialityId = characterRequest.SpecialityId.Value
                    }
                };

            return character;
        }

        public Character CreateCustomCharacter(CreateCustomCharacterRequest characterRequest)
        {
            var character = new Character
            {
                Name = characterRequest.Name,
                Sex = characterRequest.Sex,
                IsActive = true,
                IsNpc = characterRequest.IsNpc,

                Ad = characterRequest.Stats.Ad,
                Cha = characterRequest.Stats.Cha,
                Cou = characterRequest.Stats.Cou,
                Fo = characterRequest.Stats.Fo,
                Int = characterRequest.Stats.Int,

                Ev = characterRequest.BasicStatsOverrides.Ev,
                Ea = characterRequest.BasicStatsOverrides.Ea,
                // FIXME: maxEv, maxEa
                // FIXME: At/Prd

                Level = characterRequest.Level,
                Experience = characterRequest.Experience,

                FatePoint = characterRequest.FatePoint,

                OriginId = characterRequest.OriginId,
            };

            character.Jobs = characterRequest.JobIds.Select(jobId => new CharacterJob
            {
                JobId = jobId
            }).ToList();

            character.Skills = characterRequest.SkillIds.Select(x => new CharacterSkill
            {
                SkillId = x
            }).ToList();
            character.Specialities = characterRequest.SpecialityIds.SelectMany(x => x.Value).Select(specialityId => new CharacterSpeciality
            {
                SpecialityId = specialityId
            }).ToList();

            return character;
        }
    }
}
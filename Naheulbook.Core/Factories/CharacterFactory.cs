using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Factories;

public interface ICharacterFactory
{
    CharacterEntity CreateCharacter(CreateCharacterRequest characterRequest);
    CharacterEntity CreateCustomCharacter(CreateCustomCharacterRequest request);
}

public class CharacterFactory : ICharacterFactory
{
    public CharacterEntity CreateCharacter(CreateCharacterRequest characterRequest)
    {
        var character = new CharacterEntity
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

            Modifiers = new List<CharacterModifierEntity>(),
        };

        if (characterRequest.JobId.HasValue)
            character.Jobs = new List<CharacterJobEntity>
            {
                new CharacterJobEntity
                {
                    JobId = characterRequest.JobId.Value
                }
            };

        character.Skills = characterRequest.SkillIds
            .Select(x => new CharacterSkillEntity
            {
                SkillId = x
            })
            .ToList();

        foreach (var modifiedStat in characterRequest.ModifiedStat.Values)
        {
            character.Modifiers.Add(new CharacterModifierEntity
            {
                Name = modifiedStat.Name,
                IsActive = true,
                Permanent = true,
                DurationType = "forever",
                Values = modifiedStat.Stats.Select(s => new CharacterModifierValueEntity
                {
                    StatName = s.Key,
                    Value = (short) s.Value,
                    Type = "ADD",
                }).ToList(),
            });
        }

        if (characterRequest.SpecialityId.HasValue)
            character.Specialities = new List<CharacterSpecialityEntity>
            {
                new CharacterSpecialityEntity
                {
                    SpecialityId = characterRequest.SpecialityId.Value
                }
            };

        return character;
    }

    public CharacterEntity CreateCustomCharacter(CreateCustomCharacterRequest characterRequest)
    {
        var character = new CharacterEntity
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

        character.Jobs = characterRequest.JobIds.Select(jobId => new CharacterJobEntity
        {
            JobId = jobId
        }).ToList();

        character.Skills = characterRequest.SkillIds.Select(x => new CharacterSkillEntity
        {
            SkillId = x
        }).ToList();
        character.Specialities = characterRequest.SpecialityIds.SelectMany(x => x.Value).Select(specialityId => new CharacterSpecialityEntity
        {
            SpecialityId = specialityId
        }).ToList();

        return character;
    }
}
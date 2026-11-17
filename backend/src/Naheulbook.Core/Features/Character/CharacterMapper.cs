using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Job;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterMapper
{
    NamedIdResponse ToNamedIdResponse(CharacterEntity character);
    CreateCharacterResponse ToCreateResponse(CharacterEntity character);
    ListActiveCharacterResponse ToListActiveResponse(CharacterEntity character);
    CharacterSearchResponse ToSearchResponse(CharacterEntity character);
    CharacterSummaryResponse ToSummaryResponse(CharacterEntity character);
    CharacterResponse ToResponse(CharacterEntity character);
    CharacterFoGmResponse ToGmResponse(CharacterEntity character);
    CharacterLevelUpResponse ToResponse(LevelUpResult levelUpResult);
    IHistoryEntryResponse ToResponse(IHistoryEntry historyEntry);
    CharacterHistoryEntryResponse ToResponse(CharacterHistoryEntryEntity historyEntry);
}

public class CharacterMapper(
    IAptitudeMapper aptitudeMapper,
    ICharacterModifierMapper characterModifierMapper,
    IGroupMapper groupMapper,
    IItemMapper itemMapper,
    ISpecialityMapper specialityMapper
) : ICharacterMapper
{
    public NamedIdResponse ToNamedIdResponse(CharacterEntity character)
    {
        return new NamedIdResponse
        {
            Id = character.Id,
            Name = character.Name,
        };
    }

    public CreateCharacterResponse ToCreateResponse(CharacterEntity character)
    {
        return new CreateCharacterResponse
        {
            Id = character.Id,
        };
    }

    public ListActiveCharacterResponse ToListActiveResponse(CharacterEntity character)
    {
        return new ListActiveCharacterResponse
        {
            Id = character.Id,
            IsNpc = character.IsNpc,
            Name = character.Name,
        };
    }

    public CharacterSearchResponse ToSearchResponse(CharacterEntity character)
    {
        return new CharacterSearchResponse
        {
            Id = character.Id,
            Name = character.Name,
            OriginName = character.Origin.Name,
            OwnerName = character.Owner.DisplayName!,
        };
    }

    public CharacterSummaryResponse ToSummaryResponse(CharacterEntity character)
    {
        return new CharacterSummaryResponse
        {
            Id = character.Id,
            Name = character.Name,
            Level = character.Level,
            OriginName = character.Origin.Name,
            JobNames = character.Jobs.Select(j => j.Job.Name).ToList(),
        };
    }

    public CharacterResponse ToResponse(CharacterEntity character)
    {
        return FillCharacterResponse(new CharacterResponse(), character);
    }

    public CharacterFoGmResponse ToGmResponse(CharacterEntity character)
    {
        var response = FillCharacterResponse(new CharacterFoGmResponse(), character);
        response.IsActive = character.IsActive;
        response.Color = character.Color;
        response.GmData = MapperHelpers.FromJson<CharacterGmData>(character.GmData) ?? new CharacterGmData();
        response.OwnerId = character.OwnerId;
        response.Target = ToTargetResponse(character);
        return response;
    }

    public CharacterLevelUpResponse ToResponse(LevelUpResult levelUpResult)
    {
        return new CharacterLevelUpResponse
        {
            NewModifiers = levelUpResult.NewModifiers.Select(characterModifierMapper.ToResponse).ToList(),
            NewSkillIds = levelUpResult.NewSkills.Select(s => s.SkillId).ToList(),
            NewLevel = levelUpResult.NewLevel,
            NewSpecialities = levelUpResult.NewSpecialities.Select(s => specialityMapper.ToResponse(s.Speciality)).ToList(),
        };
    }

    public IHistoryEntryResponse ToResponse(IHistoryEntry historyEntry)
    {
        return historyEntry switch
        {
            CharacterHistoryEntryEntity characterHistoryEntry => ToResponse(characterHistoryEntry),
            GroupHistoryEntryEntity groupHistoryEntry => groupMapper.ToResponse(groupHistoryEntry),
            _ => throw new NotSupportedException($"Unsupported history entry type `{historyEntry.GetType().Name}`"),
        };
    }

    public CharacterHistoryEntryResponse ToResponse(CharacterHistoryEntryEntity historyEntry)
    {
        return new CharacterHistoryEntryResponse
        {
            Id = historyEntry.Id,
            Action = historyEntry.Action,
            Data = MapperHelpers.FromJson<JObject>(historyEntry.Data) ?? new JObject(),
            Date = historyEntry.Date.ToString("s"),
            Gm = historyEntry.Gm,
            Info = historyEntry.Info,
            Item = historyEntry.Item is { } item
                ? new CharacterHistoryEntryResponse.ItemHistoryResponse {Name = MapperHelpers.ItemNameFromData(item.Data)}
                : null,
            Modifier = historyEntry.CharacterModifier is { } modifier
                ? new CharacterHistoryEntryResponse.ModifierHistoryResponse {Name = modifier.Name}
                : null,
            Effect = historyEntry.Effect is { } effect
                ? new CharacterHistoryEntryResponse.EffectHistoryResponse {Name = effect.Name}
                : null,
        };
    }

    private T FillCharacterResponse<T>(T response, CharacterEntity character)
        where T : CharacterResponse
    {
        response.Id = character.Id;
        response.Name = character.Name;
        response.Sex = character.Sex;
        response.OriginId = character.OriginId;
        response.IsNpc = character.IsNpc;
        response.Ev = character.Ev;
        response.Ea = character.Ea;
        response.Notes = character.Notes;
        response.Level = character.Level;
        response.Experience = character.Experience;
        response.FatePoint = character.FatePoint;
        response.Stats = new CharacterResponse.BasicStats
        {
            Ad = character.Ad,
            Cou = character.Cou,
            Cha = character.Cha,
            Fo = character.Fo,
            Int = character.Int,
        };
        response.StatBonusAd = character.StatBonusAd;
        response.JobIds = character.Jobs.Select(j => j.JobId).ToList();
        response.SkillIds = character.Skills.Select(s => s.SkillId).ToList();
        response.Group = character.Group is { } group ? groupMapper.ToCharacterGroupResponse(group) : null;
        response.Aptitudes = character.Aptitudes.Select(aptitudeMapper.ToResponse).ToList();
        response.Modifiers = character.Modifiers.Select(characterModifierMapper.ToResponse).ToList();
        response.Specialities = character.Specialities.Select(s => specialityMapper.ToResponse(s.Speciality)).ToList();
        response.Items = character.Items.Select(itemMapper.ToResponse).ToList();
        response.Invites = character.Invites.Select(groupMapper.ToCharacterInviteResponse).ToList();
        return response;
    }

    private static TargetResponse? ToTargetResponse(CharacterEntity character)
    {
        if (character.TargetedCharacterId.HasValue)
            return new TargetResponse {Id = character.TargetedCharacterId.Value, IsMonster = false};
        if (character.TargetedMonsterId.HasValue)
            return new TargetResponse {Id = character.TargetedMonsterId.Value, IsMonster = true};
        return null;
    }
}

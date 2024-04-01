using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Responses;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<CalendarEntity, CalendarResponse>();

        CreateMap<CharacterEntity, NamedIdResponse>();
        CreateMap<CharacterEntity, CreateCharacterResponse>();
        CreateMap<CharacterEntity, ListActiveCharacterResponse>();
        CreateMap<CharacterEntity, CharacterSearchResponse>()
            .ForMember(x => x.OriginName, opt => opt.MapFrom(c => c.Origin.Name))
            .ForMember(x => x.OwnerName, opt => opt.MapFrom(c => c.Owner.DisplayName));
        CreateMap<CharacterEntity, CharacterSummaryResponse>()
            .ForMember(x => x.JobNames, opt => opt.MapFrom(c => c.Jobs.Select(x => x.Job.Name)))
            .ForMember(x => x.OriginName, opt => opt.MapFrom(c => c.Origin.Name));
        CreateMap<CharacterEntity, CharacterFoGmResponse>()
            .IncludeBase<CharacterEntity, CharacterResponse>()
            .ForMember(x => x.GmData, opt => opt.MapFrom(c => MapperHelpers.FromJson<CharacterGmData>(c.GmData) ?? new CharacterGmData()))
            .ForMember(x => x.Target, opt => opt.MapFrom(c => c.TargetedCharacterId.HasValue ? new TargetResponse {Id = c.TargetedCharacterId.Value, IsMonster = false} : (c.TargetedMonsterId.HasValue ? new TargetResponse {Id = c.TargetedMonsterId.Value, IsMonster = true} : null)));
        CreateMap<CharacterEntity, CharacterResponse>()
            .ForMember(x => x.Stats, opt => opt.MapFrom(c => new CharacterResponse.BasicStats {Ad = c.Ad, Cou = c.Cou, Cha = c.Cha, Fo = c.Fo, Int = c.Int}))
            .ForMember(x => x.SkillIds, opt => opt.MapFrom(c => c.Skills.Select(x => x.SkillId)))
            .ForMember(x => x.JobIds, opt => opt.MapFrom(c => c.Jobs.Select(x => x.JobId)))
            .ForMember(x => x.Specialities, opt => opt.MapFrom(c => c.Specialities.Select(x => x.Speciality)));
        CreateMap<CharacterModifierEntity, ActiveStatsModifier>()
            .ForMember(x => x.Active, opt => opt.MapFrom(x => x.IsActive))
            .ForMember(x => x.Values, opt => opt.MapFrom(x => x.Values.OrderBy(v => v.Id)));
        CreateMap<CharacterModifierValueEntity, StatModifierRequest>()
            .ForMember(x => x.Stat, opt => opt.MapFrom(c => c.StatName))
            .ForMember(x => x.Special, opt => opt.Ignore());
        CreateMap<AddCharacterModifierRequest, CharacterModifierEntity>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Character, opt => opt.Ignore())
            .ForMember(x => x.CharacterId, opt => opt.Ignore())
            .ForMember(x => x.Permanent, opt => opt.MapFrom(x => false))
            .ForMember(x => x.IsActive, opt => opt.MapFrom(x => true))
            .ForMember(x => x.CurrentCombatCount, opt => opt.MapFrom(x => x.CombatCount))
            .ForMember(x => x.CurrentTimeDuration, opt => opt.MapFrom(x => x.TimeDuration))
            .ForMember(x => x.CurrentLapCount, opt => opt.MapFrom(x => x.LapCount))
            .ForMember(x => x.LapCountDecrement, opt => opt.MapFrom(x => MapperHelpers.ToJson(x.LapCountDecrement)));
        CreateMap<StatModifierRequest, CharacterModifierValueEntity>()
            .ForMember(x => x.StatName, opt => opt.MapFrom(c => c.Stat))
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.CharacterModifierId, opt => opt.Ignore());
        CreateMap<LevelUpResult, CharacterLevelUpResponse>()
            .ForMember(x => x.NewSkillIds, opt => opt.MapFrom(x => x.NewSkills.Select(s => s.SkillId)))
            .ForMember(x => x.NewSpecialities, opt => opt.MapFrom(x => x.NewSpecialities.Select(s => s.Speciality)));

        CreateMap<CharacterHistoryEntryEntity, CharacterHistoryEntryResponse>()
            .ForMember(m => m.Modifier, opt => opt.MapFrom(im => im.CharacterModifier))
            .ForMember(m => m.Item, opt => opt.MapFrom(im => im.Item))
            .ForMember(m => m.Data, opt => opt.MapFrom(im => MapperHelpers.FromJson<JObject>(im.Data)))
            .ForMember(m => m.Date, opt => opt.MapFrom(b => b.Date.ToString("s")));

        CreateMap<ItemEntity, CharacterHistoryEntryResponse.ItemHistoryResponse>()
            .ForMember(m => m.Name,
                opt => { opt.MapFrom(im => MapperHelpers.ItemNameFromData(im.Data)); });
        CreateMap<CharacterModifierEntity, CharacterHistoryEntryResponse.ModifierHistoryResponse>()
            .ForMember(m => m.Name, opt => opt.MapFrom(im => im.Name));
        CreateMap<EffectEntity, CharacterHistoryEntryResponse.EffectHistoryResponse>()
            .ForMember(m => m.Name, opt => opt.MapFrom(im => im.Name));

        CreateMap<EffectEntity, EffectResponse>()
            .ForMember(m => m.Modifiers, opt => opt.MapFrom(e => e.Modifiers.OrderBy(m => m.StatName)));
        CreateMap<EffectTypeEntity, EffectTypeResponse>()
            .ForMember(m => m.SubCategories, opt => opt.MapFrom(et => et.SubCategories.OrderBy(x => x.Id)));
        CreateMap<EffectSubCategoryEntity, EffectSubCategoryResponse>();
        CreateMap<EffectModifierEntity, StatModifierResponse>()
            .ForMember(m => m.Stat, opt => opt.MapFrom(se => se.StatName))
            .ForMember(m => m.Special, opt => opt.Ignore());

        CreateMap<EventEntity, EventResponse>();

        CreateMap<GodEntity, GodResponse>();

        CreateMap<GroupInviteEntity, DeleteInviteResponse>();
        CreateMap<GroupInviteEntity, CharacterGroupInviteResponse>()
            .ForMember(m => m.Config, opt => opt.MapFrom(im => MapperHelpers.FromJsonNotNull<GroupConfig>(im.Group.Config)))
            .ForMember(m => m.GroupId, opt => opt.MapFrom(i => i.Group.Id))
            .ForMember(m => m.GroupName, opt => opt.MapFrom(i => i.Group.Name));
        CreateMap<GroupInviteEntity, GroupInviteResponse>()
            .ForMember(m => m.Id, opt => opt.MapFrom(i => i.Character.Id))
            .ForMember(m => m.Name, opt => opt.MapFrom(i => i.Character.Name))
            .ForMember(m => m.Group, opt => opt.MapFrom(i => i.Group))
            .ForMember(m => m.OriginName, opt => opt.MapFrom(i => i.Character.Origin.Name))
            .ForMember(m => m.JobNames, opt => opt.MapFrom(i => i.Character.Jobs.Select(j => j.Job.Name)));
        CreateMap<GroupInviteEntity, GroupGroupInviteResponse>()
            .ForMember(m => m.Id, opt => opt.MapFrom(i => i.Character.Id))
            .ForMember(m => m.Name, opt => opt.MapFrom(i => i.Character.Name))
            .ForMember(m => m.OriginName, opt => opt.MapFrom(i => i.Character.Origin.Name))
            .ForMember(m => m.JobNames, opt => opt.MapFrom(i => i.Character.Jobs.Select(j => j.Job.Name)));
        CreateMap<GroupEntity, GroupResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(im => MapperHelpers.FromJson<JObject>(im.Data)))
            .ForMember(m => m.Config, opt => opt.MapFrom(im => MapperHelpers.FromJsonNotNull<GroupConfig>(im.Config)))
            .ForMember(m => m.CharacterIds, opt => opt.MapFrom(g => g.Characters.Select(c => c.Id)));
        CreateMap<GroupEntity, CharacterGroupResponse>()
            .ForMember(m => m.Config, opt => opt.MapFrom(g => MapperHelpers.FromJsonNotNull<GroupConfig>(g.Config)));
        CreateMap<GroupEntity, NamedIdResponse>();
        CreateMap<GroupEntity, GroupSummaryResponse>()
            .ForMember(m => m.CharacterCount, opt => opt.MapFrom(g => g.Characters.Count));
        CreateMap<GroupHistoryEntryEntity, GroupHistoryEntryResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(im => MapperHelpers.FromJson<JObject>(im.Data)))
            .ForMember(m => m.Date, opt => opt.MapFrom(b => b.Date.ToString("s")));

        CreateMap<IHistoryEntry, IHistoryEntryResponse>()
            .Include<GroupHistoryEntryEntity, GroupHistoryEntryResponse>()
            .Include<CharacterHistoryEntryEntity, CharacterHistoryEntryResponse>();

        CreateMap<PostGroupUpdateDurationsRequest, FighterDurationChanges>()
            .ConstructUsing((request, context) =>
            {
                if (request.MonsterId.HasValue)
                    return new MonsterUpdateDuration {MonsterId = request.MonsterId.Value, Changes = context.Mapper.Map<IList<IDurationChange>>(request.Changes)};
                if (request.CharacterId.HasValue)
                    return new CharacterUpdateDuration {CharacterId = request.CharacterId.Value, Changes = context.Mapper.Map<IList<IDurationChange>>(request.Changes)};
                throw new NotSupportedException("Either MonsterId or CharacterId should be set");
            });
        CreateMap<DurationChangeRequest, IDurationChange>()
            .ConstructUsing((request, _) =>
            {
                switch (request.Type)
                {
                    case ItemModifierDurationChange.TypeValue:
                        if (!request.ItemId.HasValue)
                            throw new NotSupportedException($"Missing itemId when converting DurationChange of type `{request.Type}`");
                        return new ItemModifierDurationChange {ItemId = request.ItemId.Value, Modifier = request.Modifier!};
                    case ItemLifetimeDurationChange.TypeValue:
                        if (!request.ItemId.HasValue)
                            throw new NotSupportedException($"Missing itemId when converting DurationChange of type `{request.Type}`");
                        return new ItemLifetimeDurationChange {ItemId = request.ItemId.Value, LifeTime = request.LifeTime!};
                    case ModifierDurationChange.TypeValue:
                        return new ModifierDurationChange {Modifier = request.Modifier!};
                    default:
                        throw new NotSupportedException($"Invalid ChangeDurationType `{request.Type}`");
                }
            });

        CreateMap<ItemEntity, ItemPartialResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(x => MapperHelpers.FromJson<JObject>(x.Data) ?? new JObject()))
            .ForMember(m => m.Modifiers, opt => opt.MapFrom(x => MapperHelpers.FromJson<List<ActiveStatsModifier>>(x.Modifiers)));
        CreateMap<ItemEntity, ItemResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(x => MapperHelpers.FromJson<JObject>(x.Data) ?? new JObject()))
            .ForMember(m => m.Modifiers, opt => opt.MapFrom(x => MapperHelpers.FromJson<List<ActiveStatsModifier>>(x.Modifiers)));

        CreateMap<ItemTemplateEntity, ItemTemplateResponse>()
            .ForMember(m => m.SkillIds, opt => opt.MapFrom(i => i.Skills.Select(s => s.SkillId)))
            .ForMember(m => m.UnSkillIds, opt => opt.MapFrom(i => i.UnSkills.Select(s => s.SkillId)))
            .ForMember(m => m.Data, opt => opt.MapFrom(i => MapperHelpers.FromJson<JObject>(i.Data) ?? new JObject()))
            .ForMember(m => m.Slots, opt => opt.MapFrom(i => i.Slots.Select(x => x.Slot)))
            .ForMember(m => m.SourceUser, opt => opt.MapFrom(i => i.SourceUserNameCache));
        CreateMap<ItemTemplateModifierEntity, ItemTemplateModifierResponse>()
            .ForMember(m => m.JobId, opt => opt.MapFrom(im => im.RequiredJobId))
            .ForMember(m => m.OriginId, opt => opt.MapFrom(im => im.RequiredOriginId))
            .ForMember(m => m.Special, opt => opt.MapFrom(im => MapperHelpers.FromCommaSeparatedList(im.Special)))
            .ForMember(m => m.Stat, opt => opt.MapFrom(im => im.StatName));
        CreateMap<ItemTemplateRequirementEntity, ItemTemplateRequirementResponse>()
            .ForMember(m => m.Stat, opt => opt.MapFrom(ir => ir.StatName))
            .ForMember(m => m.Min, opt => opt.MapFrom(ir => ir.MinValue))
            .ForMember(m => m.Max, opt => opt.MapFrom(ir => ir.MaxValue));
        CreateMap<ItemTemplateSkillModifierEntity, ItemTemplateSkillModifierResponse>();
        CreateMap<ItemTemplateSlotEntity, IdResponse>()
            .ForMember(m => m.Id, opt => opt.MapFrom(i => i.SlotId));
        CreateMap<ItemTemplateSectionEntity, ItemTemplateSectionResponse>()
            .ForMember(m => m.SubCategories, opt => opt.MapFrom(et => et.SubCategories.OrderBy(x => x.Id)))
            .ForMember(m => m.Specials, opt => opt.MapFrom(i => MapperHelpers.FromCommaSeparatedList(i.Special)));
        CreateMap<ItemTemplateSubCategoryEntity, ItemTemplateSubCategoryResponse>();
        CreateMap<ItemTypeEntity, ItemTypeResponse>();
        CreateMap<SlotEntity, ItemSlotResponse>();

        CreateMap<JobEntity, JobResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(j => MapperHelpers.FromJsonNotNull<JobData>(j.Data)))
            .ForMember(m => m.Requirements, opt => opt.MapFrom(j => j.Requirements.OrderBy(r => r.Id)))
            .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)))
            .ForMember(m => m.AvailableSkillIds, opt => opt.MapFrom(j => j.Skills.Where(s => !s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)))
            .ForMember(m => m.SkillIds, opt => opt.MapFrom(j => j.Skills.Where(s => s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)));
        CreateMap<JobBonusEntity, DescribedFlagResponse>()
            .ForMember(m => m.Flags, opt => opt.MapFrom(b => MapperHelpers.FromJson<List<FlagResponse>>(b.Flags)));
        CreateMap<JobRequirementEntity, StatRequirementResponse>()
            .ForMember(m => m.Stat, opt => opt.MapFrom(r => r.StatName))
            .ForMember(m => m.Min, opt => opt.MapFrom(r => r.MinValue))
            .ForMember(m => m.Max, opt => opt.MapFrom(r => r.MaxValue));
        CreateMap<JobRestrictionEntity, DescribedFlagResponse>()
            .ForMember(m => m.Description, opt => opt.MapFrom(r => r.Text))
            .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)));

        CreateMap<LootEntity, LootResponse>();

        CreateMap<OriginEntity, OriginResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(o => MapperHelpers.FromJsonNotNull<OriginData>(o.Data)))
            .ForMember(m => m.Requirements, opt => opt.MapFrom(o => o.Requirements.OrderBy(r => r.Id)))
            .ForMember(m => m.Flags, opt => opt.MapFrom(o => MapperHelpers.FromJson<List<FlagResponse>>(o.Flags)))
            .ForMember(m => m.AvailableSkillIds, opt => opt.MapFrom(o => o.Skills.Where(s => !s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)))
            .ForMember(m => m.SkillIds, opt => opt.MapFrom(o => o.Skills.Where(s => s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId)));
        CreateMap<OriginInfoEntity, OriginInformationResponse>();
        CreateMap<OriginRequirementEntity, OriginRequirementResponse>()
            .ForMember(m => m.Stat, opt => opt.MapFrom(r => r.StatName))
            .ForMember(m => m.Min, opt => opt.MapFrom(r => r.MinValue))
            .ForMember(m => m.Max, opt => opt.MapFrom(r => r.MaxValue));
        CreateMap<OriginBonus, DescribedFlagResponse>()
            .ForMember(m => m.Flags, opt => opt.MapFrom(b => MapperHelpers.FromJson<List<FlagResponse>>(b.Flags)));
        CreateMap<OriginRestrictEntity, DescribedFlagResponse>()
            .ForMember(m => m.Description, opt => opt.MapFrom(r => r.Text))
            .ForMember(m => m.Flags, opt => opt.MapFrom(r => MapperHelpers.FromJson<List<FlagResponse>>(r.Flags)));

        CreateMap<MapEntity, MapResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(r => MapperHelpers.FromJson<MapData>(r.Data)))
            .ForMember(m => m.ImageData, opt => opt.MapFrom(r => MapperHelpers.FromJson<MapImageData>(r.ImageData)));
        CreateMap<MapEntity, MapSummaryResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(r => MapperHelpers.FromJson<MapData>(r.Data)));
        CreateMap<MapLayerEntity, MapLayerResponse>();
        CreateMap<MapMarkerEntity, MapMarkerResponse>()
            .ForMember(m => m.MarkerInfo, opt => opt.MapFrom(r => MapperHelpers.FromJsonNotNull<JObject>(r.MarkerInfo)))
            .ForMember(m => m.Links, opt => opt.MapFrom(r => r.Links));
        CreateMap<MapMarkerLinkEntity, MapMarkerLinkResponse>()
            .ForMember(m => m.TargetMapName, opt => opt.MapFrom(r => r.TargetMap.Name))
            .ForMember(m => m.TargetMapIsGm, opt => opt.MapFrom(r => MapperHelpers.FromJsonNotNull<MapData>(r.TargetMap.Data).IsGm));

        CreateMap<MonsterEntity, DeadMonsterResponse>()
            .ForMember(m => m.Dead, opt => opt.MapFrom(b => MapperHelpers.FromDateTimeToString(b.Dead)))
            .ForMember(m => m.Data, opt => opt.MapFrom(b => MapperHelpers.FromJson<JObject>(b.Data)));
        CreateMap<MonsterEntity, MonsterResponse>()
            .ForMember(m => m.Dead, opt => opt.MapFrom(b => MapperHelpers.FromDateTimeToString(b.Dead)))
            .ForMember(m => m.Data, opt => opt.MapFrom(b => MapperHelpers.FromJson<JObject>(b.Data)))
            .ForMember(m => m.Modifiers, opt => opt.MapFrom(b => MapperHelpers.FromJson<List<ActiveStatsModifier>>(b.Modifiers)));

        CreateMap<MonsterTemplateEntity, MonsterTemplateResponse>()
            .ForMember(x => x.Inventory, opt => opt.MapFrom(m => m.Items))
            .ForMember(m => m.Data, opt => opt.MapFrom(b => MapperHelpers.FromJson<JObject>(b.Data)));
        CreateMap<MonsterTemplateInventoryElementEntity, MonsterTemplateResponse.MonsterTemplateInventoryElementResponse>();
        CreateMap<MonsterTypeEntity, MonsterTypeResponse>()
            .ForMember(m => m.SubCategories, opt => opt.MapFrom(c => c.SubCategories.OrderBy(ca => ca.Id)));
        CreateMap<MonsterSubCategoryEntity, MonsterSubCategoryResponse>();
        CreateMap<MonsterTraitEntity, MonsterTraitResponse>()
            .ForMember(m => m.Levels, opt => opt.MapFrom(b => MapperHelpers.FromJson<List<string>>(b.Levels)));

        CreateMap<NpcEntity, NpcResponse>()
            .ForMember(m => m.Data, opt => opt.MapFrom(r => MapperHelpers.FromJson<NpcData>(r.Data)));

        CreateMap<SkillEffect, SkillEffectResponse>()
            .ForMember(m => m.Type, opt => opt.MapFrom(x => "ADD"))
            .ForMember(m => m.Stat, opt => opt.MapFrom(s => s.StatName));
        CreateMap<SkillEntity, SkillResponse>()
            .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)))
            .ForMember(m => m.Effects, opt => opt.MapFrom(s => s.SkillEffects))
            .ForMember(m => m.Stat, opt => opt.MapFrom(s => MapperHelpers.FromCommaSeparatedStringArray(s.Stat)));

        CreateMap<SpecialityEntity, SpecialityResponse>()
            .ForMember(m => m.Modifiers, opt => opt.MapFrom(s => s.Modifiers.OrderBy(m => m.Id)))
            .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)))
            .ForMember(m => m.Specials, opt => opt.MapFrom(s => s.Specials.OrderBy(p => p.Id)));
        CreateMap<SpecialitySpecialEntity, SpecialitySpecialResponse>()
            .ForMember(m => m.Flags, opt => opt.MapFrom(s => MapperHelpers.FromJson<List<FlagResponse>>(s.Flags)));
        CreateMap<SpecialityModifierEntity, StatModifierResponse>()
            .ForMember(m => m.Type, opt => opt.MapFrom(x => "ADD"))
            .ForMember(m => m.Special, opt => opt.Ignore());

        CreateMap<string?, LapCountDecrement?>().ConvertUsing(c => MapperHelpers.FromJson<LapCountDecrement>(c));

        CreateMap<StatEntity, StatResponse>();

        CreateMap<UserEntity, UserInfoResponse>()
            .ForMember(m => m.ShowInSearch, opt => opt.MapFrom(u => u.ShowInSearchUntil > DateTime.UtcNow))
            .ForMember(m => m.LinkedWithFb, opt => opt.MapFrom(u => u.FbId != null))
            .ForMember(m => m.LinkedWithTwitter, opt => opt.MapFrom(u => u.TwitterId != null))
            .ForMember(m => m.LinkedWithGoogle, opt => opt.MapFrom(u => u.TwitterId != null))
            .ForMember(m => m.LinkedWithMicrosoft, opt => opt.MapFrom(u => u.MicrosoftId != null))
            ;
        CreateMap<UserEntity, UserSearchResponse>();
        CreateMap<UserAccessTokenEntity, UserAccessTokenResponse>();
        CreateMap<UserAccessTokenEntity, UserAccessTokenResponseWithKey>()
            .IncludeBase<UserAccessTokenEntity, UserAccessTokenResponse>()
            ;
    }
}
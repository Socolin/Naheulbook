using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Job;

public interface IJobMapper
{
    JobResponse ToResponse(JobEntity job);
    DescribedFlagResponse ToResponse(JobBonusEntity jobBonus);
    StatRequirementResponse ToResponse(JobRequirementEntity jobRequirement);
    DescribedFlagResponse ToResponse(JobRestrictionEntity jobRestriction);
}

public class JobMapper(ISpecialityMapper specialityMapper) : IJobMapper
{
    public JobResponse ToResponse(JobEntity job)
    {
        return new JobResponse
        {
            Id = job.Id,
            Name = job.Name,
            Information = job.Information,
            PlayerDescription = job.PlayerDescription,
            PlayerSummary = job.PlayerSummary,
            IsMagic = job.IsMagic,
            Data = MapperHelpers.FromJsonNotNull<JobData>(job.Data),
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(job.Flags) ?? [],
            SkillIds = job.Skills.Where(s => s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId).ToList(),
            AvailableSkillIds = job.Skills.Where(s => !s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId).ToList(),
            Bonuses = job.Bonuses.Select(ToResponse).ToList(),
            Requirements = job.Requirements.OrderBy(r => r.Id).Select(ToResponse).ToList(),
            Restrictions = job.Restrictions.Select(ToResponse).ToList(),
            Specialities = job.Specialities.Select(specialityMapper.ToResponse).ToList(),
        };
    }

    public DescribedFlagResponse ToResponse(JobBonusEntity jobBonus)
    {
        return new DescribedFlagResponse
        {
            Description = jobBonus.Description,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(jobBonus.Flags) ?? [],
        };
    }

    public StatRequirementResponse ToResponse(JobRequirementEntity jobRequirement)
    {
        return new StatRequirementResponse
        {
            Stat = jobRequirement.StatName,
            Min = (int?)jobRequirement.MinValue,
            Max = (int?)jobRequirement.MaxValue,
        };
    }

    public DescribedFlagResponse ToResponse(JobRestrictionEntity jobRestriction)
    {
        return new DescribedFlagResponse
        {
            Description = jobRestriction.Text,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(jobRestriction.Flags) ?? [],
        };
    }
}

using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.Configurations;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.DbContexts
{
    public class NaheulbookDbContext : DbContext
    {
        public DbSet<Effect> Effects { get; set; }
        public DbSet<EffectType> EffectTypes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Origin> Origins { get; set; }
        public DbSet<Job> Jobs { get; set; }

        public NaheulbookDbContext(DbContextOptions<NaheulbookDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EffectConfiguration());
            modelBuilder.ApplyConfiguration(new EffectCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new EffectModifierConfiguration());
            modelBuilder.ApplyConfiguration(new EffectTypeConfiguration());

            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new JobBonusConfiguration());
            modelBuilder.ApplyConfiguration(new JobOriginBlacklistConfiguration());
            modelBuilder.ApplyConfiguration(new JobOriginWhitelistConfiguration());
            modelBuilder.ApplyConfiguration(new JobRequirementConfiguration());
            modelBuilder.ApplyConfiguration(new JobRestrictConfiguration());
            modelBuilder.ApplyConfiguration(new JobSkillConfiguration());

            modelBuilder.ApplyConfiguration(new OriginConfiguration());
            modelBuilder.ApplyConfiguration(new OriginBonusConfiguration());
            modelBuilder.ApplyConfiguration(new OriginInfoConfiguration());
            modelBuilder.ApplyConfiguration(new OriginRequirementConfiguration());
            modelBuilder.ApplyConfiguration(new OriginRestrictConfiguration());
            modelBuilder.ApplyConfiguration(new OriginSkillConfiguration());

            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new SkillEffectConfiguration());

            modelBuilder.ApplyConfiguration(new SpecialityConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialityModifierConfiguration());
            modelBuilder.ApplyConfiguration(new SpecialitySpecialConfiguration());

            modelBuilder.ApplyConfiguration(new StatConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
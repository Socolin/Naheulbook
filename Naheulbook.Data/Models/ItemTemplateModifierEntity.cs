using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class ItemTemplateModifierEntity
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public string? Special { get; set; }
        public string Type { get; set; } = null!;

        public Guid ItemTemplateId { get; set; }
        private ItemTemplateEntity _itemTemplate = null!;
        public ItemTemplateEntity ItemTemplate { get => _itemTemplate.ThrowIfNotLoaded(); set => _itemTemplate = value; }

        public Guid? RequiredJobId { get; set; }
        private JobEntity? _requiredJob;
        public JobEntity? RequiredJob { get => _requiredJob.ThrowIfNotLoadedAndNotNull(RequiredJobId); set => _requiredJob = value; }

        public Guid? RequiredOriginId { get; set; }
        private OriginEntity? _requiredOrigin;
        public OriginEntity? RequiredOrigin { get => _requiredOrigin.ThrowIfNotLoadedAndNotNull(RequiredOriginId); set => _requiredOrigin = value; }

        public string StatName { get; set; } = null!;
        private StatEntity? _stat;
        public StatEntity Stat { get => _stat.ThrowIfNotLoaded(); set => _stat = value; }
    }
}
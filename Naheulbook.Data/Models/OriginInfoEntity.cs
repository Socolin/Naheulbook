using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class OriginInfoEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Guid OriginId { get; set; }
        private OriginEntity? _origin;
        public OriginEntity Origin { get => _origin.ThrowIfNotLoaded(); set => _origin = value; }
    }
}
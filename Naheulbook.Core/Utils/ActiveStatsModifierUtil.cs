using System.Collections.Generic;
using System.Linq;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Utils
{
    public interface IActiveStatsModifierUtil
    {
        void InitializeModifierIds(ICollection<ActiveStatsModifier> modifiers);
    }

    public class ActiveStatsModifierUtil : IActiveStatsModifierUtil
    {
        public void InitializeModifierIds(ICollection<ActiveStatsModifier> modifiers)
        {
            var nextId = modifiers.Select(x => x.Id).Max() + 1;
            foreach (var statsModifier in modifiers)
            {
                if (statsModifier.Id == 0)
                    statsModifier.Id = nextId++;
            }
        }
    }
}
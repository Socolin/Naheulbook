using System.Collections.Generic;
using System.Linq;
using Naheulbook.Core.Exceptions;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Utils;

public interface IActiveStatsModifierUtil
{
    void InitializeModifierIds(ICollection<ActiveStatsModifier>? modifiers);
    void AddModifier(IList<ActiveStatsModifier> modifiers, ActiveStatsModifier newModifier);
    void RemoveModifier(IList<ActiveStatsModifier> modifiers, int modifierId);
}

public class ActiveStatsModifierUtil : IActiveStatsModifierUtil
{
    public void InitializeModifierIds(ICollection<ActiveStatsModifier>? modifiers)
    {
        if (modifiers == null || modifiers.Count == 0)
            return;
        var nextId = modifiers.Select(x => x.Id).Max() + 1;
        foreach (var statsModifier in modifiers)
        {
            if (statsModifier.Id == 0)
                statsModifier.Id = nextId++;
        }
    }

    public void AddModifier(IList<ActiveStatsModifier> modifiers, ActiveStatsModifier newModifier)
    {
        if (modifiers.Count > 0)
        {
            newModifier.Id = modifiers.Max(m => m.Id) + 1;
        }
        else
        {
            newModifier.Id = 1;
        }

        newModifier.Active = true;
        newModifier.CurrentLapCount = newModifier.LapCount;
        newModifier.CurrentCombatCount = newModifier.CombatCount;
        newModifier.CurrentTimeDuration = newModifier.TimeDuration;
        modifiers.Add(newModifier);
    }

    public void RemoveModifier(IList<ActiveStatsModifier> modifiers, int modifierId)
    {
        var deletedModifier = modifiers.FirstOrDefault(modifier => modifier.Id == modifierId);
        if (deletedModifier != null)
            modifiers.Remove(deletedModifier);
        else
            throw new MonsterModifierNotFoundException(modifierId);
    }
}
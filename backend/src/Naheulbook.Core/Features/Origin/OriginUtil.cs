using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Origin;

public interface IOriginUtil
{
    bool HasFlag(OriginEntity origin, string chaLvl2Lvl3);
}

public class OriginUtil(IJsonUtil jsonUtil) : IOriginUtil
{
    public bool HasFlag(OriginEntity origin, string flagName)
    {
        var originFlags = jsonUtil.Deserialize<IList<NhbkFlag>>(origin.Flags) ?? new List<NhbkFlag>();
        if (originFlags.Any(f => f.Type == flagName))
            return true;

        foreach (var bonus in origin.Bonuses)
        {
            var flags = jsonUtil.Deserialize<IList<NhbkFlag>>(bonus.Flags) ?? new List<NhbkFlag>();
            if (flags.Any(f => f.Type == flagName))
                return true;
        }

        foreach (var restrict in origin.Restrictions)
        {
            var flags = jsonUtil.Deserialize<IList<NhbkFlag>>(restrict.Flags) ?? new List<NhbkFlag>();
            if (flags.Any(f => f.Type == flagName))
                return true;
        }

        return false;
    }
}
using System.Collections.Generic;
using System.Linq;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IOriginUtil
    {
        bool HasFlag(Origin origin, string chaLvl2Lvl3);
    }

    public class OriginUtil : IOriginUtil
    {
        private readonly IJsonUtil _jsonUtil;

        public OriginUtil(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
        }

        public bool HasFlag(Origin origin, string flagName)
        {
            var originFlags = _jsonUtil.Deserialize<IList<NhbkFlag>>(origin.Flags) ?? new List<NhbkFlag>();
            if (originFlags.Any(f => f.Type == flagName))
                return true;

            foreach (var bonus in origin.Bonuses)
            {
                var flags = _jsonUtil.Deserialize<IList<NhbkFlag>>(bonus.Flags) ?? new List<NhbkFlag>();
                if (flags.Any(f => f.Type == flagName))
                    return true;
            }

            foreach (var restrict in origin.Restrictions)
            {
                var flags = _jsonUtil.Deserialize<IList<NhbkFlag>>(restrict.Flags) ?? new List<NhbkFlag>();
                if (flags.Any(f => f.Type == flagName))
                    return true;
            }

            return false;
        }
    }
}
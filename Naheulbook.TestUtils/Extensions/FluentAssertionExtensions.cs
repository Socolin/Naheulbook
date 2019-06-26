using System;
using System.Linq;
using FluentAssertions.Equivalency;

namespace Naheulbook.TestUtils.Extensions
{
    public static class FluentAssertionExtensions
    {
        public static EquivalencyAssertionOptions<TExpectation> ExcludingChildren<TExpectation>(this EquivalencyAssertionOptions<TExpectation> config)
        {
            return config
                .ExcludingFields()
                .Including(x => (
                                    !x.SelectedMemberPath.Contains(".")
                                    || (x.SelectedMemberPath.StartsWith("[") && x.SelectedMemberPath.Count(c => c == '.') == 1)
                                )
                                && (
                                    x.SelectedMemberInfo.MemberType == typeof(string)
                                    || x.SelectedMemberInfo.MemberType == typeof(bool)
                                    || x.SelectedMemberInfo.MemberType == typeof(short)
                                    || x.SelectedMemberInfo.MemberType == typeof(int)
                                    || x.SelectedMemberInfo.MemberType == typeof(long)
                                    || x.SelectedMemberInfo.MemberType == typeof(ushort)
                                    || x.SelectedMemberInfo.MemberType == typeof(uint)
                                    || x.SelectedMemberInfo.MemberType == typeof(ulong)
                                    || x.SelectedMemberInfo.MemberType == typeof(byte)
                                    || x.SelectedMemberInfo.MemberType == typeof(sbyte)
                                    || x.SelectedMemberInfo.MemberType == typeof(char)
                                    || x.SelectedMemberInfo.MemberType == typeof(float)
                                    || x.SelectedMemberInfo.MemberType == typeof(double)
                                    || x.SelectedMemberInfo.MemberType == typeof(decimal)
                                    || x.SelectedMemberInfo.MemberType == typeof(Guid)
                                )
                );
        }
    }
}


// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Monster;

public class MonsterSubCategoryNotFoundException(int subCategoryId) : Exception
{
    public int SubCategoryId { get; } = subCategoryId;
}
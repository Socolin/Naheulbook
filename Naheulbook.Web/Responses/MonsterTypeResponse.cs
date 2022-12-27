using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses;

public class MonsterTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IList<MonsterSubCategoryResponse> SubCategories { get; set; } = null!;
}
using System;

namespace Naheulbook.Data.Models
{
    public interface IHistoryEntry
    {
        string Action { get; set; }
        bool Gm { get; set; }
        DateTime Date { get; set; }
        string? Data { get; set; }
    }
}
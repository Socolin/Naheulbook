using System;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests;

public class CreateItemRequest
{
    public Guid ItemTemplateId { get; set; }
    public ItemData ItemData { get; set; } = null!;
}
namespace Naheulbook.Shared.TransientModels;

public class ItemTemplateConfinsDuGivreData
{
    /// <summary>Local price in the Confins du Givre, expressed in <see cref="Currency"/></summary>
    public int? Price { get; set; }

    public ZgarugCurrency? Currency { get; set; }
}

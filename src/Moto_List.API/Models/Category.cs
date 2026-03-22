namespace Moto_List.API.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<MotoItem> MotoItems { get; set; } = [];
}

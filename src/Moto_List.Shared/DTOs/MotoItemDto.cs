namespace Moto_List.Shared.DTOs;

public class MotoItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsChecked { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}

public class CreateMotoItemRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateMotoItemRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsChecked { get; set; }
    public int? CategoryId { get; set; }
}

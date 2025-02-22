namespace StainManager.Domain.Common;

public class Entity
{
    public int Id { get; init; }

    public DateTime CreatedDateTime { get; init; } = DateTime.Now;

    public string CreatedBy { get; set; } = "System";

    public DateTime? UpdatedDateTime { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; set; } = true;
}
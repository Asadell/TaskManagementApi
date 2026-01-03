namespace TaskManagementApi.Core.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Todo"; // Todo, InProgress, Review, Done
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public int ProjectId { get; set; }
    public int? AssignedToId { get; set; }
    public int CreatedById { get; set; }

    public Project Project { get; set; } = null!;
    public User? AssignedTo { get; set; }
    public User CreatedBy { get; set; } = null!;
}
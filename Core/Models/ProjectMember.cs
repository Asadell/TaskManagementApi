namespace TaskManagementApi.Core.Models;

public class ProjectMember
{
    public int Id { get; set; }
    public string Role { get; set; } = "Member"; // Owner, Manager, Member
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }
    public int UserId { get; set; }

    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}
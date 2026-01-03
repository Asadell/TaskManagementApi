namespace TaskManagementApi.Core.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserDto Owner { get; set; } = null!;
    public int MemberCount { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}

public class ProjectDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public UserDto Owner { get; set; } = null!;
    public List<ProjectMemberDto> Members { get; set; } = new();
    public List<TaskDto> Tasks { get; set; } = new();
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ProjectMemberDto
{
    public int Id { get; set; }
    public UserDto User { get; set; } = null!;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class AddProjectMemberDto
{
    public int UserId { get; set; }
    public string Role { get; set; } = "Member";
}
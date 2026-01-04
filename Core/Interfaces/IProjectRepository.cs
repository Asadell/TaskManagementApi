using TaskManagementApi.Core.Models;

namespace TaskManagementApi.Core.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetUserProjectsAsync(int userId);
    Task<Project?> GetProjectWithDetailsAsync(int projectId);
    Task<IEnumerable<Project>> GetProjectsByOwnerAsync(int ownerId);
    Task<bool> IsUserMemberAsync(int projectId, int userId);
    Task<ProjectMember?> GetProjectMemberAsync(int projectId, int userId);
    Task AddMemberAsync(ProjectMember member);
}
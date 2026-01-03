using TaskManagementApi.Core.DTOs;

namespace TaskManagementApi.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId);
    Task<ProjectDetailDto?> GetProjectByIdAsync(int projectId, int userId);
    Task<ProjectDto> CreateProjectAsync(int userId, CreateProjectDto createProjectDto);
    Task<bool> UpdateProjectAsync(int projectId, int userId, UpdateProjectDto updateProjectDto);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
    Task<bool> AddMemberAsync(int projectId, int userId, AddProjectMemberDto addMemberDto);
    Task<bool> RemoveMemberAsync(int projectId, int userId, int memberUserId);
}
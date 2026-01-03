using TaskManagementApi.Core.Models;

namespace TaskManagementApi.Core.Interfaces;

public interface ITaskRepository : IRepository<ProjectTask>
{
    Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(int projectId);
    Task<IEnumerable<ProjectTask>> GetTasksByUserAsync(int userId);
    Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(int projectId, string status);
    Task<ProjectTask?> GetTaskWithDetailsAsync(int taskId);
}
using TaskManagementApi.Core.DTOs;

namespace TaskManagementApi.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId, int userId);
    Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId);
    Task<TaskDto?> GetTaskByIdAsync(int taskId, int userId);
    Task<TaskDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto);
    Task<bool> UpdateTaskAsync(int taskId, int userId, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(int taskId, int userId);
}
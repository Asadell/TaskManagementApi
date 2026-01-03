using AutoMapper;
using TaskManagementApi.Core.DTOs;
using TaskManagementApi.Core.Models;
using TaskManagementApi.Core.Interfaces;

namespace TaskManagementApi.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TaskService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskDto>> GetProjectTasksAsync(int projectId, int userId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new Exception("Project not found");
        }

        if (project.OwnerId != userId && !await _unitOfWork.Projects.IsUserMemberAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have access to this project");
        }

        var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(projectId);
        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }

    public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId)
    {
        var tasks = await _unitOfWork.Tasks.GetTasksByUserAsync(userId);
        return _mapper.Map<IEnumerable<TaskDto>>(tasks);
    }

    public async Task<TaskDto?> GetTaskByIdAsync(int taskId, int userId)
    {
        var task = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(taskId);
        
        if (task == null)
        {
            return null;
        }

        var project = task.Project;
        if (project.OwnerId != userId && !await _unitOfWork.Projects.IsUserMemberAsync(project.Id, userId))
        {
            throw new UnauthorizedAccessException("You don't have access to this task");
        }

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateTaskAsync(int userId, CreateTaskDto createTaskDto)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(createTaskDto.ProjectId);
        if (project == null)
        {
            throw new Exception("Project not found");
        }

        if (project.OwnerId != userId && !await _unitOfWork.Projects.IsUserMemberAsync(createTaskDto.ProjectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have access to this project");
        }

        if (createTaskDto.AssignedToId.HasValue)
        {
            if (!await _unitOfWork.Projects.IsUserMemberAsync(createTaskDto.ProjectId, createTaskDto.AssignedToId.Value))
            {
                throw new Exception("Assigned user is not a member of this project");
            }
        }

        var task = _mapper.Map<ProjectTask>(createTaskDto);
        task.CreatedById = userId;
        task.Status = "Todo";

        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        var createdTask = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(task.Id);
        return _mapper.Map<TaskDto>(createdTask!);
    }

    public async Task<bool> UpdateTaskAsync(int taskId, int userId, UpdateTaskDto updateTaskDto)
    {
        var task = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(taskId);
        
        if (task == null)
        {
            return false;
        }

        var project = task.Project;
        if (project.OwnerId != userId && !await _unitOfWork.Projects.IsUserMemberAsync(project.Id, userId))
        {
            throw new UnauthorizedAccessException("You don't have access to this task");
        }

        if (updateTaskDto.AssignedToId.HasValue)
        {
            if (!await _unitOfWork.Projects.IsUserMemberAsync(task.ProjectId, updateTaskDto.AssignedToId.Value))
            {
                throw new Exception("Assigned user is not a member of this project");
            }
        }

        _mapper.Map(updateTaskDto, task);

        if (updateTaskDto.Status == "Done" && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (updateTaskDto.Status != "Done")
        {
            task.CompletedAt = null;
        }

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int userId)
    {
        var task = await _unitOfWork.Tasks.GetTaskWithDetailsAsync(taskId);
        
        if (task == null)
        {
            return false;
        }

        var project = task.Project;
        if (project.OwnerId != userId && task.CreatedById != userId)
        {
            throw new UnauthorizedAccessException("Only project owner or task creator can delete the task");
        }

        _unitOfWork.Tasks.Delete(task);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.Interfaces;
using TaskManagementApi.Core.Models;
using TaskManagementApi.Infrastructure.Data;

namespace TaskManagementApi.Infrastructure.Repositories;

public class TaskRepository : Repository<ProjectTask>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(int projectId)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Status)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksByUserAsync(int userId)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.AssignedToId == userId)
            .OrderBy(t => t.Status)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(int projectId, string status)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .Where(t => t.ProjectId == projectId && t.Status == status)
            .ToListAsync();
    }

    public async Task<ProjectTask?> GetTaskWithDetailsAsync(int taskId)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Include(t => t.AssignedTo)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }
}
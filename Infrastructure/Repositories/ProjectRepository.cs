using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.Interfaces;
using TaskManagementApi.Core.Models;
using TaskManagementApi.Infrastructure.Data;

namespace TaskManagementApi.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetUserProjectsAsync(int userId)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
            .ToListAsync();
    }

    public async Task<Project?> GetProjectWithDetailsAsync(int projectId)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.AssignedTo)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.CreatedBy)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    }

    public async Task<IEnumerable<Project>> GetProjectsByOwnerAsync(int ownerId)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<bool> IsUserMemberAsync(int projectId, int userId)
    {
        return await _context.ProjectMembers
            .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId);
    }

    public async Task<ProjectMember?> GetProjectMemberAsync(int projectId, int userId)
    {
        return await _context.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId);
    }

    public async Task AddMemberAsync(ProjectMember member)
    {
        await _context.ProjectMembers.AddAsync(member);
    }
}
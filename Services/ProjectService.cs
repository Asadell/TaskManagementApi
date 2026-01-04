using AutoMapper;
using TaskManagementApi.Core.DTOs;
using TaskManagementApi.Core.Models;
using TaskManagementApi.Core.Interfaces;

namespace TaskManagementApi.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId)
    {
        var projects = await _unitOfWork.Projects.GetUserProjectsAsync(userId);
        return _mapper.Map<IEnumerable<ProjectDto>>(projects);
    }

    public async Task<ProjectDetailDto?> GetProjectByIdAsync(int projectId, int userId)
    {
        var project = await _unitOfWork.Projects.GetProjectWithDetailsAsync(projectId);
        
        if (project == null)
        {
            return null;
        }

        if (project.OwnerId != userId && !await _unitOfWork.Projects.IsUserMemberAsync(projectId, userId))
        {
            throw new UnauthorizedAccessException("You don't have access to this project");
        }

        return _mapper.Map<ProjectDetailDto>(project);
    }

    public async Task<ProjectDto> CreateProjectAsync(int userId, CreateProjectDto createProjectDto)
    {
        var project = _mapper.Map<Project>(createProjectDto);
        project.OwnerId = userId;

        await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        var member = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = userId,
            Role = "Owner"
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        var createdProject = await _unitOfWork.Projects.GetProjectWithDetailsAsync(project.Id);
        return _mapper.Map<ProjectDto>(createdProject!);
    }

    public async Task<bool> UpdateProjectAsync(int projectId, int userId, UpdateProjectDto updateProjectDto)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
        
        if (project == null)
        {
            return false;
        }

        if (project.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only project owner can update the project");
        }

        _mapper.Map(updateProjectDto, project);
        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
        
        if (project == null)
        {
            return false;
        }

        if (project.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only project owner can delete the project");
        }

        _unitOfWork.Projects.Delete(project);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddMemberAsync(int projectId, int userId, AddProjectMemberDto addMemberDto)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
        
        if (project == null)
        {
            return false;
        }

        if (project.OwnerId != userId)
        {
            var currentMember = await _unitOfWork.Projects.GetProjectMemberAsync(projectId, userId);
            if (currentMember == null || (currentMember.Role != "Owner" && currentMember.Role != "Manager"))
            {
                throw new UnauthorizedAccessException("Only project owner or manager can add members");
            }
        }

        var userToAdd = await _unitOfWork.Users.GetByIdAsync(addMemberDto.UserId);
        if (userToAdd == null)
        {
            throw new Exception("User not found");
        }

        if (await _unitOfWork.Projects.IsUserMemberAsync(projectId, addMemberDto.UserId))
        {
            throw new Exception("User is already a member");
        }

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = addMemberDto.UserId,
            Role = addMemberDto.Role
        };
        await _unitOfWork.Projects.AddMemberAsync(member);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveMemberAsync(int projectId, int userId, int memberUserId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId);
        
        if (project == null)
        {
            return false;
        }

        if (project.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("Only project owner can remove members");
        }

        if (memberUserId == project.OwnerId)
        {
            throw new Exception("Cannot remove project owner");
        }

        var member = await _unitOfWork.Projects.GetProjectMemberAsync(projectId, memberUserId);
        if (member == null)
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
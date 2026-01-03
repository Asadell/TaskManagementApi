using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagementApi.Core.DTOs;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    // GET: api/projects
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
    {
        var userId = GetCurrentUserId();
        var projects = await _projectService.GetUserProjectsAsync(userId);
        return Ok(projects);
    }

    // GET: api/projects/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailDto>> GetProject(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var project = await _projectService.GetProjectByIdAsync(id, userId);
            
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            return Ok(project);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    // POST: api/projects
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto createProjectDto)
    {
        if (string.IsNullOrWhiteSpace(createProjectDto.Name))
        {
            return BadRequest(new { message = "Project name is required" });
        }

        var userId = GetCurrentUserId();
        var project = await _projectService.CreateProjectAsync(userId, createProjectDto);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    // PUT: api/projects/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto updateProjectDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _projectService.UpdateProjectAsync(id, userId, updateProjectDto);
            
            if (!success)
            {
                return NotFound(new { message = "Project not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    // DELETE: api/projects/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _projectService.DeleteProjectAsync(id, userId);
            
            if (!success)
            {
                return NotFound(new { message = "Project not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    // POST: api/projects/5/members
    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(int id, AddProjectMemberDto addMemberDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _projectService.AddMemberAsync(id, userId, addMemberDto);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/projects/5/members/3
    [HttpDelete("{id}/members/{memberUserId}")]
    public async Task<IActionResult> RemoveMember(int id, int memberUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _projectService.RemoveMemberAsync(id, userId, memberUserId);
            
            if (!success)
            {
                return NotFound(new { message = "Member not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
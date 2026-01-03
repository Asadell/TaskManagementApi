using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagementApi.Core.DTOs;
using TaskManagementApi.Services;

namespace TaskManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    // GET: api/tasks?projectId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] int? projectId)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            if (projectId.HasValue)
            {
                var tasks = await _taskService.GetProjectTasksAsync(projectId.Value, userId);
                return Ok(tasks);
            }
            else
            {
                var tasks = await _taskService.GetUserTasksAsync(userId);
                return Ok(tasks);
            }
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

    // GET: api/tasks/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            return Ok(task);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto createTaskDto)
    {
        if (string.IsNullOrWhiteSpace(createTaskDto.Title))
        {
            return BadRequest(new { message = "Task title is required" });
        }

        try
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.CreateTaskAsync(userId, createTaskDto);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
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

    // PUT: api/tasks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _taskService.UpdateTaskAsync(id, userId, updateTaskDto);
            
            if (!success)
            {
                return NotFound(new { message = "Task not found" });
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

    // DELETE: api/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _taskService.DeleteTaskAsync(id, userId);
            
            if (!success)
            {
                return NotFound(new { message = "Task not found" });
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
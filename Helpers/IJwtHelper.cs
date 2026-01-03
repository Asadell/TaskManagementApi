using TaskManagementApi.Core.Models;

namespace TaskManagementApi.Helpers;

public interface IJwtHelper
{
    string GenerateToken(User user);
}
using AutoMapper;
using TaskManagementApi.Core.DTOs;
using TaskManagementApi.Core.Models;
using TaskManagementApi.Core.Interfaces;
using TaskManagementApi.Helpers;
using BCrypt.Net;

namespace TaskManagementApi.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwtHelper _jwtHelper;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IJwtHelper jwtHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtHelper = jwtHelper;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
        {
            throw new Exception("Username already exists");
        }

        if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
        {
            throw new Exception("Email already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            FullName = registerDto.FullName,
            Role = "Member"
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtHelper.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.Username);
        
        if (user == null)
        {
            throw new Exception("Invalid username or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid username or password");
        }

        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtHelper.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }
}
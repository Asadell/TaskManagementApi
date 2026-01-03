using AutoMapper;
using TaskManagementApi.Core.Models;
using TaskManagementApi.Core.DTOs;

namespace TaskManagementApi.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>();

        // Project mappings
        // CreateMap<Project, ProjectDto>()
        //     .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Members.Count))
        //     .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.Tasks.Count))
        //     .ForMember(dest => dest.CompletedTaskCount, opt => opt.MapFrom(src => src.Tasks.Count(t => t.Status == "Done")));

        // CreateMap<Project, ProjectDetailDto>();
        // CreateMap<CreateProjectDto, Project>();
        // CreateMap<UpdateProjectDto, Project>()
        //     .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // // ProjectMember mappings
        // CreateMap<ProjectMember, ProjectMemberDto>();

        // // Task mappings
        // CreateMap<ProjectTask, TaskDto>()
        //     .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name));
        // CreateMap<CreateTaskDto, ProjectTask>();
        // CreateMap<UpdateTaskDto, ProjectTask>()
        //     .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}
using AutoMapper;
using CloudNet.Application.Features.Folders.Dtos;
using CloudNet.Domain.Storage;

namespace CloudNet.Application.Common.Mappings;

public sealed class FolderMappingProfile : Profile
{
    public FolderMappingProfile()
    {
        CreateMap<Folder, FolderDto>();
        CreateMap<CreateFolderDto, Folder>();
    }
}

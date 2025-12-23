using AutoMapper;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Domain.Storage;

namespace CloudNet.Application.Common.Mappings;

public sealed class FileEntryMappingProfile : Profile
{
    public FileEntryMappingProfile()
    {
        CreateMap<FileEntry, FileEntryDto>();
        CreateMap<CreateFileEntryDto, FileEntry>();
    }
}

using AutoMapper;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.ListDeleted;

public sealed class ListDeletedFilesQueryHandler
    : IRequestHandler<ListDeletedFilesQuery, IReadOnlyList<FileEntryDto>>
{
    private readonly IFileEntryRepository _files;
    private readonly IMapper _mapper;

    public ListDeletedFilesQueryHandler(IFileEntryRepository files, IMapper mapper)
    {
        _files = files;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FileEntryDto>> Handle(ListDeletedFilesQuery request, CancellationToken cancellationToken)
    {
        var items = await _files.ListDeletedAsync(request.OwnerId, cancellationToken);
        return items.Select(_mapper.Map<FileEntryDto>).ToList();
    }
}

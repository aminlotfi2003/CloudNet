using AutoMapper;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Features.Files.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Files.Queries.ListByFolder;

public sealed class ListFilesByFolderQueryHandler
    : IRequestHandler<ListFilesByFolderQuery, IReadOnlyList<FileEntryDto>>
{
    private readonly IFileEntryRepository _files;
    private readonly IMapper _mapper;

    public ListFilesByFolderQueryHandler(IFileEntryRepository files, IMapper mapper)
    {
        _files = files;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FileEntryDto>> Handle(ListFilesByFolderQuery request, CancellationToken cancellationToken)
    {
        var items = await _files.ListByFolderAsync(request.OwnerId, request.FolderId, cancellationToken);
        return items.Select(_mapper.Map<FileEntryDto>).ToList();
    }
}

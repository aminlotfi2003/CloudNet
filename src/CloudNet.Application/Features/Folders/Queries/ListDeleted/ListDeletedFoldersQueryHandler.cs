using AutoMapper;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Queries.ListDeleted;

public sealed class ListDeletedFoldersQueryHandler
    : IRequestHandler<ListDeletedFoldersQuery, IReadOnlyList<FolderDto>>
{
    private readonly IFolderRepository _folders;
    private readonly IMapper _mapper;

    public ListDeletedFoldersQueryHandler(IFolderRepository folders, IMapper mapper)
    {
        _folders = folders;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FolderDto>> Handle(ListDeletedFoldersQuery request, CancellationToken cancellationToken)
    {
        var items = await _folders.ListDeletedAsync(request.OwnerId, cancellationToken);
        return items.Select(_mapper.Map<FolderDto>).ToList();
    }
}

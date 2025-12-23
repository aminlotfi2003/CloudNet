using AutoMapper;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Features.Folders.Dtos;
using MediatR;

namespace CloudNet.Application.Features.Folders.Queries.ListChildren;

public sealed class ListFolderChildrenQueryHandler
    : IRequestHandler<ListFolderChildrenQuery, IReadOnlyList<FolderDto>>
{
    private readonly IFolderRepository _folders;
    private readonly IMapper _mapper;

    public ListFolderChildrenQueryHandler(IFolderRepository folders, IMapper mapper)
    {
        _folders = folders;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FolderDto>> Handle(ListFolderChildrenQuery request, CancellationToken cancellationToken)
    {
        var items = await _folders.ListChildrenAsync(request.OwnerId, request.ParentId, cancellationToken);
        return items.Select(_mapper.Map<FolderDto>).ToList();
    }
}

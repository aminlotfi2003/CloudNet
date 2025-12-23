using AutoMapper;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Features.Folders.Dtos;
using CloudNet.Domain.Storage;
using MediatR;

namespace CloudNet.Application.Features.Folders.Commands.CreateFolder;

public sealed class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, FolderDto>
{
    private readonly IFolderRepository _folders;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _clock;

    public CreateFolderCommandHandler(
        IFolderRepository folders,
        IUnitOfWork uow,
        IMapper mapper,
        IDateTimeProvider clock)
    {
        _folders = folders;
        _uow = uow;
        _mapper = mapper;
        _clock = clock;
    }

    public async Task<FolderDto> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var folder = _mapper.Map<Folder>(dto);
        folder.CreatedAt = _clock.UtcNow;
        folder.ModifiedAt = null;
        folder.IsDeleted = false;
        folder.DeletedAt = null;

        await _folders.AddAsync(folder, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FolderDto>(folder);
    }
}

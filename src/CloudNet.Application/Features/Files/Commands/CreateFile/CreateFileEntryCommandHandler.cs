using AutoMapper;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Features.Files.Dtos;
using CloudNet.Domain.Storage;
using MediatR;

namespace CloudNet.Application.Features.Files.Commands.CreateFile;

public sealed class CreateFileEntryCommandHandler : IRequestHandler<CreateFileEntryCommand, FileEntryDto>
{
    private readonly IFileEntryRepository _files;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _clock;

    public CreateFileEntryCommandHandler(
        IFileEntryRepository files,
        IUnitOfWork uow,
        IMapper mapper,
        IDateTimeProvider clock)
    {
        _files = files;
        _uow = uow;
        _mapper = mapper;
        _clock = clock;
    }

    public async Task<FileEntryDto> Handle(CreateFileEntryCommand request, CancellationToken cancellationToken)
    {
        var file = _mapper.Map<FileEntry>(request.Dto);

        file.CreatedAt = _clock.UtcNow;
        file.ModifiedAt = null;
        file.IsDeleted = false;
        file.DeletedAt = null;

        await _files.AddAsync(file, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FileEntryDto>(file);
    }
}

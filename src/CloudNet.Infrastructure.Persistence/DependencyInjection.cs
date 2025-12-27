using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Abstractions.Storage;
using CloudNet.Infrastructure.Persistence.Clock;
using CloudNet.Infrastructure.Persistence.Context;
using CloudNet.Infrastructure.Persistence.Options;
using CloudNet.Infrastructure.Persistence.Repositories;
using CloudNet.Infrastructure.Persistence.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CloudNet.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CloudNetDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"));
        });

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        // Repositories
        services.AddScoped<IFolderRepository, FolderRepository>();
        services.AddScoped<IFileEntryRepository, FileEntryRepository>();
        services.AddScoped<IStorageQuotaRepository, StorageQuotaRepository>();
        services.AddScoped<IShareLinkRepository, ShareLinkRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();
        services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();

        // Clock
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // Storage
        services.AddOptions<FileStorageOptions>()
             .Bind(configuration.GetSection(FileStorageOptions.SectionName));

        services.AddOptions<StorageQuotaOptions>()
            .Bind(configuration.GetSection(StorageQuotaOptions.SectionName));

        services.AddSingleton<IFileStorage, LocalFileStorage>();
        services.AddSingleton<IStorageQuotaSettings, StorageQuotaSettings>();

        return services;
    }
}

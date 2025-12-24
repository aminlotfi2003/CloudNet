using CloudNet.Application.Common.Abstractions.Auth;
using CloudNet.Application.Common.Abstractions.Clock;
using CloudNet.Application.Common.Abstractions.Persistence.Repositories;
using CloudNet.Application.Common.Abstractions.Persistence.UnitOfWork;
using CloudNet.Application.Common.Exceptions;
using CloudNet.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CloudNet.Infrastructure.Identity.Services;

public class PasswordPolicyService : IPasswordPolicyService
{
    private readonly IPasswordHistoryRepository _passwordHistory;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IDateTimeProvider _clock;
    private readonly IUnitOfWork _unitOfWork;

    private const int HistoryLength = 5;

    public PasswordPolicyService(
        IPasswordHistoryRepository historyRepository,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IDateTimeProvider clock,
        IUnitOfWork unitOfWork)
    {
        _passwordHistory = historyRepository;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _unitOfWork = unitOfWork;
    }

    public async Task EnsurePasswordCompliesAsync(ApplicationUser user, string newPassword, CancellationToken cancellationToken = default)
    {
        var recent = await _passwordHistory.GetRecentAsync(user.Id, HistoryLength, cancellationToken);

        foreach (var history in recent)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, history.PasswordHash, newPassword);
            if (result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded)
                throw new BusinessRuleViolationException("New password cannot match any of the last 5 passwords.");
        }
    }

    public async Task RecordPasswordChangeAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken = default)
    {
        await _passwordHistory.AddAsync(PasswordHistory.Create(user.Id, passwordHash, _clock.UtcNow), cancellationToken);
        await _passwordHistory.PruneExcessAsync(user.Id, HistoryLength, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public bool IsPasswordExpired(ApplicationUser user, DateTimeOffset utcNow, int days = 90)
    {
        var lastChanged = user.PasswordLastChangedAt ?? user.CreatedAt;
        return lastChanged.AddDays(days) <= utcNow;
    }
}

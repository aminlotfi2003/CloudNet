namespace CloudNet.Application.Common.Models;

public sealed class PageRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public int Skip => (Page - 1) * PageSize;

    public void Validate()
    {
        if (Page < 1) throw new ArgumentOutOfRangeException(nameof(Page), "Page must be >= 1.");
        if (PageSize < 1 || PageSize > 200) throw new ArgumentOutOfRangeException(nameof(PageSize), "PageSize must be between 1 and 200.");
    }
}

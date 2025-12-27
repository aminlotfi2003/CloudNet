namespace CloudNet.Api.Abstractions.Constants;

public static class FileUploadConstants
{
    // Ensure reverse proxies (e.g. Nginx client_max_body_size) match this limit.
    public const long MaxUploadSizeBytes = 100 * 1024 * 1024;
}

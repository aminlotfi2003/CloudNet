namespace CloudNet.Application.Features.Auth.Dtos;

public sealed record AuthResponseDto(
    AuthUserDto User,
    AuthTokensDto Tokens);

namespace Dynamiq.Application.DTOs.AuthDTOs
{
    public record LoginResultDto(
        string AccessToken, 
        string RefreshToken, 
        string Email, 
        string Provider, 
        string ProviderUserId
    );
}

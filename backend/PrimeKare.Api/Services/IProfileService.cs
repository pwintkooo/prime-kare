using PrimeKare.Api.DTOs.Profile;

namespace PrimeKare.Api.Services;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync();

    Task UpdateProfileAsync(UpdateProfileDto dto);

    Task ChangePasswordAsync(ChangePasswordDto dto);
}
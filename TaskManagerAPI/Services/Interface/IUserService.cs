using TaskManagerAPI.Dtos;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interface
{ 
    public record UserResult(
        bool Success,
        string Message,
        string? Token = null
    );
    public interface IUserService
    {
        Task<UserResult> RegisterAsync(RegisterDto dto);
        Task<UserResult> LoginAsync(LoginDto dto);
        Task<UserResult> ChangePasswordAsync(int id, ChangePasswordDto dto);
        Task<UserResult> DeleteAsync(int id);
    }
}

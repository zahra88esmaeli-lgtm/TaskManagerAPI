using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services.Interface
{
    public interface ITokenService
    {
     string createToken(User user);
    }
}

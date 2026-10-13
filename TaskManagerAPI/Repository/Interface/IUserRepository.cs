using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repository.Interface
{
    public interface IUserRepository
    {
        Task<bool> UsernameExistsAsync(string username);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task AddAsync(User user);
        void Delete(User user);
        Task SaveChangesAsync();
    }
}

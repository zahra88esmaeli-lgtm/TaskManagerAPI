using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repository.Interface;

namespace TaskManagerAPI.Repository
{
    
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _dbContext;

        public UserRepository(AppDBContext dbContext) => _dbContext = dbContext;
       
        public async Task AddAsync(User user) =>
            await _dbContext.Users.AddAsync(user);


        public  void Delete(User user) =>
            _dbContext.Users.Remove(user);
       
        public  Task<User?> GetByIdAsync(int id)=>
             _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
      

        public  Task<bool> UsernameExistsAsync(string username) => 
            _dbContext.Users.AnyAsync(u => u.Username == username);
       
        public  Task<User?> GetByUsernameAsync(string username)=>
             _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
       

        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();

    }
}

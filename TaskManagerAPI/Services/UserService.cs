using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Dtos;
using TaskManagerAPI.Repository.Interface;
using TaskManagerAPI.Services.Interface;
using TaskManagerAPI.Services;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<UserResult> ChangePasswordAsync(int id, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return new UserResult(false, "User not found");
            if (!PasswordHasher.VerifyPassword(dto.CurrentPassword, user.HashPassword))
                return new UserResult(false, "Current password is wrong");
            user.HashPassword = PasswordHasher.HashPassword(dto.NewPassword);
        
            await _userRepository.SaveChangesAsync();
            return new UserResult(true, "Update password successful");
        }

        public async Task<UserResult> DeleteAsync(int id)
        {
            var user =await _userRepository.GetByIdAsync(id);
            if (user == null)
              return   new UserResult(false, "User not found");
           _userRepository.Delete(user);
          await  _userRepository.SaveChangesAsync();
           return new UserResult(true, "Delete Successful");
        }

        public async Task<UserResult> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.HashPassword))
                return new UserResult(false, "Invalid username Or password");
            var token = _tokenService.createToken(user);
            return new UserResult(true, "Login Successful", token);
                              
           
        }

        public async Task<UserResult> RegisterAsync(RegisterDto dto)
        {
           if(await _userRepository.UsernameExistsAsync(dto.Username))
           {
                return new UserResult(false, "Username already exists.");
             
            }
           
           var user = new User
                {
                    Username = dto.Username,
                    HashPassword = PasswordHasher.HashPassword(dto.Password)
                };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return new UserResult( true,"User registered successful.");
            


        }
    }
}

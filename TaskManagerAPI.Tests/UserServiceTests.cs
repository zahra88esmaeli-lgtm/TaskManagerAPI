using Microsoft.AspNetCore.Http;
using Moq;
using TaskManagerAPI.Dtos;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repository.Interface;
using TaskManagerAPI.Services;
using TaskManagerAPI.Services.Interface;

namespace TaskManagerAPI.Tests
{
    public class UserServiceTests
    {
        //Register test
        [Fact]
        public async Task RegisterAsync_WhenUsernameIsNew_SavesUserAndReturnsSuccess()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            userRepoMock
                .Setup(r => r.UsernameExistsAsync("niki"))
                .ReturnsAsync(false);

            var service = new UserService(userRepoMock.Object, tokenServiceMock.Object);
            var dto = new RegisterDto
            {
                Username = "niki",
                Password = "123456"
            };

            var result = await service.RegisterAsync(dto);

            Assert.True(result.Success);
            userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
                    u.Username == "niki" &&
                    u.HashPassword != "123456")), Times.Once);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task RegisterAsync_WhenUsernameExist_ReturnsFailure()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            var service = new Services.UserService(userRepoMock.Object, tokenServiceMock.Object);
            userRepoMock
                .Setup(r => r.UsernameExistsAsync("niki"))
                .ReturnsAsync(true);
            var dto = new RegisterDto
            {
                Username = "niki",
                Password = "123456"
            };
            var result = await service.RegisterAsync(dto);
            Assert.False(result.Success);
            userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        
        //Login test
       
        [Fact]
        public async Task LoginAsync_WhenPasswordIsCorrect_ReturnsSuccessWithToken()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            var service = new UserService(userRepoMock.Object, tokenServiceMock.Object);
            var user = new User
            {
                Username = "niki",
                HashPassword = PasswordHasher.HashPassword("123456")
            };

            userRepoMock
                .Setup(r => r.GetByUsernameAsync("niki"))
                .ReturnsAsync(user);
            tokenServiceMock
                .Setup(t => t.createToken(user))
                .Returns("fake-token");
            var dto = new LoginDto
            {
                Username = "niki",
                Password = "123456"
            };

            var result = await service.LoginAsync(dto);

            Assert.True(result.Success);
            Assert.Equal("fake-token", result.Token);
            tokenServiceMock.Verify(t => t.createToken(user), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ReturnsFailure()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            userRepoMock
                .Setup(r => r.GetByUsernameAsync("ghost"))
                .ReturnsAsync((User?)null);

            var dto = new LoginDto
            {
                Username = "ghost",
                Password = "123456"
            };

            var service = new UserService(userRepoMock.Object, tokenServiceMock.Object);

            var result = await service.LoginAsync(dto);

            Assert.False(result.Success);
            tokenServiceMock.Verify(t => t.createToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIswrong_ReturnsFailure()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenServiceMock = new Mock<ITokenService>();

            var service = new UserService(userRepoMock.Object, tokenServiceMock.Object);

            var user = new User
            {
                Username = "niki",
                HashPassword = PasswordHasher.HashPassword("123456")
            };
            userRepoMock
                .Setup(r => r.GetByUsernameAsync("niki"))
                .ReturnsAsync(user);
            var dto = new LoginDto
            {
                Username = "niki",
                Password = "985741"
            };
            var result = await service.LoginAsync(dto);
            Assert.False(result.Success);
            tokenServiceMock.Verify(t => t.createToken(It.IsAny<User>()), Times.Never);
        }

        //Change Password test
        [Fact]
        public async Task ChangePasswordAsync_WhenCurrentPasswordIsCorrect_SavesTheNewPassword()
        {
            var userRepoMock =new Mock<IUserRepository>();
            var tokenServiceMock = new Mock<ITokenService>();
            var user = new User
            {
                Id = 1,
                Username = "Niki",
                HashPassword = PasswordHasher.HashPassword("123456")
               
            };
            userRepoMock
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

            var dto = new ChangePasswordDto
            { CurrentPassword = "123456",
                NewPassword = "9874"
            };
          
            var service = new UserService(userRepoMock.Object, tokenServiceMock.Object);
            var result=await  service.ChangePasswordAsync(user.Id, dto);
            Assert.True(result.Success);
            Assert.True(PasswordHasher.VerifyPassword("9874", user.HashPassword));
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);


        }
        [Fact]
        public async Task ChangePasswordAsync_WhenCurrentPasswordIsWrong_ReturnsFailure()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenSericeMock = new Mock<ITokenService>();
            var user = new User
            {
                Id = 1,
                Username = "Niki",
                HashPassword = PasswordHasher.HashPassword("12345")
            };
            var service = new UserService(userRepoMock.Object, tokenSericeMock.Object);
            var dto = new ChangePasswordDto
            {
                CurrentPassword = "4563",
                NewPassword = "9874"
            };
            userRepoMock
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);
            var result = await service.ChangePasswordAsync(user.Id, dto);
            Assert.False(result.Success);
            Assert.True(PasswordHasher.VerifyPassword("12345", user.HashPassword));
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        [Fact]
        public async Task ChangePasswordAsync_WhenUserNotFound_ReturnsFailure()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenService = new Mock<ITokenService>();

            var service = new Services.UserService(userRepoMock.Object, tokenService.Object);
            var dto = new ChangePasswordDto
            {
                CurrentPassword = "12345",
                NewPassword = "98741"
            };
            userRepoMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((User?)null);
            var result = await service.ChangePasswordAsync(99, dto);
            Assert.False(result.Success);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);

        }

        //Delete test
        [Fact]
        public async Task Delete_WhenUserExists_DeletesUserAndSavesChanges()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var TokenService = new Mock<ITokenService>();

            var service = new Services.UserService(userRepoMock.Object, TokenService.Object);
            var user = new User
            {
                Id = 1
            };
           
            userRepoMock
                 .Setup(r => r.GetByIdAsync(1))
                 .ReturnsAsync(user);
            var result = await service.DeleteAsync(1);
            Assert.True(result.Success);
            userRepoMock.Verify(r => r.Delete(user), Times.Once);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

        
        }
        [Fact]
        public async Task DeleteAsync_WhenUserNotFound_ReturnsFailure()
        {
            var userRepoMock = new Mock<IUserRepository>();
            var tokenService = new Mock<ITokenService>();
            var service = new Services.UserService(userRepoMock.Object, tokenService.Object);
            userRepoMock
                .Setup(r => r.GetByIdAsync(9))
                .ReturnsAsync((User?)null);
            var result = await service.DeleteAsync(9);
            Assert.False(result.Success);
            userRepoMock.Verify(r => r.Delete(It.IsAny<User>()), Times.Never);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never());
        }
    }
}

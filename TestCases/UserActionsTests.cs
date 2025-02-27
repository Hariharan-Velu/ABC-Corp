using Xunit;
using DomainLayer.Entities;
using DomainLayer.Interfaces;
using Moq;
using ABC_Corp.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace TestCases
{
    public class UserActionsTests
    {
        [Fact]
        public async Task Execute_Returns_User_For_Valid_Id()
        {
            var mockUserRepository = new Mock<IUserRepo>();
            var user = new UserDetails { UserId = 1, UserName = "John Doe", UserEmail = "john.doe@abccorp.com", UserDOB = new DateTime(1999, 2, 26), Role = "Admin" };
            var responseFormattedUser = new ResponseFormatter<UserDetails>
            {
                Data = user, 
                Success = true,
                Message = "User found"
            };
            mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1)).ReturnsAsync(responseFormattedUser);
            var getUserById = new UserController(Mock.Of<ILogger<UserController>>(),mockUserRepository.Object);
            var result = await getUserById.GetMyDetails(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDetails>(okResult.Value);
            Assert.Equal("John Doe", returnedUser.UserName);
        }
    }
}

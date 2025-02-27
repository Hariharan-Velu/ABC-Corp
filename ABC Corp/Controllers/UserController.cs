using DomainLayer.Entities;
using DomainLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ABC_Corp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepository;
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger, IUserRepo userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDetails user)
        {
            _logger.LogInformation("Create user started for : " + user.UserName);
            var response = await _userRepository.CreateUserAsync(user);
            if (response.Success)
                return Ok(response.Message);
            else return BadRequest(response.Message);
        }

        [Authorize]
        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetch all users started: " + DateTime.Now);
            var response = await _userRepository.GetAllUsersAsync();
            if (response.Success)
                return Ok(response.Data);
            else return BadRequest(response.Data);

        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost("updatePersonalDetails")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] UserDetails user)
        {
            _logger.LogInformation("Update user started for id: " + user.UserName);
            var response = await _userRepository.UpdatePersonalDetails(user);
            if (response.Success)
                return Ok(response.Message);
            else return BadRequest(response.Message);
        }

        [HttpGet("myPersonalDetails")]
        public async Task<IActionResult> GetMyDetails(int id)
        {
            _logger.LogInformation("Get Details for user id: " + id);
            var userId = id;
            var response = await _userRepository.GetUserByIdAsync(userId); // Fetch tasks for the employee
            if (response.Success)
            {
                if (response.Data == null)
                    return Ok("No User found");
                else
                    return Ok(response.Data);
            }
            else return BadRequest(response.Message);
        }
    }
}

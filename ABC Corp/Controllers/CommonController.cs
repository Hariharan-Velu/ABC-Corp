//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using DomainLayer.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using DomainLayer.Entities;

//namespace ABC_Corp.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CommonController : ControllerBase
//    {
//        private readonly ITaskRepo _taskRepository;
//        private readonly IUserRepo _userRepository;
//        private readonly ILogger<CommonController> _logger;
//        public CommonController(ITaskRepo taskRepository,ILogger<CommonController> logger, IUserRepo userRepository)
//        {
//            _taskRepository = taskRepository;
//            _logger = logger;
//            _userRepository = userRepository;
//        }

//        //[Authorize(Policy = "AdminOnly")]
//        [HttpGet("allTasks")]
//        public async Task<IActionResult> GetAllTasks()
//        {
//            var tasks = await _taskRepository.GetAllTasksAsync();
//            return Ok(tasks);
//        }

//        //[Authorize(Policy = "ManagerOnly")]
//        //[HttpGet("teamTasks")]
//        //public async Task<IActionResult> GetTeamTasks()
//        //{
//        //    var userId = int.Parse(User.FindFirst("sub")?.Value); // Get current user ID
//        //    var tasks = await _taskRepository.GetTasksForTeamAsync(userId); // Fetch tasks for the manager's team
//        //    return Ok(tasks);
//        //}

//        //[Authorize(Policy = "EmployeeOnly")]
//        [HttpGet("myTasks")]
//        public async Task<IActionResult> GetMyTasks(int id)
//        {
//            var userId = id;//int.Parse(User.FindFirst("sub")?.Value);
//            var tasks = await _taskRepository.GetTasksForUserAsync(userId); // Fetch tasks for the employee
//            return Ok(tasks);
//        }

//        //[Authorize(Policy = "AdminOnly")]
//        [HttpPost("createTask")]
//        public async Task<IActionResult> CreateTask([FromBody] TaskDetails task)
//        {
//            var response = await _taskRepository.CreateTaskAsync(task);
//            if (response.Success)
//                 return CreatedAtAction("createTask", response.Message);
//            else return BadRequest(response.Message);
//        }

//        [HttpPost("updateTaskStatus")]
//        public async Task<IActionResult> UpdateTaskStatus([FromBody] TaskDetails task)
//        {
//            var response = await _taskRepository.UpdateTaskAsync(task);
//            if (response.Success)
//                return CreatedAtAction("createTask", response.Message);
//            else return BadRequest(response.Message);
//        }

//        [HttpPost("createUser")]
//        public async Task<IActionResult> CreateUser([FromBody] UserDetails user)
//        {
//            await _userRepository.CreateUserAsync(user);
//            return Ok();
//        }

//        [HttpGet("allUsers")]
//        public async Task<IActionResult> GetAllUsers()
//        {
//            var tasks = await _userRepository.GetAllUsersAsync();
//            return Ok(tasks);
//        }

//        [HttpPost("updatePersonalDetails")]
//        public async Task<IActionResult> UpdateUserDetails([FromBody] UserDetails user)
//        {
//            var response = await _userRepository.UpdatePersonalDetails(user);
//            if (response.Success)
//                 return Ok( response.Message);
//            else return BadRequest(response.Message);
//        }

//        [HttpGet("myPersonalDetails")]
//        public async Task<IActionResult> GetMyDetails(int id)
//        {
//            var userId = id;//int.Parse(User.FindFirst("sub")?.Value);
//            var tasks = await _userRepository.GetUserByIdAsync(userId); // Fetch tasks for the employee
//            return Ok(tasks);
//        }


//        //[Authorize(Policy = "ManagerOnly")]
//        //[HttpPost("assign")]
//        //public async Task<IActionResult> AssignTask([FromBody] Task task)
//        //{
//        //    // Ensure the task is assigned to a team member
//        //    await _createTask.Execute(task);
//        //    return Ok();
//        //}
//    }
//}

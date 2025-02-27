using DomainLayer.Entities;
using DomainLayer.Interfaces;
using InfraLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ABC_Corp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepo _taskRepository;
        private readonly ILogger<TaskController> _logger;
        private readonly BlobService _blobService;
        public TaskController(ITaskRepo taskRepository, ILogger<TaskController> logger, BlobService blobService)
        {
            _taskRepository = taskRepository;
            _logger = logger;
            _blobService = blobService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("allTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            _logger.LogInformation("Get all tasks started at : " + DateTime.Now);
            var response = await _taskRepository.GetAllTasksAsync();
            if (response.Success)
                return Ok(response.Data);
            else return BadRequest(response.Data);
        }

        [HttpGet("myTasks")]
        public async Task<IActionResult> GetMyTasks(int id)
        {
            _logger.LogInformation("Get tasks for " + id + "started at : " + DateTime.Now);
            var response = await _taskRepository.GetTasksForUserAsync(id);
            if (response.Success)
                return Ok(response.Data);
            else return BadRequest(response.Data);
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("createTask")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDetails task)
        {
            _logger.LogInformation("Create task started at : " + DateTime.Now);
            var response = await _taskRepository.CreateTaskAsync(task);
            if (response.Success)
                return CreatedAtAction("createTask", response.Message);
            else return BadRequest(response.Message);
        }

        [Authorize(Roles = "Manager,Employee")]
        [HttpPost("updateTaskStatus")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] TaskDetails task)
        {
            _logger.LogInformation("Update task for " + task.TaskId + "started at : " + DateTime.Now);
            var response = await _taskRepository.UpdateTaskAsync(task);
            if (response.Success)
                return Ok(response.Message);
            else return BadRequest(response.Message);
        }

        [HttpGet("getTaskbyID")]
        public async Task<IActionResult> GetTaskByID(int id)
        {
            _logger.LogInformation("Get tasks " + id + "started at : " + DateTime.Now);
            var response = await _taskRepository.GetTaskByIdAsync(id);
            if (response.Success)
                return Ok(response.Data);
            else return BadRequest(response.Data);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            _logger.LogInformation("File upload started for : "+ file.FileName);
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Invalid file");
                return BadRequest("Invalid file");
            }
            var blobName = Guid.NewGuid().ToString() + "_" + file.FileName;
            using (var stream = file.OpenReadStream())
            {
                await _blobService.UploadFileAsync(blobName, stream);
            }
            _logger.LogInformation("File uploaded successfully "+file.FileName);
            return Ok(new { blobName });
        }

        [Authorize(Roles = "Employee")]
        [HttpGet("download/{blobName}")]
        public async Task<IActionResult> DownloadFile(string blobName)
        {
            _logger.LogInformation("File download started for : " + blobName);
            var fileStream = await _blobService.DownloadFileAsync(blobName);
            return File(fileStream, "application/octet-stream", blobName);
        }
    }
}

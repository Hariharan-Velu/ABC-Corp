using InfraLayer.DB_Layer;
using DomainLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;

namespace InfraLayer.Repository
{
    public class TaskRepository : ITaskRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskRepository> _logger;
        public TaskRepository(AppDbContext context,ILogger<TaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseFormatter<TaskDetails>> GetTaskByIdAsync(int id)
        {
            string responseMessage = ""; bool successFlag = false; TaskDetails taskDetails = null;
            try
            {
                taskDetails = await _context.Tasks.FindAsync(id);
                if (taskDetails == null)
                    _logger.LogInformation("No Task found for "+ id.ToString());
                successFlag = true;
                responseMessage = "";
            }
            catch (Exception ex)
            {
                successFlag = false;
                responseMessage = "";
                _logger.LogError("Exception in Task fetch "+ex.Message);
            }
            return new ResponseFormatter<TaskDetails>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = (taskDetails == null) ? new TaskDetails() : taskDetails
            };
          

        }

        public async Task<ResponseFormatter<List<TaskDetails>>> GetAllTasksAsync()
        {
            string responseMessage = ""; bool successFlag = false; List<TaskDetails> dbData = null;
            try
            {
                dbData = await _context.Tasks.AsNoTracking().ToListAsync();
                if (dbData == null)
                    _logger.LogInformation("No Tasks Found");
                successFlag = true;
                responseMessage = "";
            }
            catch (Exception ex)
            {
                successFlag = false;
                responseMessage = "";
                dbData = Enumerable.Empty<TaskDetails>().ToList();
                _logger.LogError("Exception occurred in AllTasks fetch " + ex.Message);
            }
            return new ResponseFormatter<List<TaskDetails>>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = dbData
            };
        }

        public async Task<ResponseFormatter<string>> CreateTaskAsync(TaskDetails task)
        {
            string responseMessage = ""; bool successFlag = false;
            try
            {
               
                var userRole = _context.Users.Where(t => t.UserId == task.TaskAssignedToUserId).Select(t => new { t.Role, t.UserId }).FirstOrDefault();
                responseMessage = userRole == null ? "No User Found!!" :
                    (userRole.Role == null || userRole.Role.ToUpper() != "EMPLOYEE") ? "Not authorised to assign tasks" : "";
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    if (responseMessage == "")
                    {
                        _context.Tasks.Add(task);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        successFlag = true;
                        responseMessage = "Task Created Successfully";
                    }
                    else
                    {
                        successFlag = false;
                    }
                }
                _logger.LogInformation(responseMessage);
                
            }
            catch (Exception ex)
            {
                successFlag = false; responseMessage = ex.Message;
                if (_context.Database.CurrentTransaction != null)
                {
                    await _context.Database.CurrentTransaction.RollbackAsync();
                }
                _logger.LogError("Exception in Create Task : " + responseMessage);
            }
            return new ResponseFormatter<string>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = ""
            };
        }

        public async Task<ResponseFormatter<string>> UpdateTaskAsync(TaskDetails task)
        {
            string responseMessage = ""; bool successFlag = false;
            try
            {
                using (var transaction =await _context.Database.BeginTransactionAsync())
                {
                    _context.Tasks.Update(task);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    successFlag = true;
                    responseMessage = "Update Successfull";
                }
                _logger.LogInformation(responseMessage);
            }
            catch (Exception ex)
            {
                successFlag = false; responseMessage = ex.Message;
                if (_context.Database.CurrentTransaction != null)
                {
                    await _context.Database.CurrentTransaction.RollbackAsync();
                }
                _logger.LogError("Exception in Update Task : " + responseMessage);
            }
            return new ResponseFormatter<string>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = ""
            };
        }

        //public async Task DeleteTaskAsync(int id)
        //{
        //    var task = await _context.Tasks.FindAsync(id);
        //    if (task != null)
        //    {
        //        _context.Tasks.Remove(task);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        public async Task<ResponseFormatter<List<TaskDetails>>> GetTasksForUserAsync(int userId)
        {
            string responseMessage = ""; bool successFlag = false; List<TaskDetails> dbData = null;
            try
            {
                dbData = await _context.Tasks.AsNoTracking().Where(t => t.TaskAssignedToUserId == userId).ToListAsync();
                if (dbData == null)
                    _logger.LogInformation("No Tasks Found for user: " + userId.ToString());
                successFlag = true;
                responseMessage = "";
                _logger.LogInformation("Tasks fetched successfully for user: " + userId.ToString());
            }
            catch (Exception ex)
            {
                successFlag = false;
                responseMessage = "";
                dbData = Enumerable.Empty<TaskDetails>().ToList();
                _logger.LogError("Exception occurred in TaskFetch " + ex.Message);
            }
            return new ResponseFormatter<List<TaskDetails>>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = dbData
            };
           
        }

    }
}

using DomainLayer.Entities;
using InfraLayer.DB_Layer;
using InfraLayer.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InfraLayer.Repository
{
    public class CacheImplementation
    {
        private readonly IDistributedCache _cache;
        private readonly AppDbContext _context;
        private readonly ILogger<CacheImplementation> _logger;
        public CacheImplementation(AppDbContext context, IDistributedCache cache,ILogger<CacheImplementation> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }
        public async Task<ResponseFormatter<TaskDetails>> GetTaskByIdAsync(int id)
        {
            string responseMessage = ""; bool successFlag = false;
            try
            {
                string cacheKey = $"Task_{id}";

                var task = await RedisPolicies.FallbackPolicy(_logger)
                    .WrapAsync(RedisPolicies.RetryPolicy)
                    .ExecuteAsync(async () =>
                    {
                        var cachedData = await _cache.GetStringAsync(cacheKey);
                        if (cachedData != null)
                        {
                            return new ResponseFormatter<TaskDetails>
                            {
                                Success = true,
                                Message = "",
                                Data = JsonConvert.DeserializeObject<TaskDetails>(cachedData)
                            }; 
                           
                        }
                         var taskFromDb = await _context.Tasks.FindAsync(id);
                        if (taskFromDb != null)
                        {
                            await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(taskFromDb), new DistributedCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                            });
                        }
                        return new ResponseFormatter<TaskDetails>
                        {
                            Success = true,
                            Message = "",
                            Data = taskFromDb
                        };
                    });
            }
            catch (Exception ex)
            {
                successFlag = false;
                responseMessage = "";
                _logger.LogError("Exception in Task fetch " + ex.Message);
                return new ResponseFormatter<TaskDetails>
                {
                    Success = successFlag,
                    Message = "",
                    Data = new TaskDetails()
                };
            }
            return new ResponseFormatter<TaskDetails>
            {
                Success = successFlag,
                Message = "",
                Data = new TaskDetails()
            };

        }

        public async Task<ResponseFormatter<string>> CreateTaskAsync(TaskDetails task)
        {
            string responseMessage = ""; bool successFlag = false;
            try
            {
                var userRole = _context.Users.Where(t => t.UserId == task.TaskAssignedToUserId).Select(t => new { t.Role, t.UserId }).FirstOrDefault();
                responseMessage = userRole.UserId == null ? "No User Found!!" :
                    (userRole.Role == null || userRole.Role.ToUpper() != "EMPLOYEE") ? "Not authorised to assign tasks" : "";
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    if (responseMessage == "")
                    {
                        _context.Tasks.Add(task);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        string cacheKey = $"Task_{task.TaskId}";
                        await RedisPolicies.RetryPolicy.ExecuteAsync(async () =>
                        {
                            await _cache.RemoveAsync(cacheKey);
                        });
                        successFlag = true;
                        responseMessage = "Task Created Successfully";
                    }
                    else
                    {
                        successFlag = false;
                    }
                }

            }
            catch (Exception ex)
            {
                successFlag = false; responseMessage = ex.Message;
                if (_context.Database.CurrentTransaction != null)
                {
                   await _context.Database.CurrentTransaction.RollbackAsync();
                }

            }
            return new ResponseFormatter<string>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = ""
            };
        }

    }
}

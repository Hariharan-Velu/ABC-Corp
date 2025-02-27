using InfraLayer.DB_Layer;
using DomainLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainLayer.Entities;
using Microsoft.Extensions.Logging;


namespace InfraLayer.Repository
{
    public class UserRepository : IUserRepo
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseFormatter<UserDetails>> GetUserByIdAsync(int id)
        {
            string responseMessage = ""; bool successFlag = false; UserDetails userDetails = null;
            try
            { 
                userDetails = await _context.Users.FindAsync(id);
                if (userDetails == null)
                    _logger.LogInformation("No User found for id: " + id.ToString());
                successFlag = true;
                responseMessage = "";
            }
            catch(Exception ex)
            {
                _logger.LogError("Error occurred for " + id + ex.Message);
                successFlag = false;
                responseMessage = "";
            }
            return new ResponseFormatter<UserDetails>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = (userDetails == null) ? new UserDetails() : userDetails 
            };
        }

        public async Task<ResponseFormatter<List<UserDetails>>> GetAllUsersAsync()
        {
            string responseMessage = ""; bool successFlag = false; List<UserDetails> dbData = null;
            try
            {
                dbData = await _context.Users.AsNoTracking().ToListAsync();
                if (dbData == null)
                    _logger.LogInformation("No Users found" );
                successFlag = true;
                responseMessage = "";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred in User fetch "+ ex.Message);
                successFlag = false;
                responseMessage = "";
                dbData = Enumerable.Empty<UserDetails>().ToList();
            }
            return new ResponseFormatter<List<UserDetails>>
            {
                Success = successFlag,
                Message = responseMessage,
                Data = dbData
            };
        }

        public async Task<ResponseFormatter<string>> CreateUserAsync(UserDetails user)
        {
            string responseMessage = ""; bool successFlag = false;

            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    successFlag = true;
                    responseMessage = "User created successfully";
                    _logger.LogInformation(responseMessage);
                }
            }
            catch (Exception ex)
            {
                successFlag = false; responseMessage = ex.Message;
                _logger.LogError("Transaction rolled back on User creation " + responseMessage);
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

        public async Task<ResponseFormatter<string>> UpdatePersonalDetails(UserDetails user)
        {
            string responseMessage = ""; bool successFlag = false;
            try
            {
                var dbRecord = await _context.Users.FirstOrDefaultAsync(t => t.UserId == user.UserId);
                if (dbRecord == null)
                {
                    responseMessage = "User Not Found";
                    successFlag = false;
                    _logger.LogInformation("Update personal details: " + responseMessage);
                }
                else
                {
                    try
                    {
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            dbRecord.UserName = user.UserName;
                            dbRecord.Role = user.Role;
                            dbRecord.UserEmail = user.UserEmail;
                            dbRecord.UserDOB = user.UserDOB;
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            successFlag = true;
                            responseMessage = "Update Successfull";

                        }
                    }
                    catch (Exception ex)
                    {
                        if (_context.Database.CurrentTransaction != null)
                        {
                            await _context.Database.CurrentTransaction.RollbackAsync();
                        }
                        successFlag = false;
                        responseMessage = "Transaction failed";
                        _logger.LogError("Update user transaction rollback " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                successFlag = false; responseMessage = ex.Message;
                _logger.LogError("Update user exception occurred " + responseMessage);
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

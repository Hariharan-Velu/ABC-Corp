using DomainLayer.Entities;


namespace DomainLayer.Interfaces
{
    public interface IUserRepo
    {
        Task<ResponseFormatter<UserDetails>> GetUserByIdAsync(int id);
        Task<ResponseFormatter<List<UserDetails>>> GetAllUsersAsync();
        Task<ResponseFormatter<string>> CreateUserAsync(UserDetails user);
        Task<ResponseFormatter<string>> UpdatePersonalDetails(UserDetails user);
       
    }
}

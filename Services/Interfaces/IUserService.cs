using backend.Domain;

namespace backend.Services.Interfaces;

public interface IUserService
{
        
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    
}
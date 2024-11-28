using backend.Domain;
using backend.Repo;
using backend.Services.Interfaces;

namespace backend.Services;


    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetAllUsersAsync() => await _userRepository.GetAllUsersAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await _userRepository.GetUserByIdAsync(id);

        public async Task<bool> UpdateUserAsync(User user)
        {
            var exists = await _userRepository.UserExistsAsync(user.id);
            if (!exists) return false;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var exists = await _userRepository.UserExistsAsync(id);
            if (!exists) return false;

            await _userRepository.DeleteUserAsync(id);
            return true;
        }
    }

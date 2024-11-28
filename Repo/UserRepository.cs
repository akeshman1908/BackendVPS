using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain;
using Microsoft.EntityFrameworkCore;
using backend.Repo.DbContext; // Zorg dat je een User-model hebt

namespace backend.Repo
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Haal alle gebruikers op
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Haal een specifieke gebruiker op op basis van ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Voeg een nieuwe gebruiker toe
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Update een bestaande gebruiker
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Verwijder een gebruiker op basis van ID
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Controleer of een gebruiker bestaat op basis van ID
        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.id == id);
        }
    }
}
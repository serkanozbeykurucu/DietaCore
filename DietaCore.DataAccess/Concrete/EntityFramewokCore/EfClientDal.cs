using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DietaCore.DataAccess.Concrete.EntityFramewokCore
{
    public class EfClientDal : GenericRepository<Client>, IClientDal
    {
        private readonly DietaCoreDbContext _context;
        public EfClientDal(DietaCoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Client> GetByUserIdAsync(int userId)
        {
            return await _context.Clients
                .Include(c => c.User)
                .Include(c => c.Dietitian)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<IList<Client>> GetAllWithUsersAsync()
        {
            return await _context.Clients
                .Include(c => c.User)
                .Include(c => c.Dietitian)
                 .ThenInclude(d => d.User)
                .ToListAsync();
        }

        public async Task<Client> GetByIdWithUserAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.User)
                .Include(c => c.Dietitian)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IList<Client>> GetByDietitianIdAsync(int dietitianId)
        {
            return await _context.Clients
                .Include(c => c.User)
                .Where(c => c.DietitianId == dietitianId)
                .ToListAsync();
        }
        public async Task<int?> GetDietitianIdByClientId(int clientId)
        {
            return await _context.Clients
                .Where(c => c.Id == clientId)
                .Select(c => c.DietitianId)
                .FirstOrDefaultAsync();
        }

        public async Task<Client> GetByIdWithDietPlansAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.User)
                .Include(c => c.Dietitian)
                    .ThenInclude(d => d.User)
                .Include(c => c.DietPlans)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}

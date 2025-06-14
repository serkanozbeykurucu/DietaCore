using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DietaCore.DataAccess.Concrete.EntityFramewokCore
{
    public class EfDietitianDal : GenericRepository<Dietitian>, IDietitianDal
    {
        private readonly DietaCoreDbContext _context;
        public EfDietitianDal(DietaCoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Dietitian> GetByUserIdAsync(int userId)
        {
            return await _context.Dietitians
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        public async Task<IList<Dietitian>> GetAllWithUsersAsync()
        {
            return await _context.Dietitians.Include(d => d.User).ToListAsync();
        }

        public async Task<Dietitian> GetByIdWithUsersAsync(int id)
        {
            return await _context.Dietitians.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == id );
        }

        public async Task<Dietitian> GetByIdWithClientsAsync(int id)
        {
            return await _context.Dietitians
                .Include(d => d.User)
                .Include(d => d.Clients)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}

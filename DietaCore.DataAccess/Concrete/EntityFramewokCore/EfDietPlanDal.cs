using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DietaCore.DataAccess.Concrete.EntityFramewokCore
{
    public class EfDietPlanDal : GenericRepository<DietPlan>, IDietPlanDal
    {
        private readonly DietaCoreDbContext _context;
        public EfDietPlanDal(DietaCoreDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<DietPlan> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DietPlans
                .Include(dp => dp.Client)
                    .ThenInclude(c => c.User)
                .Include(dp => dp.CreatedByDietitian)
                    .ThenInclude(d => d.User)
                .Include(dp => dp.Meals)
                .FirstOrDefaultAsync(dp => dp.Id == id);
        }

        public async Task<IList<DietPlan>> GetByClientIdAsync(int clientId)
        {
            return await _context.DietPlans
                .Include(dp => dp.Client)
                    .ThenInclude(c => c.User)
                .Include(dp => dp.CreatedByDietitian)
                    .ThenInclude(d => d.User)
                .Include(dp => dp.Meals)
                .Where(dp => dp.ClientId == clientId)
                .ToListAsync();
        }
        public async Task<IList<DietPlan>> GetByDietitianIdAsync(int dietitianId)
        {
            return await _context.DietPlans
                .Include(dp => dp.Client)
                    .ThenInclude(c => c.User)
                .Include(dp => dp.CreatedByDietitian)
                    .ThenInclude(d => d.User)
                .Include(dp => dp.Meals)
                .Where(dp => dp.CreatedByDietitianId == dietitianId)
                .ToListAsync();
        }

        public async Task<IList<DietPlan>> GetActiveByClientIdAsync(int clientId)
        {
            return await _context.DietPlans
                .Where(dp => dp.ClientId == clientId &&
                           dp.StartDate <= DateTime.Now &&
                           (dp.EndDate == null || dp.EndDate >= DateTime.Now))
                .OrderByDescending(dp => dp.CreatedAt)
                .ToListAsync();
        }

        public async Task<IList<DietPlan>> GetByClientIdWithDetailsAsync(int clientId)
        {
            return await _context.DietPlans
                .Include(dp => dp.Client)
                    .ThenInclude(c => c.User)
                .Include(dp => dp.CreatedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(dp => dp.ClientId == clientId)
                .OrderByDescending(dp => dp.CreatedAt)
                .ToListAsync();
        }
        public async Task<IList<DietPlan>> GetActivePlansAsync()
        {
            return await _context.DietPlans
                .Include(dp => dp.Client)
                    .ThenInclude(c => c.User)
                .Include(dp => dp.CreatedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(dp => dp.StartDate <= DateTime.Now &&
                           (dp.EndDate == null || dp.EndDate >= DateTime.Now))
                .OrderByDescending(dp => dp.CreatedAt)
                .ToListAsync();
        }
        public async Task<DietPlan> GetCurrentActivePlanByClientIdAsync(int clientId)
        {
            return await _context.DietPlans
                .Include(dp => dp.Client)
                    .ThenInclude(c => c.User)
                .Include(dp => dp.CreatedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(dp => dp.ClientId == clientId &&
                           dp.StartDate <= DateTime.Now &&
                           (dp.EndDate == null || dp.EndDate >= DateTime.Now))
                .OrderByDescending(dp => dp.StartDate)
                .FirstOrDefaultAsync();
        }
    }
}

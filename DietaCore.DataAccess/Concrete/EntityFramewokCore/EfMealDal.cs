using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DietaCore.DataAccess.Concrete.EntityFramewokCore
{
    public class EfMealDal : GenericRepository<Meal>, IMealDal
    {
        private readonly DietaCoreDbContext _context;
        public EfMealDal(DietaCoreDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IList<Meal>> GetByDietPlanIdAsync(int dietPlanId)
        {
            return await _context.Meals
                .Include(m => m.DietPlan)
                .Where(m => m.DietPlanId == dietPlanId)
                .OrderBy(m => m.StartTime)
                .ToListAsync();
        }
    }
}
using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietaCore.DataAccess.Concrete.EntityFramewokCore
{
    public class EfClientProgressDal : GenericRepository<ClientProgress>, IClientProgressDal
    {
        private readonly DietaCoreDbContext _context;

        public EfClientProgressDal(DietaCoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IList<ClientProgress>> GetByClientIdAsync(int clientId)
        {
            return await _context.ClientProgresses.Where(cp => cp.ClientId == clientId)
                .OrderByDescending(cp => cp.RecordedDate)
                .ToListAsync();
        }

        public async Task<IList<ClientProgress>> GetByClientIdWithDetailsAsync(int clientId)
        {
            return await _context.ClientProgresses.Include(cp => cp.Client).ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian).ThenInclude(d => d.User)
                .Where(cp => cp.ClientId == clientId)
                .OrderByDescending(cp => cp.RecordedDate)
                .ToListAsync();
        }

        public async Task<ClientProgress> GetByIdWithDetailsAsync(int progressId)
        {
            return await _context.ClientProgresses.Include(cp => cp.Client).ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(cp => cp.Id == progressId);
        }
        public async Task<IList<ClientProgress>> GetByDateRangeAsync(int clientId, DateTime startDate, DateTime endDate)
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(cp => cp.ClientId == clientId &&
                           cp.RecordedDate >= startDate &&
                           cp.RecordedDate <= endDate)
                .OrderByDescending(cp => cp.RecordedDate)
                .ToListAsync();
        }
        public async Task<IList<ClientProgress>> GetByDietitianIdAsync(int dietitianId)
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(cp => cp.Client.DietitianId == dietitianId)
                .OrderByDescending(cp => cp.RecordedDate)
                .ToListAsync();
        }
        public async Task<IList<ClientProgress>> GetLatestByClientIdAsync(int clientId, int count = 10)
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(cp => cp.ClientId == clientId)
                .OrderByDescending(cp => cp.RecordedDate)
                .Take(count)
                .ToListAsync();
        }
        public async Task<ClientProgress> GetLatestByClientIdAsync(int clientId)
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(cp => cp.ClientId == clientId)
                .OrderByDescending(cp => cp.RecordedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<ClientProgress>> GetByClientIdsAsync(IList<int> clientIds)
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .Where(cp => clientIds.Contains(cp.ClientId))
                .OrderByDescending(cp => cp.RecordedDate)
                .ToListAsync();
        }

        public async Task<ClientProgress> GetByIdAsync(int id)
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(cp => cp.Id == id);
        }

        public async Task<IList<ClientProgress>> GetAllAsync()
        {
            return await _context.ClientProgresses
                .Include(cp => cp.Client)
                    .ThenInclude(c => c.User)
                .Include(cp => cp.RecordedByDietitian)
                    .ThenInclude(d => d.User)
                .OrderByDescending(cp => cp.RecordedDate)
                .ToListAsync();
        }
    }
}

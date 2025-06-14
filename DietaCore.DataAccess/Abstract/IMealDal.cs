using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietaCore.DataAccess.Abstract
{
    public interface IMealDal : IGenericDal<Meal>
    {
        Task<IList<Meal>> GetByDietPlanIdAsync(int dietPlanId);
    }
}

using DietaCore.Business.Abstract;
using DietaCore.Business.Concrete;
using DietaCore.Business.Utilities.Abstract;
using DietaCore.Business.Utilities.Concrete;
using DietaCore.DataAccess.Abstract;
using DietaCore.DataAccess.Concrete;
using DietaCore.DataAccess.Concrete.EntityFramewokCore;
using DietaCore.DataAccess.Repositories;
using DietaCore.Entities.Concrete;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using DotNetEnv;


namespace DietaCore.Business.DependencyResolvers.Microsoft
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ContainerDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            Env.Load();

            var connectionString = Environment.GetEnvironmentVariable("DietaCoreDbConnection") ?? throw new InvalidOperationException("Connection string not found.");

            services.AddDbContext<DietaCoreDbContext>(options => options.UseNpgsql(connectionString));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDietitianService, DietitianService>();
            services.AddScoped<IDietitianDal, EfDietitianDal>();

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IClientDal, EfClientDal>();

            services.AddScoped<IDietPlanService, DietPlanService>();
            services.AddScoped<IDietPlanDal, EfDietPlanDal>();

            services.AddScoped<IMealService, MealService>();
            services.AddScoped<IMealDal, EfMealDal>();

            services.AddScoped<IClientProgressService, ClientProgressService>();
            services.AddScoped<IClientProgressDal, EfClientProgressDal>();

            services.AddScoped(typeof(IGenericDal<>), typeof(GenericRepository<>));

            services.AddScoped<IUserContextHelper, UserContextHelper>();
            services.AddHttpContextAccessor();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
           .AddEntityFrameworkStores<DietaCoreDbContext>()
           .AddDefaultTokenProviders();

            return services;
        }
    }
}

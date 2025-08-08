using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopSpire.Repository.Data;
using ShopSpire.Repository.Repositories;
using ShopSpire.Service;
using ShopSpire.Utilities.Helpers;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;
using ShopSpireCore.Services;

namespace ShopSpire.API.Extensions
{
    public static class AppSerivce
    {
        public static void AddAppServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<ShopSpireDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ShopSpireConnnectionString")));
            //Serice Registration
            services.AddScoped<IUserService, UserService>();
            //Repository Registration
            services.AddScoped<IUserRepository, UserRepository>();
            //Token Helper Registration
            services.AddSingleton<TokenHelper>();
            // HttpContext Accessor Registration
            services.AddHttpContextAccessor();
            services.AddIdentity<User, IdentityRole>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
                o.Password.RequiredUniqueChars = 1;

            }).AddEntityFrameworkStores<ShopSpireDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}

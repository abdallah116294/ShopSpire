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
            //Register IEmailService if you have an email service
            services.Configure<EmailConfiguration>(configuration.GetSection("EmialConfiguration"));
            var emailConfig = configuration.GetSection("EmialConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig); // Register EmailConfiguration as a singleton service
            services.AddScoped<IEmailService, EmailService>(); // Register the email service    
            //Add Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //Add UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //Add Role Repository
            services.AddScoped<IRoleRepository, RoleRepository>();
            //Add Role Service 
            services.AddScoped<IRoleService, RoleService>();
            services.AddIdentityCore<User>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
                o.Password.RequiredUniqueChars = 1;

            }).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ShopSpireDbContext>()
                .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();
        }
    }
}

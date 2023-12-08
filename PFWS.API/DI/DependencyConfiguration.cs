using Microsoft.AspNetCore.Identity;
using PFWS.BusinessLogicLayer.Services;
using PFWS.BusinessLogicLayer.Services.Implementation;
using PFWS.DataAccessLayer.Data;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;
using PFWS.DataAccessLayer.Repositories.Implementation;

namespace PFWS.API.DI;

public static class DependencyConfiguration
{
    public static IServiceCollection ConfigureDependency(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepositoryBase<Account>, RepositoryBase<Account>>();
        services.AddScoped<IRepositoryBase<Transaction>, RepositoryBase<Transaction>>();
        services.AddScoped<IRepositoryBase<Category>, RepositoryBase<Category>>();
        services.AddScoped<IRepositoryBase<User>, RepositoryBase<User>>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();

        // Identity
        services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = false;
        })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<WalletContext>();

        return services;
    }
}

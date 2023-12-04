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

        // Services
        services.AddScoped<IAccountService, AccountService>();

        // Identity
        services.AddIdentityCore<User>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = false;
        })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<WalletContext>();

        return services;
    }
}

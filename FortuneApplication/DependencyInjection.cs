using System.Reflection;
using FluentValidation;
using FortuneApplication.IServices;
using FortuneApplication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FortuneApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupExpenseService, GroupExpenseService>();
            services.AddScoped<ISettlementService, SettlementService>();

            return services;
        }
    }
}

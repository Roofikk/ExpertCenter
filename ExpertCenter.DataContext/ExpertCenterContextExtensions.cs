using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertCenter.DataContext;

public static class ExpertCenterContextExtensions
{
    public static IServiceCollection AddExpertCenterContext(this IServiceCollection services, string? connectionString = null)
    {
        connectionString ??= "Data Source=../ExpertCenter.db";

        return services.AddDbContext<ExpertCenterContext>(options =>
        {
            options.UseSqlite(connectionString);
        });
    }
}

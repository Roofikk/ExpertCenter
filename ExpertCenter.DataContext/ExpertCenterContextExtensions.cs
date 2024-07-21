using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertCenter.DataContext;

public static class ExpertCenterContextExtensions
{
    public static IServiceCollection AddExpertCenterContext(this IServiceCollection services, string? connectionString = null)
    {
        connectionString ??= @"Server=RufikDesktop;Database=ExpertCenter;User=sa;Password=Rufik2024;TrustServerCertificate=True;";

        return services.AddDbContext<ExpertCenterContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
}

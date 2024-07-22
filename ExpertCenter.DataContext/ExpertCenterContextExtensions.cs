using ExpertCenter.DataContext.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ExpertCenter.DataContext;

public static class ExpertCenterContextExtensions
{
    public static IServiceCollection AddExpertCenterContext(this IServiceCollection services, string? connectionString = null)
    {
        connectionString ??= @"Server=RUFIKDESKTOP;Database=ExpertCenter;User=sa;Password=root1234;TrustServerCertificate=True;";

        return services.AddDbContext<ExpertCenterContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }

    public static async Task InitializeAsync(this ExpertCenterContext context)
    {
        var types = await context.ColumnTypes.ToListAsync();

        if (!types.Any(t => t.ColumnTypeId == nameof(IntColumn)))
        {
            await context.ColumnTypes.AddAsync(new ColumnType
            {
                ColumnTypeId = nameof(IntColumn),
                DisplayName = "Числовой",
            });
        }

        if (!types.Any(t => t.ColumnTypeId == nameof(VarCharColumn)))
        {
            await context.ColumnTypes.AddAsync(new ColumnType
            {
                ColumnTypeId = nameof(VarCharColumn),
                DisplayName = "Однострочный"
            });
        }

        if (!types.Any(t => t.ColumnTypeId == nameof(StringTextColumn)))
        {
            await context.ColumnTypes.AddAsync(new ColumnType
            {
                ColumnTypeId = nameof(StringTextColumn),
                DisplayName = "Многострочный"
            });
        }

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync();
        }
    }
}

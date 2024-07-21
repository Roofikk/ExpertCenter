using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddExpertCenterContext();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ExpertCenterContext>();

    if (!await context.ColumnTypes.AnyAsync(c => c.ColumnTypeId == nameof(IntColumn)))
    {
        await context.ColumnTypes.AddAsync(new ColumnType
        {
            ColumnTypeId = nameof(IntColumn),
            DisplayName = "Числовой",
        });
    }

    if (!await context.ColumnTypes.AnyAsync(c => c.ColumnTypeId == nameof(VarCharColumn)))
    {
        await context.ColumnTypes.AddAsync(new ColumnType
        {
            ColumnTypeId = nameof(VarCharColumn),
            DisplayName = "Однострочный"
        });
    }

    if (!await context.ColumnTypes.AnyAsync(c => c.ColumnTypeId == nameof(StringTextColumn)))
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
});

app.Run();

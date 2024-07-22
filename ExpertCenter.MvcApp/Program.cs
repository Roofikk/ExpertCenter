using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Services.PriceLists;
using ExpertCenter.MvcApp.Services.Products;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddExpertCenterContext(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IPriceListsService, PriceListsService>();

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
            DisplayName = "��������",
        });
    }

    if (!await context.ColumnTypes.AnyAsync(c => c.ColumnTypeId == nameof(VarCharColumn)))
    {
        await context.ColumnTypes.AddAsync(new ColumnType
        {
            ColumnTypeId = nameof(VarCharColumn),
            DisplayName = "������������"
        });
    }

    if (!await context.ColumnTypes.AnyAsync(c => c.ColumnTypeId == nameof(StringTextColumn)))
    {
        await context.ColumnTypes.AddAsync(new ColumnType
        {
            ColumnTypeId = nameof(StringTextColumn),
            DisplayName = "�������������"
        });
    }

    if (context.ChangeTracker.HasChanges())
    {
        await context.SaveChangesAsync();
    }
});

app.Run();

using ExpertCenter.DataContext;
using ExpertCenter.MvcApp.Hubs;
using ExpertCenter.MvcApp.Services.PriceLists;
using ExpertCenter.MvcApp.Services.Products;
using ExpertCenter.MvcApp.Services.ProductTableDependency;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddExpertCenterContext(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IPriceListsService, PriceListsService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ProductsHub>();
builder.Services.AddSingleton<ISubscribeProductTableDependency, SubscribeProductTableDependency>();

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

app.MapHub<ProductsHub>("/productHub");
app.Services.GetService<ISubscribeProductTableDependency>()
    ?.SubscribeTableDependency(app.Configuration.GetConnectionString("DefaultConnection")!);

app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ExpertCenterContext>();
    await context.InitializeAsync();
});

app.Run();

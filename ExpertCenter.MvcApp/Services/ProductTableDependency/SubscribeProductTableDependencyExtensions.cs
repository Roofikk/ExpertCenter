namespace ExpertCenter.MvcApp.Services.ProductTableDependency;

public static class SubscribeProductTableDependencyExtensions
{
    public static void SubscribeProductTableDependency(this IServiceProvider services, IConfiguration configuration)
    {
        var subscribeProductTableDependency = services.GetService<ISubscribeProductTableDependency>()
            ?? throw new Exception("Need to implement ISubscribeProductTableDependency");

        subscribeProductTableDependency.SubscribeTableDependency(configuration.GetConnectionString("DefaultConnection")!);
    }
}

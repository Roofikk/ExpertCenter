using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Hubs;
using TableDependency.SqlClient;

namespace ExpertCenter.MvcApp.Services.ProductTableDependency;

public class SubscribeProductTableDependency : ISubscribeProductTableDependency
{
    private SqlTableDependency<Product>? tableDependency;
    private readonly ProductsHub _productsHub;

    public SubscribeProductTableDependency(ProductsHub dashboardHub)
    {
        _productsHub = dashboardHub;
    }

    public void SubscribeTableDependency(string connectionString)
    {
        tableDependency = new SqlTableDependency<Product>(connectionString, "Products");

        tableDependency.OnChanged += TableDependency_OnChanged;
        tableDependency.OnError += TableDependency_OnError;
        tableDependency.Start();
    }

    private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Product> e)
    {
        if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
        {
            Task.Run(async () => await _productsHub.SendProducts(e.Entity.PriceListId));
        }
    }

    private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
    {
        Console.WriteLine($"{nameof(Products)} SqlTableDependency error: {e.Error.Message}");
    }
}

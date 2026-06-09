namespace Application.DashboardManagement.Get;

public sealed record GetDashboardResponse(
    int NumberOfProducts,
    int LowStockProducts,
    int NumOfActiveOrders,
    IReadOnlyCollection<DashboardOrderDto> RecentOrders,
    IReadOnlyCollection<DashboardProductDto> Top5ProductsWithLowStock
    );

namespace Dal;

/// <summary>
/// Simple in-memory data container used by the demo DAL implementations.
/// Holds shared lists that act as the application's temporary "database".
/// Not thread-safe and not persistent; intended for testing and demos only.
/// </summary>
internal static class DataSource
{
    // In-memory list of couriers (acts like the Couriers table).
    internal static List<DO.Courier> Couriers { get; } = new();

    // In-memory list of orders (acts like the Orders table).
    internal static List<DO.Order> Orders { get; } = new();

    // In-memory list of deliveries (acts like the Deliveries table).
    internal static List<DO.Delivery> Deliveries { get; } = new();
}

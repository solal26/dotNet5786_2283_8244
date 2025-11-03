namespace DO;

using System;

/// <summary>
/// Represents an order for shipment or internal transfer.
/// </summary>
/// <param name="OrderId">Unique identifier for the order.</param>
/// <param name="typeOfOrder">The category of the order (Single, Bulk, Express, InternalTransfer).</param>
/// <param name="ShippingAddress">Delivery address for the order.</param>
/// <param name="Latitude">Latitude coordinate of the shipping address.</param>
/// <param name="Longitude">Longitude coordinate of the shipping address.</param>
/// <param name="CustomerName">Customer full name associated with the order.</param>
/// <param name="CustomerPhone">Customer contact phone number.</param>
/// <param name="weight">Shipment weight in kilograms.</param>
/// <param name="OrderInitiation">Date and time when the order was created or initiated.</param>
/// <param name="VerbalDescription">Optional free-text description of the order.</param>
public record Order
    (
    int OrderId,
    TypeOfOrder typeOfOrder,
    string ShippingAddress,
    double Latitude,
    double Longitude,
    string CustomerName,
    string CustomerPhone,
    double weight,
    DateTime OrderInitiation,
    string? VerbalDescription = null
    )
{
    /// <summary>
    /// Initializes a default <see cref="Order"/> instance.
    /// Defaults: OrderId = 0, <see cref="TypeOfOrder.SingleOrder"/>, empty strings for text fields,
    /// zero coordinates and weight, and <see cref="DateTime.MinValue"/> for initiation time.
    /// </summary>
    public Order(): this(0, TypeOfOrder.SingleOrder, "", 0.0, 0.0, "", "", 0.0, DateTime.MinValue) { }
}












namespace BO;
using Helpers;
using System;

/// <summary>
/// Business-layer projection for a closed delivery used in lists and reports.
/// Contains combined, read-only information derived from DO.Order and DO.Delivery suitable for UI presentation.
/// </summary>
/// <remarks>
/// - A "closed" delivery here means a delivery that has an end state (delivery completed, refused, cancelled, etc.)
///   and therefore a measured completion time and (optionally) a recorded actual distance.
/// - This DTO is intended for read-only scenarios (display, reporting, sorting, filtering).
/// - Nullability:
///   - <see cref="ActualDistance"/> is nullable when distance was not measured or not recorded.
///   - <see cref="EndType"/> may be null only in edge cases where outcome is unknown.
/// </remarks>
public class ClosedDeliveryInList
{
    /// <summary>
    /// Identifier of the delivery record (maps to DO.Delivery.Id).
    /// </summary>
    public int DeliveryId { get; init; }

    /// <summary>
    /// Identifier of the associated order (maps to DO.Order.OrderId).
    /// </summary>
    public int OrderId { get; init; }

    /// <summary>
    /// The order classification (Single, Bulk, Express, InternalTransfer).
    /// Useful for filtering and display.
    /// </summary>
    public OrderType OrderType { get; init; }

    /// <summary>
    /// Full shipping address from the order (human-readable).
    /// </summary>
    public string FullAddress { get; init; }

    /// <summary>
    /// The delivery transport type used for this delivery (Car, Scooter, Bike, OnFoot).
    /// </summary>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// The actual distance travelled for this delivery in kilometers, if available.
    /// </summary>
    public double? ActualDistance { get; init; }

    /// <summary>
    /// Time span between delivery start (<c>TimeBegin</c>) and completion (<c>TimeEnd</c>).
    /// </summary>
    public TimeSpan CompletionTime { get; init; }

    /// <summary>
    /// The outcome of the delivery (Delivered, Refused, Cancelled, NotHere, Failed).
    /// </summary>
    public DeliveryEndType? EndType { get; init; }

    /// <summary>
    /// Returns a human-readable representation of the object using the Helpers extension.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}


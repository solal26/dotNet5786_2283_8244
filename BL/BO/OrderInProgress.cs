using System;
using BO;
using Helpers;
namespace BO;

/// <summary>
/// Business-layer projection that describes an order currently in progress for a courier.
/// Combines order and delivery data required by courier-facing UIs and runtime logic.
/// </summary>
/// <remarks>
/// - This DTO is produced by BL helpers (e.g., OrderManager / CourierManager) and is read-only (init-only properties).
/// - Times and time calculations should use the application's time source (see <c>Config.Clock</c>).
/// - Distances are expressed in kilometers; weights in kilograms where applicable.
/// </remarks>
public class OrderInProgress
{
    /// <summary>
    /// Delivery record identifier (DO.Delivery.Id) for the active delivery attempt.
    /// </summary>
    public int DeliveryId { get; init; }

    /// <summary>
    /// Associated order identifier (DO.Order.OrderId).
    /// </summary>
    public int OrderId { get; init; }

    /// <summary>
    /// Order classification (SingleOrder, BulkOrder, ExpressOrder, InternalTransfer).
    /// </summary>
    public OrderType Type { get; init; }

    /// <summary>
    /// Optional textual description or customer notes for the order.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Full shipping address as stored on the order (human-readable).
    /// </summary>
    public string FullAddress { get; init; }

    /// <summary>
    /// Straight-line (great-circle) distance from company to destination in kilometers.
    /// Used for rough ETA calculations and ranking.
    /// </summary>
    public double AirDistance { get; init; }

    /// <summary>
    /// Optional measured or estimated route distance (kilometers). Null when not available.
    /// </summary>
    public double? ActualDistance { get; init; }

    /// <summary>
    /// Customer full name associated with the order.
    /// </summary>
    public string CustomerName { get; init; }

    /// <summary>
    /// Customer contact phone number.
    /// </summary>
    public string CustomerPhone { get; init; }

    /// <summary>
    /// Order creation/open time (DO.Order.OrderInitiation). Init-only.
    /// </summary>
    public DateTime OpenTime { get; init; }

    /// <summary>
    /// Delivery start/dispatch time for this attempt (DO.Delivery.TimeBegin).
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Computed expected delivery DateTime for the current assignment (based on speed estimates and distance).
    /// </summary>
    public DateTime ExpectedDeliveryTime { get; init; }

    /// <summary>
    /// Maximum allowed delivery DateTime (OpenTime + configured MaxDeliveryTime).
    /// </summary>
    public DateTime MaxDeliveryTime { get; init; }

    /// <summary>
    /// Logical order status (Open, InProgress, Delivered, Refused, Cancelled).
    /// </summary>
    public OrderStatus OrderStatus { get; init; }

    /// <summary>
    /// Schedule status relative to SLA (OnTime, InRisk, Late).
    /// </summary>
    public ScheduleStatus ScheduleStatus { get; init; }

    /// <summary>
    /// Time remaining until the maximum allowed delivery time (positive) or negative if overdue.
    /// </summary>
    public TimeSpan TimeRemaining { get; init; }

    /// <summary>
    /// Human-readable representation produced by the Helpers extension.
    /// </summary>
    public override string ToString()
    {
        return this.ToStringProperty();
    }
}
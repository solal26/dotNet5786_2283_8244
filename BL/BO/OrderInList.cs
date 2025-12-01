using System;
using BO;
using Helpers;

namespace BO;

/// <summary>
/// Lightweight business projection representing an order for list displays.
/// Contains summary fields derived from DO.Order and related DO.Delivery records,
/// optimized for sorting, filtering and quick UI presentation.
/// </summary>
/// <remarks>
/// - This DTO is read-only (init-only properties) and intended for display and reporting.
/// - Time and distance units:
///   - <see cref="AirDistance"/> is in kilometers (great-circle).
///   - <see cref="TimeRemaining"/> and <see cref="TotalCompletionTime"/> are <see cref="TimeSpan"/> values.
/// - Status values (<see cref="OrderStatus"/>, <see cref="ScheduleStatus"/>) are computed by BL logic.
/// </remarks>
public class OrderInList
{
    /// <summary>
    /// Identifier of the last delivery attempt (if any) associated with the order.
    /// Null when the order has no delivery attempts yet.
    /// </summary>
    public int? DeliveryId { get; init; }

    /// <summary>
    /// Unique order identifier (maps to DO.Order.OrderId).
    /// </summary>
    public int OrderId { get; init; }

    /// <summary>
    /// Classification of the order (Single, Bulk, Express, InternalTransfer).
    /// </summary>
    public OrderType Type { get; init; }

    /// <summary>
    /// Great-circle (air) distance from the company to the delivery address, in kilometers.
    /// Used for rough ETA and ranking when assigning couriers.
    /// </summary>
    public double AirDistance { get; init; }

    /// <summary>
    /// Logical order status (Open, InProgress, Delivered, Refused, Cancelled).
    /// Determined from associated delivery records.
    /// </summary>
    public OrderStatus OrderStatus { get; init; }

    /// <summary>
    /// Schedule status relative to SLA: OnTime, InRisk or Late.
    /// Computed using the order initiation time and configured max delivery time.
    /// </summary>
    public ScheduleStatus ScheduleStatus { get; init; }

    /// <summary>
    /// Time remaining until the order's maximum allowed delivery time (positive) or negative if overdue.
    /// </summary>
    public TimeSpan TimeRemaining { get; init; }

    /// <summary>
    /// Total completion time for the order based on the last completed delivery:
    /// Time from order open to the last delivery completion. Zero when not completed.
    /// </summary>
    public TimeSpan TotalCompletionTime { get; init; }

    /// <summary>
    /// Total number of delivery attempts performed for this order.
    /// </summary>
    public int TotalDeliveries { get; init; }

    /// <summary>
    /// Human-readable representation produced by the Helpers extension.
    /// </summary>
    public override string ToString()
    {
        return this.ToStringProperty();
    }
}
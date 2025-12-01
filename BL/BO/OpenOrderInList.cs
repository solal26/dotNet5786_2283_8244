using System;
using BO;
using Helpers;

namespace BO;

/// <summary>
/// Lightweight business DTO representing an open order suitable for courier-facing lists.
/// </summary>
/// <remarks>
/// - Produced by BL to show orders that are available for assignment (no active delivery) or to present candidate orders to a courier.
/// - Contains both static order data (weight, address) and computed metrics used for matching and display:
///   - <see cref="AirDistance"/> is the great-circle distance from company to destination (km).
///   - <see cref="ActualDistance"/> and <see cref="EstimatedActualTime"/> are optional runtime estimates (may be null for unassigned orders).
///   - <see cref="ScheduleStatus"/>, <see cref="TimeRemaining"/> and <see cref="MaxDeliveryTime"/> are used for SLA and prioritization.
/// - This DTO is read-only (init-only properties) and intended for UI listing, sorting and filtering.
/// </remarks>
public class OpenOrderInList
{
    /// <summary>
    /// Id of the courier that selected or is assigned to this order, or <c>null</c> when none.
    /// </summary>
    public int? CourierId { get; init; }

    /// <summary>
    /// Unique identifier of the order (maps to DO.Order.OrderId).
    /// </summary>
    public int OrderId { get; init; }

    /// <summary>
    /// Order classification (Single, Bulk, Express, InternalTransfer).
    /// </summary>
    public OrderType Type { get; init; }

    /// <summary>
    /// Shipment weight in kilograms.
    /// </summary>
    public double Weight { get; init; }

    /// <summary>
    /// Full shipping address as stored on the order.
    /// </summary>
    public string FullAddress { get; init; }

    /// <summary>
    /// Straight-line (air) distance from the company to the delivery address in kilometers.
    /// Used for rough ETA and matching logic.
    /// </summary>
    public double AirDistance { get; init; }

    /// <summary>
    /// Optional estimated route or actual distance in kilometers (nullable when not measured).
    /// </summary>
    public double? ActualDistance { get; init; }

    /// <summary>
    /// Optional estimated travel time for the chosen transport mode (TimeSpan) — nullable when not computed.
    /// </summary>
    public TimeSpan? EstimatedActualTime { get; init; }

    /// <summary>
    /// Schedule status relative to the company's SLA: OnTime, InRisk or Late.
    /// </summary>
    public ScheduleStatus ScheduleStatus { get; init; }

    /// <summary>
    /// Time remaining until the order's maximum allowed delivery time (positive) or negative if overdue.
    /// </summary>
    public TimeSpan TimeRemaining { get; init; }

    /// <summary>
    /// Latest allowed delivery DateTime for the order (OrderInitiation + Config.MaxDeliveryTime).
    /// </summary>
    public DateTime MaxDeliveryTime { get; init; }

    public override string ToString()
    {
        return this.ToStringProperty();
    }
}
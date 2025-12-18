using System;
using System.Collections.Generic;
using Helpers;

namespace BO;

/// <summary>
/// Business-layer representation of an order.
/// Contains both stored order data and computed fields used by the BL layer and presentation logic.
/// </summary>
/// <remarks>
/// - This DTO is used by BL services and the UI. It combines persisted order information (DO.Order)
///   with runtime/computed fields such as <see cref="OrderStatus"/>, <see cref="ScheduleStatus"/>,
///   <see cref="ExpectedDeliveryTime"/>, and <see cref="TimeRemaining"/>.
/// - Times should be interpreted using the application's time source (e.g., <c>Config.Clock</c>).
/// - Units:
///   - <see cref="Weight"/> is in kilograms.
///   - <see cref="AirDistance"/> is in kilometers (great-circle distance).
/// </remarks>
public class Order
{
    /// <summary>
    /// Order identifier (maps to DO.Order.OrderId). Immutable after creation.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Classification of the order (Single, Bulk, Express, InternalTransfer).
    /// </summary>
    public OrderType Type { get; set; }

    /// <summary>
    /// Optional textual description or notes provided by the customer.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Full shipping address as a human-readable string.
    /// Validation and geocoding occur in BL before persisting.
    /// </summary>
    public string FullAddress { get; set; }

    /// <summary>
    /// Destination latitude (decimal degrees). Populated by geocoding or supplied by caller.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Destination longitude (decimal degrees). Populated by geocoding or supplied by caller.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Straight-line (air) distance from the company to the destination in kilometers.
    /// Used for rough ETA and matching logic.
    /// </summary>
    public double AirDistance { get; set; }

    /// <summary>
    /// Customer full name associated with the order.
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// Customer contact phone number (validated by BL).
    /// </summary>
    public string CustomerPhone { get; set; }

    /// <summary>
    /// Shipment weight in kilograms.
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// Order creation/open time. Init-only and represents the order's open timestamp.
    /// </summary>
    public DateTime OpenTime { get; init; }=AdminManager.Now;

    /// <summary>
    /// Computed expected delivery DateTime for the current assignment/transport type.
    /// </summary>
    public DateTime? ExpectedDeliveryTime { get; init; }

    /// <summary>
    /// Maximum allowed delivery DateTime (OpenTime + configured MaxDeliveryTime).
    /// </summary>
    public DateTime MaxDeliveryTime { get; init; }

    /// <summary>
    /// Logical order status (Open, InProgress, Delivered, Refused, Cancelled).
    /// Computed by BL based on related deliveries.
    /// </summary>
    public OrderStatus OrderStatus { get; init; }

    /// <summary>
    /// Schedule status relative to SLA (OnTime, InRisk, Late).
    /// Computed using <see cref="MaxDeliveryTime"/> and the application's clock.
    /// </summary>
    public ScheduleStatus ScheduleStatus { get; init; }

    /// <summary>
    /// Time remaining until the maximum allowed delivery time (positive) or negative if overdue.
    /// </summary>
    public TimeSpan TimeRemaining { get; init; }

    /// <summary>
    /// List of delivery attempts for this order. Items are projections (<see cref="DeliveryPerOrderInList"/>).
    /// </summary>
    public List<DeliveryPerOrderInList> Deliveries { get; init; }

    /// <summary>
    /// Returns a readable representation of the order using helper reflection-based formatting.
    /// </summary>
    public override string ToString()
    {
        return this.ToStringProperty();
    }
}

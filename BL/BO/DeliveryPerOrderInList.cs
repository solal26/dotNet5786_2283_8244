namespace BO;
using Helpers;
using System;

/// <summary>
/// Lightweight business DTO describing a single delivery entry that belongs to an order.
/// Used for list views showing each delivery attempt for an order (open or closed).
/// </summary>
/// <remarks>
/// - This class is a read-only projection assembled by BL from DO.Order and DO.Delivery records.
/// - Nullable properties indicate values that may be unknown for active/in-progress deliveries (for example <see cref="EndType"/> and <see cref="EndTime"/>).
/// - Times are expressed using the application's time source convention (see <c>Config.Clock</c>).
/// </remarks>
public class DeliveryPerOrderInList
{
    /// <summary>
    /// Delivery record identifier (maps to DO.Delivery.Id).
    /// </summary>
    public int DeliveryId { get; init; }

    /// <summary>
    /// Identifier of the assigned courier, or <c>null</c> when there is no courier assigned.
    /// </summary>
    public int? CourierId { get; init; }

    /// <summary>
    /// Human-friendly courier name used for display; <c>null</c>/empty when the courier is unknown.
    /// </summary>
    public string CourierName { get; init; }

    /// <summary>
    /// Transport type used for this delivery (Car, Scooter, Bike, OnFoot).
    /// Used for ETA calculations and display.
    /// </summary>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// Delivery start time (dispatch time / TimeBegin).
    /// </summary>
    public DateTime StartTime { get; init; }

    /// <summary>
    /// Delivery outcome (Delivered, Refused, Cancelled, NotHere, Failed).
    /// Nullable for in-progress deliveries that have no final outcome yet.
    /// </summary>
    public DeliveryEndType? EndType { get; init; }

    /// <summary>
    /// Delivery end time (completion or failure). Nullable for active/in-progress deliveries.
    /// </summary>
    public DateTime? EndTime { get; init; }

    /// <summary>
    /// Returns a readable representation of the DTO using the Helpers extension.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
namespace BO;
using Helpers;
using System;

/// <summary>
/// Lightweight business-layer projection of a courier used for list displays.
/// Contains read-only summary information and computed metrics suitable for UI lists and sorting/filtering.
/// </summary>
/// <remarks>
/// - This DTO is optimized for presentation: it carries a small, commonly used subset of courier data.
/// - Values are populated by BL helpers (for example <see cref="Helpers.CourierManager.GetAllCouriersInList"/>).
/// - All properties are init-only to emphasize immutability for list projections.
/// - Times use the application's time source convention (see <c>Config.Clock</c> in the DAL/BL).
/// </remarks>
public class CourierInList
{
    /// <summary>
    /// Courier unique identifier (maps to DO.Courier.Id).
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Courier full name for display and sorting.
    /// </summary>
    public string FullName { get; init; }

    /// <summary>
    /// Whether the courier is currently active/available.
    /// </summary>
    public bool Active { get; init; }

    /// <summary>
    /// Preferred or assigned delivery transport type (Car, Scooter, Bike, OnFoot).
    /// Used for filtering and ETA/assignment logic at higher layers.
    /// </summary>
    public DeliveryType DeliveryType { get; init; }

    /// <summary>
    /// Couriers start date/time (init-only). Useful for sorting by tenure or calculating inactivity.
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Count of deliveries completed on time by this courier (computed by BL).
    /// </summary>
    public int DeliveredOnTime { get; init; }

    /// <summary>
    /// Count of deliveries completed late by this courier (computed by BL).
    /// </summary>
    public int DeliveredLate { get; init; }

    /// <summary>
    /// Identifier of the current order in progress for this courier, or null when none.
    /// This is a convenience field for list UIs showing a quick assignment indicator.
    /// </summary>
    public int? CurrentOrderId { get; init; }

    /// <summary>
    /// Returns a readable representation of the object using the Helpers extension.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
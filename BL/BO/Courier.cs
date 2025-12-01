namespace BO;
using Helpers;
using System;

/// <summary>
/// Business-layer representation of a courier (delivery person).
/// Encapsulates identity, contact information, availability and runtime metrics used by the BL layer.
/// </summary>
/// <remarks>
/// - Instances are used by the BL to present courier data to callers and to perform BL-level logic.
/// - Sensitive fields (for example <see cref="Password"/>) are present for the test/demo scenarios in this project;
///   in production you should never store plaintext passwords in memory or persist them without secure hashing.
/// - Computed metrics (<see cref="DeliveredOnTime"/>, <see cref="DeliveredLate"/>) are populated by BL helpers.
/// </remarks>
public class Courier
{
    /// <summary>
    /// Unique courier identifier (immutable after construction).
    /// Must follow application id rules (validated in BL helpers).
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Courier's full name used for display and lookup.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Contact phone number (local format expected by validation rules).
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Contact email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Authentication secret for the courier (demo only). Do not store plaintext in production.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Whether the courier is currently active/available for assignments.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Optional maximum delivery distance the courier is willing/able to travel (kilometers).
    /// Null means no explicit limit (or use system default).
    /// </summary>
    public double? MaxDeliveryDistance { get; set; }

    /// <summary>
    /// Preferred or assigned delivery vehicle/type (car, scooter, bike, on-foot).
    /// </summary>
    public DeliveryType DeliveryType { get; set; }

    /// <summary>
    /// The date/time when the courier started (immutable).
    /// Used for inactivity and history calculations.
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Number of deliveries completed on time (computed by BL).
    /// </summary>
    public int DeliveredOnTime { get; init; }

    /// <summary>
    /// Number of deliveries completed late (computed by BL).
    /// </summary>
    public int DeliveredLate { get; init; }

    /// <summary>
    /// Current order assigned to the courier, or null when none.
    /// Contains runtime delivery and order details used by the UI and BL flows.
    /// </summary>
    public OrderInProgress CurrentOrder { get; set; }

    /// <summary>
    /// Returns a multi-line string showing property names and values.
    /// Uses the <see cref="Helpers.ToStringProperty"/> extension to produce a readable representation.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
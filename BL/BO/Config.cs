namespace BO;

using System;

/// <summary>
/// Runtime configuration values used by the business layer (BL).
/// Encapsulates the application clock, administrative credentials, company location,
/// movement characteristics (speeds), service ranges and time thresholds used across BL.
/// </summary>
/// <remarks>
/// - This DTO is read/write and typically constructed from DAL configuration values.
/// - Times should be interpreted according to the application's clock semantics (prefer UTC where appropriate).
/// - Speeds are expressed in kilometers per hour (km/h). Distances and ranges are in kilometers.
/// - Sensitive values (for example <see cref="ManagerPassword"/>) should be stored/transported securely in production.
/// </remarks>
public class Config
{
    /// <summary>
    /// The application/system clock used by BL for all time-based calculations and comparisons.
    /// Mutating this value advances or sets simulated time for testing/demo scenarios.
    /// Prefer using UTC values to avoid DST/local-time issues.
    /// </summary>
    public DateTime Clock { get; set; }

    /// <summary>
    /// Administrator (manager) numeric identifier used for authorization checks.
    /// </summary>
    public int ManagerId { get; set; }

    /// <summary>
    /// Administrator password. In production, do not keep plaintext passwords here — use secure hashing/secret storage.
    /// </summary>
    public string ManagerPassword { get; set; }

    /// <summary>
    /// Company postal/street address (human-readable). May be null when not configured.
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// Company geographic latitude in decimal degrees (−90.0 .. 90.0). Null if not configured.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Company geographic longitude in decimal degrees (−180.0 .. 180.0). Null if not configured.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Maximum service range (kilometers) used to determine whether an order is serviceable.
    /// Must be non-negative.
    /// </summary>
    public int MaxRange { get; set; }

    /// <summary>
    /// Average car speed (km/h) used for ETA and routing estimates. Must be > 0.
    /// </summary>
    public double AvgCarSpeed { get; set; }

    /// <summary>
    /// Average motorbike / scooter speed (km/h).
    /// </summary>
    public double AvgMotorbikeSpeed { get; set; }

    /// <summary>
    /// Average bicycle speed (km/h).
    /// </summary>
    public double AvgBicycleSpeed { get; set; }

    /// <summary>
    /// Average walking speed (km/h).
    /// </summary>
    public double AvgWalkingSpeed { get; set; }

    /// <summary>
    /// Maximum allowed delivery duration (TimeSpan). Used to compute SLA and late detection.
    /// </summary>
    public TimeSpan MaxDeliveryTime { get; set; }

    /// <summary>
    /// Risk window (TimeSpan) before the SLA deadline where an order is considered "at risk".
    /// </summary>
    public TimeSpan RiskRange { get; set; }

    /// <summary>
    /// Inactivity timeout (TimeSpan) after which a courier is considered inactive when no updates are received.
    /// </summary>
    public TimeSpan InactivityRange { get; set; }
}

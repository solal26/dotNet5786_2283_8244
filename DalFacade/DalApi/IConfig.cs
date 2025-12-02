namespace DalApi;

using System;

/// <summary>
/// Provides application-wide configuration and administrative settings
/// (runtime clock, manager credentials, location, speed/range settings and timeouts).
/// </summary>
/// <remarks>
/// - This interface does not mandate persistence strategy (in-memory, file, DB); implementations must document persistence semantics.
/// - All time values are represented by <see cref="DateTime"/> and <see cref="TimeSpan"/>. Prefer UTC for <see cref="Clock"/> to avoid DST issues.
/// - Speeds are expressed in kilometers per hour (km/h). Ranges are in kilometers.
/// </remarks>
/// <remarks>
/// - This interface does not mandate persistence strategy (in-memory, file, DB); implementations must document persistence semantics.
/// - All time values are represented by <see cref="DateTime"/> and <see cref="TimeSpan"/>. Prefer UTC for <see cref="Clock"/> to avoid DST issues.
/// - Speeds are expressed in kilometers per hour (km/h). Ranges are in kilometers.
/// </remarks>
/// <remarks>
/// - This interface does not mandate persistence strategy (in-memory, file, DB); implementations must document persistence semantics.
/// - All time values are represented by <see cref="DateTime"/> and <see cref="TimeSpan"/>. Prefer UTC for <see cref="Clock"/> to avoid DST issues.
/// - Speeds are expressed in kilometers per hour (km/h). Ranges are in kilometers.
/// </remarks>
/// <remarks>
/// - This interface does not mandate persistence strategy (in-memory, file, DB); implementations must document persistence semantics.
/// - All time values are represented by <see cref="DateTime"/> and <see cref="TimeSpan"/>. Prefer UTC for <see cref="Clock"/> to avoid DST issues.
/// - Speeds are expressed in kilometers per hour (km/h). Ranges are in kilometers.
/// </remarks>
   public interface IConfig
{
    /// <summary>
    /// System clock (read/write). Used to control simulated or current time.
    /// </summary>
    DateTime Clock { get; set; }

    /// <summary>
    /// Manager identifier for administrative operations.
    /// </summary>
    int ManagerId { get; set; }

    /// <summary>
    /// Manager password for administrative access.
    /// </summary>
    string ManagerPassword { get; set; }

    /// <summary>
    /// Company address (admin-modifiable).
    /// </summary>
    string? CompanyAddress { get; set; }

    /// <summary>
    /// Company location latitude (admin-modifiable).
    /// </summary>
    double? Latitude { get; set; }

    /// <summary>
    /// Company location longitude (admin-modifiable).
    /// </summary>
    double? Longitude { get; set; }

    /// <summary>
    /// Maximum service range (e.g., kilometers) for deliveries.
    /// </summary>
    int MaxRange { get; set; }

    /// <summary>
    /// Average car speed in units consistent with distance/time settings.
    /// </summary>
    double AvgCarSpeed { get; set; }

    /// <summary>
    /// Average motorbike speed.
    /// </summary>
    double AvgMotorBikeSpeed { get; set; }

    /// <summary>
    /// Average bicycle speed.
    /// </summary>
    double AvgBicycleSpeed { get; set; }

    /// <summary>
    /// Average walking speed.
    /// </summary>
    double AvgWalkingSpeed { get; set; }

    /// <summary>
    /// Maximum allowed delivery time.
    /// </summary>
    TimeSpan MaxDeliveryTime { get; set; }

    /// <summary>
    /// Time threshold considered a risky delivery window.
    /// </summary>
    TimeSpan RiskRange { get; set; }

    /// <summary>
    /// Time of inactivity after which a courier/order is considered inactive.
    /// </summary>
    TimeSpan InactivityRange { get; set; }

    /// <summary>
    /// Reset configuration to default values.
    /// </summary>
    void Reset();
}


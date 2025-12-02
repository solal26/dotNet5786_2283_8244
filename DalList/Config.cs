namespace Dal;

/// <summary>
/// Central configuration and runtime constants for the DAL list implementation.
/// Holds default values, incremental id generators and runtime-adjustable settings
/// used across the in-memory data store.
/// </summary>
internal static class Config
{
    // Identifiers for runners
    /// <summary>
    /// Starting value for automatically generated order identifiers.
    /// </summary>
    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;
    /// <summary>
    /// Returns the next order identifier and advances the internal counter.
    /// Use when creating a new order to obtain a unique id.
    /// </summary>
    internal static int NextOrderId => nextOrderId++;

    internal const int startDeliveryId = 5000;
    private static int nextDeliveryId = startDeliveryId;
    /// <summary>
    /// Returns the next delivery identifier and advances the internal counter.
    /// Use when creating a new delivery to obtain a unique id.
    /// </summary>
    internal static int NextDeliveryId => nextDeliveryId++;

    // System clock
    /// <summary>
    /// System-wide clock used by the DAL for deterministic timestamps during tests.
    /// It can be advanced in the test harness to simulate time passing.
    /// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // Manager details
    /// <summary>
    /// ID of the manager user. Default is 0 (not set).
    /// </summary>
    internal static int ManagerId { get; set; } = 0;        // Manager ID
    /// <summary>
    /// Manager password used for simple authentication in the demo DAL.
    /// Change in production systems to a secure store.
    /// </summary>
    internal static string ManagerPassword { get; set; } = "1234"; // Initial password

    // Company address and coordinates
    /// <summary>
    /// Optional company address (human-readable) used for distance calculations.
    /// </summary>
    internal static string? CompanyAddress { get; set; } = null;
    /// <summary>
    /// Optional latitude of the company location. Null when unknown.
    /// </summary>
    internal static double? Latitude { get; set; } = null;
    /// <summary>
    /// Optional longitude of the company location. Null when unknown.
    /// </summary>
    internal static double? Longitude { get; set; } = null;

    // General variables
    /// <summary>
    /// Maximum delivery range (in kilometers) used by routing and assignment logic.
    /// </summary>
    internal static int MaxRange { get; set; } = 4; // Maximum delivery range
    /// <summary>
    /// Average driving speed for cars (km/h). Used for ETA approximations.
    /// </summary>
    internal static double AvgCarSpeed { get; set; } = 50;       // in km/h
    /// <summary>
    /// Average speed for motorbikes (km/h). Used for ETA approximations.
    /// </summary>
    internal static double AvgMotorBikeSpeed { get; set; } = 40; // in km/h
    /// <summary>
    /// Average speed for bicycles (km/h). Used for ETA approximations.
    /// </summary>
    internal static double AvgBicycleSpeed { get; set; } = 15;   // in km/h
    /// <summary>
    /// Average walking speed (km/h). Used for ETA approximations.
    /// </summary>
    internal static double AvgWalkingSpeed { get; set; } = 5;    // in km/h

    // Time ranges
    /// <summary>
    /// Maximum allowed delivery time. Defaults to 4 hours.
    /// </summary>
    internal static TimeSpan MaxDeliveryTime { get; set; } = TimeSpan.FromHours(4);
    /// <summary>
    /// Time range considered risky (e.g. for alerts or SLA calculations).
    /// </summary>
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);
    /// <summary>
    /// Inactivity threshold used to detect inactive couriers.
    /// </summary>
    internal static TimeSpan InactivityRange { get; set; } = TimeSpan.FromHours(2);

    // Method to reset all values to initial defaults
    /// <summary>
    /// Reset all configuration values to their initial defaults.
    /// This restores id counters, default speeds, time ranges and clears company info.
    /// Intended for test setup and demo convenience.
    /// </summary>
    internal static void Reset()
    {
        nextOrderId = startOrderId;
        nextDeliveryId = startDeliveryId;

        Clock = DateTime.Now;

        ManagerId = 0;
        ManagerPassword = "1234";

        CompanyAddress = null;
        Latitude = null;
        Longitude = null;

        MaxRange = 4;
        AvgCarSpeed = 50;
        AvgMotorBikeSpeed = 40;
        AvgBicycleSpeed = 15;
        AvgWalkingSpeed = 5;

        MaxDeliveryTime = TimeSpan.FromHours(4);
        RiskRange = TimeSpan.FromHours(1);
        InactivityRange = TimeSpan.FromHours(2);
    }
}


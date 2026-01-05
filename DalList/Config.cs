namespace Dal;
using System.Runtime.CompilerServices;

/// <summary>
/// Central configuration and runtime constants for the DAL list implementation.
/// Holds default values, incremental id generators and runtime-adjustable settings
/// used across the in-memory data store.
/// </summary>
internal static class Config
{
    // Identifiers for runners
    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;

    /// <summary>
    /// Returns the next order identifier and advances the internal counter.
    /// </summary>
    internal static int NextOrderId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextOrderId++;
    }

    internal const int startDeliveryId = 5000;
    private static int nextDeliveryId = startDeliveryId;

    /// <summary>
    /// Returns the next delivery identifier and advances the internal counter.
    /// </summary>
    internal static int NextDeliveryId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextDeliveryId++;
    }

    // System clock
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = DateTime.Now;

    // Manager details
    internal static int ManagerId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = 0;

    internal static string ManagerPassword
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = "1234";

    // Company address and coordinates
    internal static string? CompanyAddress
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = null;

    internal static double? Latitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = null;

    internal static double? Longitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = null;

    // General variables
    internal static int MaxRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = 4;

    internal static double AvgCarSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = 50;

    internal static double AvgMotorBikeSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = 40;

    internal static double AvgBicycleSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = 15;

    internal static double AvgWalkingSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = 5;

    // Time ranges
    internal static TimeSpan MaxDeliveryTime
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = TimeSpan.FromHours(4);

    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = TimeSpan.FromHours(1);

    internal static TimeSpan InactivityRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = TimeSpan.FromHours(2);

    /// <summary>
    /// Reset all configuration values to their initial defaults.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
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
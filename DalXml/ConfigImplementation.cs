namespace Dal;
using DalApi;

/// <summary>
/// Concrete implementation of <see cref="IConfig"/> that delegates operations
/// to the internal static <see cref="Config"/> container used by the in-memory DAL.
/// This class exposes Config values via instance properties so callers use the
/// <see cref="IConfig"/> interface while the storage remains centralized in <see cref="Config"/>.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Global system clock used by the DAL. Reading and writing delegates to <see cref="Config.Clock"/>.
    /// Useful for deterministic timestamps in tests and for advancing simulated time.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Manager identifier. Delegates to <see cref="Config.ManagerId"/>.
    /// </summary>
    public int ManagerId
    {
        get => Config.ManagerId;
        set => Config.ManagerId = value;
    }

    /// <summary>
    /// Manager password used by the demo DAL. Delegates to <see cref="Config.ManagerPassword"/>.
    /// </summary>
    public string ManagerPassword
    {
        get => Config.ManagerPassword;
        set => Config.ManagerPassword = value;
    }

    /// <summary>
    /// Company address (optional). Delegates to <see cref="Config.CompanyAddress"/>.
    /// </summary>
    public string? CompanyAddress
    {
        get => Config.CompanyAddress;
        set => Config.CompanyAddress = value;
    }

    /// <summary>
    /// Company latitude (optional). Delegates to <see cref="Config.Latitude"/>.
    /// </summary>
    public double? Latitude
    {
        get => Config.Latitude;
        set => Config.Latitude = value;
    }

    /// <summary>
    /// Company longitude (optional). Delegates to <see cref="Config.Longitude"/>.
    /// </summary>
    public double? Longitude
    {
        get => Config.Longitude;
        set => Config.Longitude = value;
    }

    /// <summary>
    /// Maximum delivery range in kilometers. Delegates to <see cref="Config.MaxRange"/>.
    /// </summary>
    public int MaxRange
    {
        get => Config.MaxRange;
        set => Config.MaxRange = value;
    }

    /// <summary>
    /// Average car speed (km/h) used for ETA approximations. Delegates to <see cref="Config.AvgCarSpeed"/>.
    /// </summary>
    public double AvgCarSpeed
    {
        get => Config.AvgCarSpeed;
        set => Config.AvgCarSpeed = value;
    }

    /// <summary>
    /// Average motorbike speed (km/h) used for ETA approximations. Delegates to <see cref="Config.AvgMotorbikeSpeed"/>.
    /// </summary>
    public double AvgMotorBikeSpeed
    {
        get => Config.AvgMotorBikeSpeed;
        set => Config.AvgMotorBikeSpeed = value;
    }

    /// <summary>
    /// Average bicycle speed (km/h). Delegates to <see cref="Config.AvgBicycleSpeed"/>.
    /// </summary>
    public double AvgBicycleSpeed
    {
        get => Config.AvgBicycleSpeed;
        set => Config.AvgBicycleSpeed = value;
    }

    /// <summary>
    /// Average walking speed (km/h). Delegates to <see cref="Config.AvgWalkingSpeed"/>.
    /// </summary>
    public double AvgWalkingSpeed
    {
        get => Config.AvgWalkingSpeed;
        set => Config.AvgWalkingSpeed = value;
    }

    /// <summary>
    /// Maximum allowed delivery time. Delegates to <see cref="Config.MaxDeliveryTime"/>.
    /// </summary>
    public TimeSpan MaxDeliveryTime
    {
        get => Config.MaxDeliveryTime;
        set => Config.MaxDeliveryTime = value;
    }

    /// <summary>
    /// Time span considered a risk window (e.g. for alerts/SLA). Delegates to <see cref="Config.RiskRange"/>.
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    /// <summary>
    /// Threshold to detect inactive couriers. Delegates to <see cref="Config.InactivityRange"/>.
    /// </summary>
    public TimeSpan InactivityRange
    {
        get => Config.InactivityRange;
        set => Config.InactivityRange = value;
    }

    /// <summary>
    /// Reset all configuration values to their defaults by calling <see cref="Config.Reset"/>.
    /// This method is exposed through <see cref="IConfig"/> for test and demo convenience.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}



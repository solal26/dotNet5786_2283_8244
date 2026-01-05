namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System;
using System.Threading.Tasks;

/// <summary>
/// BL façade implementing administrative operations (clock, configuration and DB initialization/reset).
/// Delegates behavior to <see cref="AdminManager"/> and exposes the <see cref="IAdmin"/> surface.
/// </summary>
internal class AdminImplementation : IAdmin
{
    /// <summary>
    /// Advance the system clock by a unit.
    /// </summary>
    /// <param name="unit">Requested unit to advance (currently ignored).</param>
    /// <remarks>
    /// Current implementation advances the clock by one minute via <see cref="AdminManager.UpdateClock(DateTime)"/>.
    /// The <paramref name="unit"/> parameter is accepted for the API but not applied — update implementation if other units are required.
    /// </remarks>
    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        var now = AdminManager.Now;
        DateTime newTime = unit switch
        {
            TimeUnit.Minute => now.AddMinutes(1),
            TimeUnit.Hour => now.AddHours(1),
            TimeUnit.Day => now.AddDays(1),
            TimeUnit.Month => now.AddMonths(1),
            TimeUnit.Year => now.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit))
        };

        AdminManager.UpdateClock(newTime);
    }


    /// <summary>
    /// Returns the current system time used by the application (local system time).
    /// </summary>
    /// <returns>The current <see cref="DateTime"/>.</returns>
    public DateTime GetClock() => AdminManager.Now;

    /// <summary>
    /// Read current runtime configuration.
    /// </summary>
    /// <returns>Business-layer <see cref="Config"/> instance from <see cref="AdminManager"/>.</returns>
    public async Task<Config> GetConfig() =>  await AdminManager.GetConfig();

    /// <summary>
    /// Initialize the demo database and seed sample data.
    /// </summary>
    /// <remarks>
    /// Delegates seeding and setup to <see cref="AdminManager.InitializeDB"/>; intended for test/demo use.
    /// </remarks>
    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB();
    }

    /// <summary>
    /// Reset database and configuration to defaults.
    /// </summary>
    /// <remarks>
    /// Delegates the reset operation to <see cref="AdminManager.ResetDB"/>.
    /// </remarks>
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB();
    }

    /// <summary>
    /// Replace runtime configuration with the provided <paramref name="config"/>.
    /// </summary>
    /// <param name="config">Configuration values to apply.</param>
    public void SetConfig(Config config)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.SetConfig(config);
    }
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator() { //stage 7
        AdminManager.Stop(); //stage 7
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5


}

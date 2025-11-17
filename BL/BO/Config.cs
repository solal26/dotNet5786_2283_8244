namespace BO;

using System;

public class Config
{
    public DateTime Clock { get; set; }

    public int ManagerId { get; set; }

    public string ManagerPassword { get; set; }

    public string? CompanyAddress { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public int MaxRange { get; set; }

    public double AvgCarSpeed { get; set; }

    public double AvgMotorbikeSpeed { get; set; }

    public double AvgBicycleSpeed { get; set; }

    public double AvgWalkingSpeed { get; set; }

    public TimeSpan MaxDeliveryTime { get; set; }

    public TimeSpan RiskRange { get; set; }

    public TimeSpan InactivityRange { get; set; }
}

namespace BO;

using System;

public class Config
{
    DateTime Clock { get; set; }

    int ManagerId { get; set; }

    string ManagerPassword { get; set; }

    string? CompanyAddress { get; set; }

    double? Latitude { get; set; }

    double? Longitude { get; set; }

    int MaxRange { get; set; }

    double AvgCarSpeed { get; set; }

    double AvgMotorbikeSpeed { get; set; }

    double AvgBicycleSpeed { get; set; }

    double AvgWalkingSpeed { get; set; }

    TimeSpan MaxDeliveryTime { get; set; }

    TimeSpan RiskRange { get; set; }

    TimeSpan InactivityRange { get; set; }

    
}

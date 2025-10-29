namespace DalApi;

public interface IConfig
{
 
    // System clock (Read/Write)
    DateTime Clock { get; set; }

    // Manager details (Read/Write)
    int ManagerId { get; set; }
    string ManagerPassword { get; set; }

    // Admin-modifiable properties (Read/Write)
    string? CompanyAddress { get; set; }
    double? Latitude { get; set; }
    double? Longitude { get; set; }
    int MaxRange { get; set; }
    double AvgCarSpeed { get; set; }
    // ... all other speeds
    double AvgMotorbikeSpeed { get; set; }
    double AvgBicycleSpeed { get; set; }
    double AvgWalkingSpeed { get; set; }

    //Time ranges (Read/Write)
    TimeSpan MaxDeliveryTime { get; set; }
    TimeSpan RiskRange { get; set; }
    TimeSpan InactivityRange { get; set; }

    //Action methods (Administrative operations)
    void Reset();
}


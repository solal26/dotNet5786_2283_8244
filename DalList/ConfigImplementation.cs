

namespace Dal;
using DalApi;
using System;

public class ConfigImplementation : IConfig
{
    
   
    public DateTime Clock { get => Config.Clock; set => throw new NotImplementedException(); }
    public int ManagerId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ManagerPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string? CompanyAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? Latitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double? Longitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int MaxRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgCarSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgMotorbikeSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgBicycleSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public double AvgWalkingSpeed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan MaxDeliveryTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan RiskRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public TimeSpan InactivityRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}

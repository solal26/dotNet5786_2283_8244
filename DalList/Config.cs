namespace Dal;

internal static class Config
{
    // Identifiers for runners
    internal const int startOrderId = 1000;
    private static int nextOrderId = startOrderId;
    internal static int NextOrderId => nextOrderId++;

    internal const int startDeliveryId = 5000;
    private static int nextDeliveryId = startDeliveryId;
    internal static int NextDeliveryId => nextDeliveryId++;

    // System clock
    internal static DateTime Clock { get; set; } = DateTime.Now;

    // Manager details
    internal static int ManagerId { get; set; } = 0;        // Manager ID
    internal static string ManagerPassword { get; set; } = "1234"; // Initial password

    // Company address and coordinates
    internal static string? CompanyAddress { get; set; } = null;
    internal static double? Latitude { get; set; } = null;
    internal static double? Longitude { get; set; } = null;

    // General variables
    internal static int MaxRange { get; set; } = 4; // Maximum delivery range
    internal static double AvgCarSpeed { get; set; } = 50;       // in km/h
    internal static double AvgMotorbikeSpeed { get; set; } = 40; // in km/h
    internal static double AvgBicycleSpeed { get; set; } = 15;   // in km/h
    internal static double AvgWalkingSpeed { get; set; } = 5;    // in km/h

    // Time ranges
    internal static TimeSpan MaxDeliveryTime { get; set; } = TimeSpan.FromHours(4);
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);
    internal static TimeSpan InactivityRange { get; set; } = TimeSpan.FromHours(2);

    // Method to reset all values to initial defaults
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
        AvgMotorbikeSpeed = 40;
        AvgBicycleSpeed = 15;
        AvgWalkingSpeed = 5;

        MaxDeliveryTime = TimeSpan.FromHours(4);
        RiskRange = TimeSpan.FromHours(1);
        InactivityRange = TimeSpan.FromHours(2);
    }
}

 


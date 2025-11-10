
namespace Dal;

internal static class Config
{

    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_couriers_xml = "couriers.xml";
    internal const string s_deliveries_xml = "deliveries.xml";
    internal const string s_orders_xml = "orders.xml";
    internal static int nextOrderId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "nextOrderId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "nextOrderId", value);
    }
    internal static int nextDeliveryId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "nextDeliveryId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "nextDeliveryId", value);
    }	

    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static int ManagerId
    {
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerId");
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerId", value);
    }
    internal static string ManagerPassword
    {
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPassword");
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPassword", value);
    }
    internal static string? CompanyAddress
    {
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "CompanyAddress");
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value);
    }
    internal static double? Latitude
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Latitude");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", value);
    }
    internal static double? Longitude
        {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Longitude");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", value);
    }
    internal static int MaxRange
    {
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "MaxRange");
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "MaxRange", value);
    }
    internal static double AvgCarSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed", value);
    }
    internal static double AvgMotorbikeSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgMotorbikeSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgMotorbikeSpeed", value);
    }
    internal static double AvgBicycleSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed", value);
    }
    internal static double AvgWalkingSpeed
    {
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed");
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed", value);
    }
    internal static TimeSpan MaxDeliveryTime
    {
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "MaxDeliveryTime");
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "MaxDeliveryTime", value);
    }
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "RiskRange", value);
    }
    internal static TimeSpan InactivityRange
    {
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "InactivityRange");
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "InactivityRange", value);
    }










    internal static void Reset()
    {
        nextOrderId = 1000;
        nextDeliveryId = 5000;  
        //...
        Clock = DateTime.Now;
        //...
    }

}

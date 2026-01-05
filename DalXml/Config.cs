using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_couriers_xml = "couriers.xml";
    internal const string s_deliveries_xml = "deliveries.xml";
    internal const string s_orders_xml = "orders.xml";

    internal static int NextOrderId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextOrderId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextOrderId", value);
    }

    internal static int NextDeliveryId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextDeliveryId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextDeliveryId", value);
    }

    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static int ManagerId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "ManagerId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "ManagerId", value);
    }

    internal static string ManagerPassword
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "ManagerPassword");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "ManagerPassword", value);
    }

    internal static string? CompanyAddress
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigStringVal(s_data_config_xml, "CompanyAddress");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigStringVal(s_data_config_xml, "CompanyAddress", value);
    }

    internal static double? Latitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Latitude");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Latitude", value);
    }

    internal static double? Longitude
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "Longitude");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "Longitude", value);
    }

    internal static int MaxRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigIntVal(s_data_config_xml, "MaxRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "MaxRange", value);
    }

    internal static double AvgCarSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgCarSpeed", value);
    }

    internal static double AvgMotorBikeSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgMotorBikeSpeed");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgMotorBikeSpeed", value);
    }

    internal static double AvgBicycleSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgBicycleSpeed", value);
    }

    internal static double AvgWalkingSpeed
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDoubleVal(s_data_config_xml, "AvgWalkingSpeed", value);
    }

    internal static TimeSpan MaxDeliveryTime
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "MaxDeliveryTime");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "MaxDeliveryTime", value);
    }

    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "RiskRange", value);
    }

    internal static TimeSpan InactivityRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "InactivityRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "InactivityRange", value);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextOrderId = 1000;
        NextDeliveryId = 5000;
        Clock = DateTime.Now;
    }
}



using System.Net;
using System.Text.Json;
using DalApi;
using DO;

namespace Helpers;

internal static class OrderManager
{
    private static IDal s_dal = Factory.Get;
    internal static void AddOrdeDo(BO.Order boOrder)
    {
        if (boOrder == null)
            throw new ArgumentNullException(nameof(boOrder));

        if (string.IsNullOrWhiteSpace(boOrder.CustomerName))
            throw new ArgumentException("Customer name cannot be empty.", nameof(boOrder.CustomerName));

        if (boOrder.Weight <= 0)
            throw new ArgumentException("Weight must be positive.", nameof(boOrder.Weight));

        // בדיקות מורכבות דרך מתודות עזר
        CourierManager.ValidatePhone(boOrder.CustomerPhone);

        var doOrder = new DO.Order
        {
            OrderId = boOrder.Id,
            typeOfOrder = (TypeOfOrder)boOrder.Type,
            ShippingAddress = boOrder.FullAddress,
            Latitude = ValidateAddress(boOrder.FullAddress).Latitude,
            Longitude = ValidateAddress(boOrder.FullAddress).Longitude,
            CustomerName = boOrder.CustomerName,
            CustomerPhone = boOrder.CustomerPhone,
            weight = boOrder.Weight,
            OrderInitiation = boOrder.OpenTime,
            VerbalDescription = boOrder.Description
        };

        s_dal.Order.Create(doOrder);
    }

    private static readonly HttpClient client = new HttpClient();
    public static (double Latitude, double Longitude) ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));

        string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

        var response = client.GetStringAsync(url).Result; // סינכרוני
        var jsonDoc = JsonDocument.Parse(response);

        var root = jsonDoc.RootElement;
        if (root.GetArrayLength() == 0)
            throw new Exception("Address not found");

        var firstResult = root[0];
        double lat = double.Parse(firstResult.GetProperty("lat").GetString());
        double lon = double.Parse(firstResult.GetProperty("lon").GetString());

        return (lat, lon);
    }

}



using System.Net;
using System.Text.Json;
using BO;
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

        CourierManager.ValidateName(boOrder.CustomerName);
        CourierManager.ValidateWeight(boOrder.Weight);
        CourierManager.ValidatePhone(boOrder.CustomerPhone);
        ValidateFullAddress(boOrder.FullAddress);


        var doOrder = new DO.Order
        {
            OrderId =0,
            typeOfOrder = (TypeOfOrder)boOrder.Type,
            ShippingAddress = boOrder.FullAddress,
            Latitude = ValidateAddress(boOrder.FullAddress).Latitude,
            Longitude = ValidateAddress(boOrder.FullAddress).Longitude,
            CustomerName = boOrder.CustomerName,
            CustomerPhone = boOrder.CustomerPhone,
            weight = boOrder.Weight,
            OrderInitiation = s_dal.Config.Clock,
            VerbalDescription = boOrder.Description
        };

        try
        {
            s_dal.Order.Create(doOrder);
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException("Order with the same ID already exists.", ex);
        }
        
    }

    private static readonly HttpClient client = new HttpClient();
    public static (double Latitude, double Longitude) ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new BlNullPropertyException("Address cannot be empty");

        string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

        var response = client.GetStringAsync(url).Result; // סינכרוני
        var jsonDoc = JsonDocument.Parse(response);

        var root = jsonDoc.RootElement;
        if (root.GetArrayLength() == 0)
            throw new BlInvalidAddressException("Address not found");

        var firstResult = root[0];
        double lat = double.Parse(firstResult.GetProperty("lat").GetString());
        double lon = double.Parse(firstResult.GetProperty("lon").GetString());

        return (lat, lon);
    }
    internal static void ValidateFullAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new BlNullPropertyException("Address cannot be empty.");

        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            throw new BlInvalidValueException("Address must include street, number, city and country.");

        if (!parts.Last().Equals("ישראל", StringComparison.OrdinalIgnoreCase))
            throw new BlInvalidValueException("Address must end with 'ישראל'.");
    }
}



using System;
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
    internal static void CancelOrderDo(int orderId)
    {
       
        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");

        
        var status = CalculateOrderStatus(orderId);

        if (status == BO.OrderStatus.Delivered ||
            status == BO.OrderStatus.Refused ||
            status == BO.OrderStatus.Cancelled)
        {
            throw new BlInvalidRequestException("Cannot cancel an order that is already completed or canceled.");
        }

        var deliveries = s_dal.Delivery.ReadAll()
                        .Where(d => d.OrderId == orderId)
                        .OrderBy(d => d.TimeBegin);

       
        if (status == BO.OrderStatus.Open)
        {
            var dummyDelivery = new DO.Delivery
            {
                OrderId = orderId,
                CourierId = 0, 
                TimeBegin = s_dal.Config.Clock,
                TimeEnd = s_dal.Config.Clock,
                typeofshipment = TypeOfShipment.Car, 
                endcour = Endofcourier.Canceled
            };

            try
            {
                s_dal.Delivery.Create(dummyDelivery);
            }
            catch (DalAlreadyExistsException ex)
            {
                throw new BlAlreadyExistsException(" delivery already exists.", ex);
            }
            
        }
       
        else if (status == BO.OrderStatus.InProgress)
        {
            var activeDelivery = deliveries.FirstOrDefault(d => d.TimeEnd == null);

            var updatedDelivery = activeDelivery with
            {
                TimeEnd = s_dal.Config.Clock,
                endcour = Endofcourier.Canceled
            };
            try
            {
                s_dal.Delivery.Update(updatedDelivery);
            }
            catch (DalAlreadyExistsException ex)
            {
                throw new BlDoesNotExistException(" delivery does not exist.", ex);
            }
           
        }
    }



    internal static BO.OrderStatus CalculateOrderStatus(int orderId)
    {
        
        try
        {
            var order = s_dal.Order.Read(orderId);
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BlDoesNotExistException("order does not exist.", ex);
        }


        var deliveries = s_dal.Delivery.ReadAll()
                            .Where(d => d.OrderId == orderId)
                            .OrderByDescending(d => d.TimeBegin)
                            .ToList();

        if (!deliveries.Any())
            return BO.OrderStatus.Open;

        var activeDelivery = deliveries.FirstOrDefault(d => d.TimeEnd == null);
        if (activeDelivery != null)
            return BO.OrderStatus.InProgress;

        var lastDelivery = deliveries.First();
        return lastDelivery.endcour switch
        {
            Endofcourier.Delivered => BO.OrderStatus.Delivered,
            Endofcourier.Refused => BO.OrderStatus.Refused,
            Endofcourier.Canceled => BO.OrderStatus.Cancelled,
            _ => BO.OrderStatus.Open
        };
    }
    internal static void ChooseOrderForDeliveryDo(int courierId, int orderId)
    {
        var docourier = s_dal.Courier.Read(courierId);
        var doOrder = s_dal.Order.Read(orderId);
        if (doOrder == null)
            throw new BlDoesNotExistException($"Order with ID {orderId} not found.");
        if (docourier == null)
            throw new BlDoesNotExistException($"Courier with ID {courierId} not found.");


        var orderStatus = CalculateOrderStatus(orderId);
        if (orderStatus != BO.OrderStatus.Open)
            throw new BlInvalidRequestException("Cannot choose this order: it's not open for delivery.");

        var existingDelivery = s_dal.Delivery.ReadAll()
            .FirstOrDefault(d => d.OrderId == orderId && d.TimeEnd == null);

        if (existingDelivery != null)
            throw new BlInvalidRequestException("Cannot choose this order: it is already in progress.");

        var newDelivery = new DO.Delivery
        {
            Id = 0,
            OrderId = orderId,
            CourierId = courierId,
            TimeBegin = s_dal.Config.Clock,
            TimeEnd = null,
            endcour = null,
            typeofshipment = docourier.typeofshipment
        };

        try
        {
            s_dal.Delivery.Create(newDelivery);
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException("Delivery for this order already exists.", ex);
        }
    }
}
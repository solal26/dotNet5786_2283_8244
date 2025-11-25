

using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;
    internal static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Id must be positive.");
    }

    internal static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new ArgumentException("Name must be at least 2 characters.");
    }

    internal static void ValidatePhone(string phone)
    {
        if (phone == null || phone.Length != 10 || !phone.StartsWith("0") || !long.TryParse(phone, out _))
            throw new ArgumentException("Phone must be 10 digits starting with 0.");
    }

    internal static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            throw new ArgumentException("Invalid email format.");
    }

    internal static void ValidateAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty.");
    }

    internal static void ValidateDistance(double? distance)
    {
        if (distance.HasValue && distance < 0)
            throw new ArgumentException("Distance cannot be negative.");
    }

    internal static bool HasPermission(int requesterId, int courierId)
    {
        
       if (s_dal.Config.ManagerId == requesterId)
            return true;    
        if (requesterId == courierId)
            return true;
        return false;
    }
    internal static IEnumerable<BO.CourierInList> GetAllCouriersInList()
    {
        var allDoCouriers = s_dal.Courier.ReadAll(); 

        return allDoCouriers.Select(c => new BO.CourierInList
        {
            Id = c.Id,
            FullName = c.Name,
            Active = c.Active,
            DeliveryType = (DeliveryType)c.typeofshipment,
            StartDate = c.TimeBegin,
            DeliveredOnTime = CalculateDeliveredOnTime(c.Id),
            DeliveredLate = CalculateDeliveredLate(c.Id),
            CurrentOrderId = GetCurrentOrder(c.Id)?.OrderId
        });
    }


    internal static void PeriodicCouriersUpdates()
    {
        DateTime now = s_dal.Config.Clock;
        TimeSpan inactivityLimit = s_dal.Config.InactivityRange;

        var doCouriers = s_dal.Courier.ReadAll();
        var allDeliveries = s_dal.Delivery.ReadAll();

        foreach (var doCourier in doCouriers)
        {
            var boCourier = ConvertToBO(doCourier.Id);

            if (boCourier.CurrentOrder != null)
            {
                if (!doCourier.Active)
                {
                    var updated = doCourier with { Active = true };
                    s_dal.Courier.Update(updated);
                }
                continue;
            }

            var finishedDeliveries = allDeliveries
                .Where(d => d.CourierId == doCourier.Id && d.TimeEnd.HasValue)
                .ToList();

            DateTime? lastDeliveryTime = finishedDeliveries.Any()
                ? finishedDeliveries.Max(d => d.TimeEnd.Value)
                : doCourier.TimeBegin;

            if (lastDeliveryTime + inactivityLimit <= now)
            {
                var updated = doCourier with { Active = false };
                s_dal.Courier.Update(updated);
            }
        }
    }
    internal static BO.Courier ConvertToBO(int id)
    {

        var doCourier = s_dal.Courier.Read(id);
        if (doCourier == null)
            throw new KeyNotFoundException($"Courier with ID {id} not found.");

        return new BO.Courier
        {
            Id = doCourier.Id,
            FullName = doCourier.Name,
            Phone = doCourier.PhoneNumber,
            Email = doCourier.Email,
            Active =doCourier.Active,
            MaxDeliveryDistance = doCourier.Distance,
            DeliveryType = (DeliveryType)doCourier.typeofshipment,
            StartDate = doCourier.TimeBegin,
            DeliveredOnTime = CalculateDeliveredOnTime(doCourier.Id),
            DeliveredLate = CalculateDeliveredLate(doCourier.Id),
            CurrentOrder = GetCurrentOrder(doCourier.Id)!
        };
    }
    internal static int CalculateDeliveredOnTime(int courierId)
    {
        var deliveries = s_dal.Delivery.ReadAll();
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value - d.TimeBegin <= s_dal.Config.MaxDeliveryTime)
            .Count();
    }
    internal static int CalculateDeliveredLate(int courierId)
    {
        var deliveries = s_dal.Delivery.ReadAll();
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value - d.TimeBegin > s_dal.Config.MaxDeliveryTime)
            .Count();
            
    }



    internal static BO.OrderInProgress? GetCurrentOrder(int courierId)
    {
        DateTime currentClock = s_dal.Config.Clock;
        var allDeliveries = s_dal.Delivery.ReadAll();
        var allOrders = s_dal.Order.ReadAll();        

       
        var delivery = allDeliveries
           .FirstOrDefault(d => d.CourierId == courierId && d.TimeBegin <= currentClock && d.TimeEnd == null);
          
        if (delivery == null)
            return null;

        var order = allOrders.FirstOrDefault(o => o.OrderId == delivery.OrderId);
        if (order == null)
            return null;

        DateTime maxDelivery = CalculateMaxDeliveryTime(order.OrderInitiation);
        double Airdistance = CalculateAirDistance(order.Latitude, order.Longitude);

        return new BO.OrderInProgress
        {
            DeliveryId = delivery.Id,
            OrderId = delivery.OrderId,
            Type = (OrderType)(order.typeOfOrder),
            Description = order.VerbalDescription ?? "",
            FullAddress = order.ShippingAddress ?? "",
            AirDistance = Airdistance,
            ActualDistance = delivery.Distance,
            CustomerName = order.CustomerName ?? "",
            CustomerPhone = order.CustomerPhone ?? "",
            OpenTime = delivery.TimeBegin,
            StartTime = currentClock,
            ExpectedDeliveryTime = CalculateExpectedDeliveryTime(delivery.typeofshipment, Airdistance),
            MaxDeliveryTime = maxDelivery,
            OrderStatus = CalculateOrderStatus(order.OrderId),
            ScheduleStatus = CalculateScheduleStatus(order.OrderId),
            TimeRemaining = maxDelivery - currentClock
        };
    }
    internal static BO.OrderStatus CalculateOrderStatus(int orderId)
    {
        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");

      
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
    internal static BO.ScheduleStatus CalculateScheduleStatus(int orderId)
    {
        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new KeyNotFoundException($"Order with ID {orderId} not found.");

        DateTime maxDeliveryTime = order.OrderInitiation.Add(s_dal.Config.MaxDeliveryTime);
        TimeSpan timeRemaining = maxDeliveryTime - s_dal.Config.Clock;

        if (timeRemaining > s_dal.Config.RiskRange)
            return BO.ScheduleStatus.OnTime;
        else if (timeRemaining > TimeSpan.Zero)
            return BO.ScheduleStatus.InRisk;
        else
            return BO.ScheduleStatus.Late;
    }

    internal static DateTime CalculateExpectedDeliveryTime(TypeOfShipment t, double distanceKm)
    {
        double speedKmh = t switch
        {
           TypeOfShipment.Car => s_dal.Config.AvgCarSpeed,
           TypeOfShipment.Bike => s_dal.Config.AvgBicycleSpeed,
           TypeOfShipment.OnFoot => s_dal.Config.AvgWalkingSpeed,
           TypeOfShipment.Scooter => s_dal.Config.AvgMotorbikeSpeed,
            _ => s_dal.Config.AvgCarSpeed
        };

        double hours = distanceKm / speedKmh;
        return s_dal.Config.Clock.AddHours(hours);
    }

    internal static DateTime CalculateMaxDeliveryTime(DateTime Open)
    {
        return Open.Add(s_dal.Config.MaxDeliveryTime);
    }

    private static double CalculateAirDistance(double destLat, double destLon)
    {
        if (!s_dal.Config.Latitude.HasValue || !s_dal.Config.Longitude.HasValue)
            throw new InvalidOperationException("Company coordinates are not set in Config.");

        double lat1 = s_dal.Config.Latitude.Value;
        double lon1 = s_dal.Config.Longitude.Value;
        double lat2 = destLat;
        double lon2 = destLon;

        double R = 6371; 
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double angle) => angle * Math.PI / 180.0;

}






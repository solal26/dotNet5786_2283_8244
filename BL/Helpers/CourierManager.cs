

using BlApi;
using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = DalApi.Factory.Get;
    internal static void ValidateId(int id)
    {
        if (id <100000000||id>999999999)
            throw new BlInvalidValueException("Id not valid.");
    }
    internal static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new BlInvalidValueException("Name must be at least 2 characters.");
    }
    internal static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new BlInvalidValueException("Phone number cannot be empty.");

        if (phone.Length != 10)
            throw new BlInvalidValueException("Phone number must be exactly 10 digits long.");

        if (!phone.StartsWith("0"))
            throw new BlInvalidValueException("Phone number must start with '0'.");

        if (!long.TryParse(phone, out _))
            throw new BlInvalidValueException("Phone number must contain only digits.");
    }
    internal static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BlNullPropertyException("Email cannot be empty.");

        int atCount = email.Count(c => c == '@');
        if (atCount != 1)
            throw new BlInvalidValueException("Email must contain exactly one '@'.");

        int atIndex = email.IndexOf('@');
        string localPart = email.Substring(0, atIndex);
        string domainPart = email.Substring(atIndex + 1).ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(localPart))
            throw new BlInvalidValueException("Email local part cannot be empty.");

        string[] validDomains = { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "icloud.com" };
        if (!validDomains.Contains(domainPart))
            throw new BlInvalidValueException($"Email domain must be one of: {string.Join(", ", validDomains)}");
    }
    internal static void ValidateDistance(double? distance)
    {
        if (distance.HasValue && distance < 0)
            throw new BlInvalidValueException("Distance cannot be negative.");
    }
    internal static void ValidateWeight(double Weight)
    {
        if (Weight < 0)
            throw new BlInvalidValueException("Weight cannot be negative.");
    }
    internal static bool HasPermission(int requesterId, int courierId = 0)
    {
      
        if (s_dal.Config.ManagerId == requesterId)
            return true;
        if (requesterId == courierId && courierId != 0)
            return true;
        return false;
    }
    internal static void ValidateStartDate(DateTime start)
    {
        if (start > s_dal.Config.Clock)
            throw new BlInvalidDateException("Start date cannot be in the future.");
    }
    internal static void AddCourierDo(BO.Courier courier)
    {
        if (courier == null)
            throw new BlNullPropertyException(nameof(courier));
        ValidateId(courier.Id);
        ValidateName(courier.FullName);
        ValidatePhone(courier.Phone);
        ValidateEmail(courier.Email);
        ValidateDistance(courier.MaxDeliveryDistance);
        ValidateStartDate(courier.StartDate);

        
        var doCourier = new DO.Courier
        {
            Id = courier.Id,
            Name = courier.FullName,
            PhoneNumber = courier.Phone,
            Email = courier.Email,
            Code = courier.Password,
            Active = courier.Active,
            Distance = courier.MaxDeliveryDistance,
            typeofshipment = (DO.TypeOfShipment)courier.DeliveryType,
            TimeBegin = courier.StartDate
        };

        try
        {
            s_dal.Courier.Create(doCourier);
        }
        catch (DalAlreadyExistsException ex)
        {
            throw new BlAlreadyExistsException("Courier with the same ID already exists.", ex);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException("Related entity does not exist.", ex);
        }
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
            OrderStatus = OrderManager.CalculateOrderStatus(order.OrderId),
            ScheduleStatus = CalculateScheduleStatus(order.OrderId),
            TimeRemaining = maxDelivery - currentClock
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








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

        //DateTime expectedDelivery = delivery.TimeBegin.AddHours(1);
        //DateTime maxDelivery = delivery.TimeBegin.AddHours(2);

        double airDistance = CalculateAirDistance(
            32.0853, 34.7818,  // קואורדינטות החברה (לדוגמה)
            order.Latitude, order.Longitude
        );

        return new BO.OrderInProgress
        {
            DeliveryId = delivery.Id,
            OrderId = delivery.OrderId,
            Type = (OrderType)(order.typeOfOrder),
            Description = order.VerbalDescription ?? "",
            FullAddress = order.ShippingAddress ?? "",
            AirDistance = airDistance,
            ActualDistance = delivery.Distance,
            CustomerName = order.CustomerName ?? "",
            CustomerPhone = order.CustomerPhone ?? "",
            OpenTime = delivery.TimeBegin,
            StartTime = currentClock,
            ExpectedDeliveryTime = expectedDelivery,
            MaxDeliveryTime = maxDelivery,
            OrderStatus = BO.OrderStatus.InProgress,
            ScheduleStatus = BO.ScheduleStatus.OnTime,
            TimeRemaining = maxDelivery - currentClock
        };
    }






    private static double CalculateAirDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371; // רדיוס כדור הארץ בקילומטרים
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






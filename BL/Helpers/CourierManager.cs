

using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;
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
        DateTime newClock = s_dal.Config.Clock;
        var doCouriers = s_dal.Courier.ReadAll();
    
        foreach (var doCourier in doCouriers)
        {

            var boCourier = ConvertToBO(doCourier.Id);
          
        }
    }

    internal static BO.Courier ConvertToBO(int id)
    {

        var doCourier = s_dal.Courier.Read(id);
        if (doCourier == null)
            throw new KeyNotFoundException($"Courier with ID {id} not found.");

        var allDeliveries = s_dal.Delivery.ReadAll();

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
            DeliveredOnTime = CalculateDeliveredOnTime(doCourier.Id, allDeliveries),
            DeliveredLate = CalculateDeliveredLate(doCourier.Id, allDeliveries),
            CurrentOrder = GetCurrentOrder(doCourier.Id, allDeliveries.ToList())
        };
    }
    internal static int CalculateDeliveredOnTime(int courierId, IEnumerable<DO.Delivery> deliveries)
    {
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value - d.TimeBegin <= s_dal.Config.MaxDeliveryTime)
            .Count();
    }
    internal static int CalculateDeliveredLate(int courierId, IEnumerable<DO.Delivery> deliveries)
    {
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value - d.TimeBegin > s_dal.Config.MaxDeliveryTime)
            .Count();
            
    }

    internal static BO.OrderInProgress? GetCurrentOrder(int courierId, List<DO.Delivery> allDeliveries)
    {
        var doCourier = s_dal.Courier.Read(courierId);
        if (doCourier == null)
            throw new KeyNotFoundException($"Courier with ID {courierId} not found.");
        var allOrders = s_dal.Order.ReadAll().ToList();
        DateTime currentClock = s_dal.Config.Clock;
    


        double airDistance = CalculateAirDistance(
            doCourier.Latitude ?? 0, // אם אין ערך, נניח 0
            doCourier.Longitude ?? 0,
            order.Latitude,
            order.Longitude
        );

        return new BO.OrderInProgress()
        {
            DeliveryId = delivery.Id,
            OrderId = delivery.OrderId,
            Type = MapOrderType(delivery.typeofshipment),
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

    // חישוב מרחק אווירי לפי קואורדינטות
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


    private static BO.OrderType MapOrderType(TypeOfShipment typeOfShipment)
    {
        return typeOfShipment switch
        {
            TypeOfShipment.Car => BO.OrderType.Car,
            TypeOfShipment.Bike => BO.OrderType.Bike,
            TypeOfShipment.Foot => BO.OrderType.Foot,
            _ => BO.OrderType.Other
        };
    }
}






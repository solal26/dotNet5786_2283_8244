

using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CourierManager
{
    private static IDal s_dal = Factory.Get;

    internal static void PeriodicCouriersUpdates(DateTime oldClock, DateTime newClock)
    {
        var doCouriers = s_dal.Courier.ReadAll();
        var allDeliveries = s_dal.Delivery.ReadAll();
        var allOrders = s_dal.Order.ReadAll();

        foreach (var doCourier in doCouriers)
        {
            var boCourier = ConvertToBO(doCourier, allDeliveries, allOrders, newClock);

            // כאן אפשר לעדכן את ה-BO ברשימה פנימית או לשלוח לממשק שמציג נתונים
            // לדוגמה: UpdateInternalBOList(boCourier);
        }
    }

    internal static BO.Courier ConvertToBO(DO.Courier doCourier, IEnumerable<DO.Delivery> allDeliveries, IEnumerable<DO.Order> allOrders, DateTime currentClock)
    {
        if (doCourier == null)
            throw new DalDoesNotExistException("Courier does not exist in the system.");

        return new BO.Courier
        {
            Id = doCourier.Id,
            FullName = doCourier.Name,
            Phone = doCourier.PhoneNumber,
            Email = doCourier.Email,
            Active = IsCourierActive(doCourier.Id, allDeliveries, currentClock),
            MaxDeliveryDistance = doCourier.Distance,
            DeliveryType = (DeliveryType)doCourier.typeofshipment,
            StartDate = doCourier.TimeBegin,
            DeliveredOnTime = CalculateDeliveredOnTime(doCourier.Id, allDeliveries, currentClock),
            DeliveredLate = CalculateDeliveredLate(doCourier.Id, allDeliveries, currentClock),
            CurrentOrder = GetCurrentOrder(doCourier.Id, allDeliveries.ToList(), allOrders.ToList(), currentClock)
        };
    }

    private static bool IsCourierActive(int courierId, IEnumerable<DO.Delivery> allDeliveries, DateTime currentClock)
    {
        return allDeliveries.Any(d => d.CourierId == courierId && d.TimeBegin <= currentClock && d.TimeEnd == null);
    }

    internal static int CalculateDeliveredOnTime(int courierId, IEnumerable<DO.Delivery> deliveries, DateTime maxDeliveryTime)
    {
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value <= maxDeliveryTime)
            .Count();
    }

    internal static int CalculateDeliveredLate(int courierId, IEnumerable<DO.Delivery> deliveries, DateTime maxDeliveryTime)
    {
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value > maxDeliveryTime)
            .Count();
    }

    internal static BO.OrderInProgress? GetCurrentOrder(int courierId, List<DO.Delivery> allDeliveries, List<DO.Order> allOrders, DO.Courier doCourier, DateTime currentClock)
    {
        var delivery = allDeliveries
            .Where(d => d.CourierId == courierId && d.TimeBegin <= currentClock && d.TimeEnd == null)
            .OrderBy(d => d.TimeBegin)
            .FirstOrDefault();

        if (delivery == null) return null;

        var order = allOrders.FirstOrDefault(o => o.OrderId == delivery.OrderId);
        if (order == null) return null;

        DateTime expectedDelivery = delivery.TimeBegin.AddHours(1);
        DateTime maxDelivery = delivery.TimeBegin.AddHours(2);

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






using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Linq;
using System.Diagnostics.Metrics;

namespace Helpers;

/// <summary>
/// Business-layer helper that implements courier-related validation, permission checks,
/// DO↔BO mapping and orchestration of DAL operations for courier flows.
/// </summary>
/// <remarks>
/// - Internal to the BL assembly; not exposed as a public service interface.
/// - Centralizes rules for add/update/delete and small utility calculations used across BL.
/// - Keep methods small and side-effect free where possible; DAL IO is performed via <see cref="s_dal"/>.
/// </remarks>
internal static class CourierManager
{
    private static IDal s_dal = DalApi.Factory.Get;

    internal static ObserverManager Observers = new(); //stage 5 
                                                       //...


    /// <summary>
    /// Validates a national id number format used for couriers.
    /// </summary>
    /// <exception cref="BlInvalidValueException">Thrown when the id is outside allowed range.</exception>
    internal static void ValidateId(int id)
    {
        if (id <100000000||id>999999999)
            throw new BlInvalidValueException("Id not valid.");
    }

    /// <summary>
    /// Validates courier full name (non-empty and minimum length).
    /// </summary>
    internal static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new BlInvalidValueException("Name must be at least 2 characters.");
    }

    /// <summary>
    /// Validates phone number format (exactly 10 digits, starts with '0').
    /// </summary>
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

    /// <summary>
    /// Validates email structure and restricts domains to a small allowed list.
    /// </summary>
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

    /// <summary>
    /// Validates optional maximum delivery distance (non-negative).
    /// </summary>
    internal static void ValidateDistance(double? distance)
    {
        if (distance.HasValue && distance < 0)
            throw new BlInvalidValueException("Distance cannot be negative.");
    }

    /// <summary>
    /// Validates weight (non-negative).
    /// </summary>
    internal static void ValidateWeight(double Weight)
    {
        if (Weight < 0)
            throw new BlInvalidValueException("Weight cannot be negative.");
    }

    /// <summary>
    /// Checks whether the requester has permission to operate on a courier (manager or courier themself).
    /// </summary>
    internal static bool HasPermission(int requesterId, int courierId = 0)
    {
        if (s_dal.Config.ManagerId == requesterId)
            return true;
        if (requesterId == courierId && courierId != 0)
            return true;
        return false;
    }

    /// <summary>
    /// Validates that the courier start date is not in the future relative to the system clock.
    /// </summary>
    internal static void ValidateStartDate(DateTime start)
    {
        if (start > AdminManager.Now)
            throw new BlInvalidDateException("Start date cannot be in the future.");
    }

    /// <summary>
    /// Adds a courier to the DAL after validation and DO mapping.
    /// </summary>
    /// <exception cref="BlAlreadyExistsException">When DAL reports a duplicate id.</exception>
    internal static void AddCourierDo(BO.Courier courier)
    {
        if (courier == null)
            throw new BlNullPropertyException(nameof(courier));
        ValidateId(courier.Id);
        ValidateName(courier.FullName);
        ValidatePhone(courier.Phone);
        ValidateEmail(courier.Email);
        ValidateDistance(courier.MaxDeliveryDistance);
        

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
            TimeBegin = AdminManager.Now
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
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Deletes a courier after permission and delivery-history checks.
    /// </summary>
    internal static void DeleteCourier(int requesterId, int courierId)
    {
        if (!HasPermission(requesterId,courierId))
        {
            throw new BlUnauthorizedAccessException($"Requester ID={requesterId} is not authorized to delete a courier. Only the administrator is allowed.");
        }

        // Check if the courier is currently in delivery (DeliveryEndTime == null)
        bool isInDelivery = s_dal.Delivery.ReadAll(d => d.CourierId == courierId && d.TimeEnd == null).Any();

        if (isInDelivery)
        {
            throw new BlInvalidRequestException($"Courier ID={courierId} cannot be deleted because they are currently handling a delivery.");
        }

        // Check if the courier has any past delivery history (DeliveryEndTime != null)
        bool hasPastDelivery = s_dal.Delivery.ReadAll(d => d.CourierId == courierId && d.TimeEnd != null).Any();

        if (hasPastDelivery)
        {
            throw new BlInvalidRequestException($"Courier ID={courierId} cannot be deleted because they have a delivery history.");
        }

        try
        {
            s_dal.Courier.Delete(courierId);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Courier ID={courierId} does not exist.", ex);
        }
        Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Updates a courier after permission, validation and business-rule checks.
    /// </summary>
    internal static void UpdateCourier(int requesterId, BO.Courier boCourier)
    {
        // 1. Permission Check
        if (!HasPermission(requesterId, boCourier.Id))
        {
            // If the requester is neither the Admin nor the Courier himself.
            throw new BlUnauthorizedAccessException($"Requester ID={requesterId} is not authorized to update Courier ID={boCourier.Id}.");
        }

        // 2. Existence Check and retrieving the existing DO object
        DO.Courier doCourier;
        try
        {
            doCourier = s_dal.Courier.Read(boCourier.Id) ?? throw new BO.BlDoesNotExistException($"Courier ID={boCourier.Id} does not exist.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If DAL throws the exception
            throw new BO.BlDoesNotExistException($"Courier ID={boCourier.Id} does not exist.", ex);
        }

        // 3. Validation and Business Rules Checks

        // a. General Validation (format and basic range checks)
        ValidateId(boCourier.Id);
        ValidateName(boCourier.FullName);
        ValidatePhone(boCourier.Phone);
        ValidateEmail(boCourier.Email);
        ValidateDistance(boCourier.MaxDeliveryDistance);
        ValidateStartDate(boCourier.StartDate);


        // b. Business Rule: 'IsActive' status can only be changed by the Administrator
        if (boCourier.Active != doCourier.Active && requesterId != s_dal.Config.ManagerId)
        {
            throw new BlUnauthorizedAccessException($"Only the administrator can change the active status of Courier ID={boCourier.Id}.");
        }

        // c. Business Rule: 'ShipmentType' can only be changed if the courier is NOT currently in delivery
        if (boCourier.DeliveryType != (BO.DeliveryType)doCourier.typeofshipment)
        {
            bool isInDelivery = s_dal.Delivery.ReadAll(d => d.CourierId == boCourier.Id && d.TimeEnd == null).Any();
            if (isInDelivery)
            {
                throw new BlInvalidRequestException($"Cannot change the shipment type for Courier ID={boCourier.Id} while they are currently handling a delivery.");
            }
        }

        // d. Business Rule: 'MaxAirDistance' cannot exceed the company's maximum
        if (boCourier.MaxDeliveryDistance > s_dal.Config.MaxRange)
        {
            throw new BlInvalidValueException($"The maximum air distance ({boCourier.MaxDeliveryDistance} km) cannot exceed the company's maximum ({s_dal.Config.MaxRange} km).");
        }


        // 4. Create the updated DO object (using 'with' if DO.Courier is a record)
        DO.Courier updatedCourier = doCourier with
        {
            Name = boCourier.FullName,
            PhoneNumber = boCourier.Phone,
            Email = boCourier.Email,
            Active = boCourier.Active,
            Distance = boCourier.MaxDeliveryDistance,
            TimeBegin = boCourier.StartDate,
            typeofshipment = (DO.TypeOfShipment)boCourier.DeliveryType, // Type casting of ENUM

            // Non-modifiable fields keep their original DO value
        };

        // 5. Call DAL for update
        s_dal.Courier.Update(updatedCourier);
        Observers.NotifyListUpdated(); //stage 5
        Observers.NotifyItemUpdated(doCourier.Id); //stage 5
    }

    /// <summary>
    /// Returns a list projection of all couriers for listing UI.
    /// </summary>
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

    /// <summary>
    /// Periodic update toggling couriers active/inactive according to inactivity rules and current deliveries.
    /// </summary>
    internal static void PeriodicCouriersUpdates()
    {
        DateTime now = AdminManager.Now;
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
            Observers.NotifyItemUpdated(doCourier.Id); //stage 5
        }
        Observers.NotifyListUpdated();  //stage 5
    }

    /// <summary>
    /// Converts a DO courier to a BO courier including computed metrics.
    /// </summary>
    internal static BO.Courier ConvertToBO(int id)
    {

        var doCourier = s_dal.Courier.Read(id);
        if (doCourier == null)
            throw new BlDoesNotExistException($"Courier with ID {id} not found.");

        return new BO.Courier
        {
            Id = doCourier.Id,
            FullName = doCourier.Name,
            Phone = doCourier.PhoneNumber,
            Email = doCourier.Email,
            Active =doCourier.Active,
            Password= doCourier.Code,
            MaxDeliveryDistance = doCourier.Distance,
            DeliveryType = (DeliveryType)doCourier.typeofshipment,
            StartDate = doCourier.TimeBegin,
            DeliveredOnTime = CalculateDeliveredOnTime(doCourier.Id),
            DeliveredLate = CalculateDeliveredLate(doCourier.Id),
            CurrentOrder = GetCurrentOrder(doCourier.Id)!
        };
    }

    /// <summary>
    /// Returns count of deliveries delivered on time by courier.
    /// </summary>
    internal static int CalculateDeliveredOnTime(int courierId)
    {
        var deliveries = s_dal.Delivery.ReadAll();
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value - d.TimeBegin <= s_dal.Config.MaxDeliveryTime)
            .Count();
    }

    /// <summary>
    /// Returns count of deliveries delivered late by courier.
    /// </summary>
    internal static int CalculateDeliveredLate(int courierId)
    {
        var deliveries = s_dal.Delivery.ReadAll();
        return deliveries
            .Where(d => d.CourierId == courierId && d.endcour == Endofcourier.Delivered && d.TimeEnd.HasValue && d.TimeEnd.Value - d.TimeBegin > s_dal.Config.MaxDeliveryTime)
            .Count();
    }

    /// <summary>
    /// Builds and returns the current order in progress for the specified courier, or null if none.
    /// </summary>
    internal static BO.OrderInProgress? GetCurrentOrder(int courierId)
    {
        DateTime currentClock =AdminManager.Now;
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
            ActualDistance = CalculateActualDistance(order.Latitude, order.Longitude,(BO.DeliveryType)delivery.typeofshipment),
            CustomerName = order.CustomerName ?? "",
            CustomerPhone = order.CustomerPhone ?? "",
            OpenTime = delivery.TimeBegin,
            StartTime = currentClock,
            ExpectedDeliveryTime = (DateTime)CalculateExpectedDeliveryTime(delivery.typeofshipment, Airdistance),
            MaxDeliveryTime = maxDelivery,
            OrderStatus = OrderManager.CalculateOrderStatus(order.OrderId),
            ScheduleStatus = OrderManager.CalculateScheduleStatus(order.OrderId),
            TimeRemaining = maxDelivery - currentClock
        };
    }

    /// <summary>
    /// Calculates expected delivery time using configured average speeds.
    /// </summary>
    internal static DateTime? CalculateExpectedDeliveryTime(TypeOfShipment t, double distanceKm)
    {
        if(t== null)
            return null;    
        double speedKmh = t switch
        {
           TypeOfShipment.Car => s_dal.Config.AvgCarSpeed,
           TypeOfShipment.Bike => s_dal.Config.AvgBicycleSpeed,
           TypeOfShipment.OnFoot => s_dal.Config.AvgWalkingSpeed,
           TypeOfShipment.Scooter => s_dal.Config.AvgMotorBikeSpeed,
            _ => s_dal.Config.AvgCarSpeed
        };

        double hours = distanceKm / speedKmh;
        return AdminManager.Now.AddHours(hours);
    }

    /// <summary>
    /// Calculates maximum allowed delivery datetime from open time.
    /// </summary>
    internal static DateTime CalculateMaxDeliveryTime(DateTime Open)
    {
        return Open.Add(s_dal.Config.MaxDeliveryTime);
    }

    /// <summary>
    /// Calculates great-circle (air) distance from company to destination in kilometers.
    /// </summary>
    internal static double CalculateAirDistance(double destLat, double destLon)
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

    /// <summary>
    /// Calculates an estimated actual driving/walking distance multiplier by delivery type.
    /// </summary>
    internal static double CalculateActualDistance(double destLat, double destLon, DeliveryType type)
    {
        if (!s_dal.Config.Latitude.HasValue || !s_dal.Config.Longitude.HasValue)
            throw new InvalidOperationException("Company coordinates are not set in Config.");

        double baseDistance = CalculateAirDistance(destLat, destLon);
        return type switch
        {
            DeliveryType.Car => baseDistance * 1.5,       
            DeliveryType.Scooter => baseDistance * 1.5, 
            DeliveryType.Bike => baseDistance * 2.0, 
            DeliveryType.OnFoot => baseDistance * 2.0,   
            _ => baseDistance
        };
    }

    /// <summary>
    /// Calculates estimated travel time based on configured speeds.
    /// </summary>
    internal static TimeSpan CalculateEstimatedActualTime(TypeOfShipment t, double distanceKm)
    {
        double speedKmh = t switch
        {
            TypeOfShipment.Car => s_dal.Config.AvgCarSpeed,
            TypeOfShipment.Bike => s_dal.Config.AvgBicycleSpeed,
            TypeOfShipment.OnFoot => s_dal.Config.AvgWalkingSpeed,
            TypeOfShipment.Scooter => s_dal.Config.AvgMotorBikeSpeed,
            _ => s_dal.Config.AvgCarSpeed
        };

        double hours = distanceKm / speedKmh;
        return TimeSpan.FromHours(hours);
    }

    /// <summary>
    /// Authenticates a user as Admin or Courier.
    /// </summary>
    /// <returns>"Admin" or "Courier"; throws on failure.</returns>
    internal static string Login(int iduser, string password)
    {
        if (iduser == s_dal.Config.ManagerId && password == s_dal.Config.ManagerPassword)
            return "Admin";

        var courier = s_dal.Courier.Read(c => c.Id == iduser && c.Code == password);
        if (courier != null)
            return "Courier";

        throw new UnauthorizedAccessException("Username or password is incorrect.");
    }
}








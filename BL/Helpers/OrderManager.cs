using System;
using System.Net;
using System.Text.Json;
using System.Linq;
using BO;
using DalApi;
using DO;

namespace Helpers;

/// <summary>
/// Business-layer helper for order workflows: validation, DO↔BO mapping and orchestration of DAL calls.
/// </summary>
/// <remarks>
/// - Performs creation, update, cancellation and completion of orders and deliveries.
/// - Uses <see cref="s_dal"/> for persistence and <see cref="CourierManager"/> for shared validation logic.
/// - Note: <see cref="ValidateAddress(string)"/> is synchronous and calls an external geocoding service; consider making it async in production.
/// </remarks>
internal static class OrderManager
{
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Validates the BO order, maps to DO and persists a new order.
    /// </summary>
    /// <param name="boOrder">Business-layer order to add.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="boOrder"/> is null.</exception>
    /// <exception cref="BlAlreadyExistsException">When DAL reports duplicate id.</exception>
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
            OrderId = 0,
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

    /// <summary>
    /// Updates an existing order after permission, validation and status checks.
    /// </summary>
    /// <param name="requesterId">Id of the user attempting the update (must be manager).</param>
    /// <param name="boOrder">Updated business order data.</param>
    internal static void UpdateOrder(int requesterId, BO.Order boOrder)
    {
        // 1. Permission Check: Only the Manager can update an Order
        if (requesterId != s_dal.Config.ManagerId)
        {
            throw new BlUnauthorizedAccessException($"Requester ID={requesterId} is not authorized to update Order ID={boOrder.Id}. Only the administrator is allowed.");
        }

        // 2. Existence Check and retrieving the existing DO object
        DO.Order doOrder;
        try
        {
            doOrder = s_dal.Order.Read(boOrder.Id) ?? throw new BO.BlDoesNotExistException($"Order ID={boOrder.Id} does not exist.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Order ID={boOrder.Id} does not exist.", ex);
        }

        // 3. Status Check: Order must be 'Open' to be updated

        // Check if the Order has any deliveries that resulted in a final closure
        bool isClosed = s_dal.Delivery.ReadAll(d => d.OrderId == boOrder.Id && d.TimeEnd != null
                                              && d.endcour != DO.Endofcourier.NotHere).Any();

        if (isClosed)
        {
            throw new BO.BlInvalidRequestException($"Order ID={boOrder.Id} cannot be updated because it is closed.");
        }

        // 4. Validation (Format Check) and Address Logic

        //ValidateOrderProperties(boOrder);
        CourierManager.ValidateName(boOrder.CustomerName);
        CourierManager.ValidateWeight(boOrder.Weight);
        CourierManager.ValidatePhone(boOrder.CustomerPhone);
        ValidateFullAddress(boOrder.FullAddress);//
        ValidateAddress(boOrder.FullAddress);


        // 5. Create the updated DO object
        DO.Order updatedOrder = doOrder with
        {
            typeOfOrder = (DO.TypeOfOrder)boOrder.Type,
            VerbalDescription = boOrder.Description,
            ShippingAddress = boOrder.FullAddress,
            Latitude = boOrder.Latitude,
            Longitude = boOrder.Longitude,
            CustomerName = boOrder.CustomerName,
            CustomerPhone = boOrder.CustomerPhone,
            // Map other BO fields to DO fields
        };

        // 6. Call DAL for update
        s_dal.Order.Update(updatedOrder);
    }

    private static readonly HttpClient client = new HttpClient();

    /// <summary>
    /// Geocodes an address using Nominatim and returns the first result coordinates.
    /// </summary>
    /// <param name="address">Address string to geocode.</param>
    /// <returns>Latitude and Longitude tuple.</returns>
    /// <remarks>
    /// - Synchronous call that blocks the calling thread. Consider converting to async/await and adding User-Agent and retries.
    /// - Throws <see cref="BlInvalidAddressException"/> when no results are returned.
    /// </remarks>
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

    /// <summary>
    /// Validates address format (street, number, city, country) and country suffix 'ישראל'.
    /// </summary>
    /// <param name="address">Full address string.</param>
    internal static void ValidateFullAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new BlNullPropertyException("Address cannot be empty.");

        var parts = address.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
            throw new BlInvalidValueException("Address must include street, number, city and country.");

        if (!parts.Last().Equals("Israel", StringComparison.OrdinalIgnoreCase))
            throw new BlInvalidValueException("Address must end with 'Israel'.");
    }

    /// <summary>
    /// Cancels an order according to its current state (Open or InProgress).
    /// </summary>
    /// <param name="orderId">Order identifier to cancel.</param>
    internal static void CancelOrderDo(int orderId)
    {

        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new BlDoesNotExistException($"Order with ID {orderId} not found.");


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

    /// <summary>
    /// Completes a delivery: marks it delivered and sets TimeEnd.
    /// </summary>
    internal static void CompleteOrderDeliveryDo(int courierId, int deliveryId)
    {
        //Read existing DO.Delivery object
        DO.Delivery doDelivery;
        try
        {
            doDelivery = s_dal.Delivery.Read(deliveryId)
                         ?? throw new BO.BlDoesNotExistException($"Delivery ID={deliveryId} does not exist.");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Delivery ID={deliveryId} does not exist.", ex);
        }



        // Ensure the delivery belongs to the requesting courier.
        if (doDelivery.CourierId != courierId)
        {
            throw new BO.BlInvalidRequestException($"Delivery ID={deliveryId} is not assigned to Courier ID={courierId}.");
        }

        //Ensure the delivery is not already completed (DeliveryEndTime must be null).
        if (doDelivery.endcour.HasValue)
        {
            throw new BO.BlInvalidRequestException($"Delivery ID={deliveryId} is already completed.");
        }

        // Update DO.Delivery fields

        // Get the current system time
        DateTime finishTime = AdminManager.Now;

        DO.Delivery updatedDelivery = doDelivery with
        {
            endcour = DO.Endofcourier.Delivered, // Set the completion type to "Supplied"
            TimeEnd = finishTime          // Update the completion time using system clock
        };


        s_dal.Delivery.Update(updatedDelivery);

    }

    /// <summary>
    /// Returns an array summary counts for order statuses and schedule statuses.
    /// </summary>
    internal static int[] GetordersSummary()
    {

        var allOrders = s_dal.Order.ReadAll(o => true);

        int orderStatusCount = Enum.GetValues(typeof(BO.OrderStatus)).Length;
        int scheduleStatusCount = Enum.GetValues(typeof(BO.ScheduleStatus)).Length;

        int[] summaryArray = new int[orderStatusCount + scheduleStatusCount];

        var orderStatusGroups = allOrders
          .GroupBy(o => CalculateOrderStatus(o.OrderId))
          .Select(g => new { Status = g.Key, Count = g.Count() });

        foreach (var g in orderStatusGroups)
        {
            summaryArray[(int)g.Status] = g.Count;
        }


        var scheduleStatusGroups = allOrders
            .GroupBy(o => CalculateScheduleStatus(o.OrderId))
            .Select(g => new { Schedule = g.Key, Count = g.Count() });

        foreach (var g in scheduleStatusGroups)
        {
            summaryArray[orderStatusCount + (int)g.Schedule] = g.Count;
        }

        return summaryArray;

    }

    /// <summary>
    /// Calculates the logical status of an order (Open, InProgress, Delivered, Refused, Cancelled).
    /// </summary>
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

    /// <summary>
    /// Calculates schedule status (OnTime, InRisk, Late) based on order initiation and config time windows.
    /// </summary>
    internal static BO.ScheduleStatus CalculateScheduleStatus(int orderId)
    {
        var order = s_dal.Order.Read(orderId);
        if (order == null)
            throw new BlDoesNotExistException($"Order with ID {orderId} not found.");

        DateTime maxDeliveryTime = order.OrderInitiation.Add(s_dal.Config.MaxDeliveryTime);
        TimeSpan timeRemaining = maxDeliveryTime - s_dal.Config.Clock;

        if (timeRemaining > s_dal.Config.RiskRange)
            return BO.ScheduleStatus.OnTime;
        else if (timeRemaining > TimeSpan.Zero)
            return BO.ScheduleStatus.InRisk;
        else
            return BO.ScheduleStatus.Late;
    }

    /// <summary>
    /// Assigns an open order to a courier and creates a delivery record.
    /// </summary>
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

    /// <summary>
    /// Returns closed deliveries for a courier, optionally filtered and sorted.
    /// </summary>
    internal static IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesForCourier(int courierId, OrderType? filterType, ClosedDeliveryInListSort? sortBy)
    {

        var listToFilter = s_dal.Delivery.ReadAll(d =>
            d.CourierId == courierId && d.TimeEnd.HasValue)
            .Select(doDelivery =>
            {

                DO.Order doOrder = s_dal.Order.Read(doDelivery.OrderId)
                                   ?? throw new BlDoesNotExistException($"Order ID={doDelivery.OrderId} for Delivery ID={doDelivery.Id} not found.");



                return new BO.ClosedDeliveryInList
                {
                    DeliveryId = doDelivery.Id,
                    OrderId = doOrder.OrderId,
                    OrderType = (BO.OrderType)doOrder.typeOfOrder,
                    DeliveryType = (BO.DeliveryType)doDelivery.typeofshipment,
                    EndType = (BO.DeliveryEndType)doDelivery.endcour!,
                    CompletionTime = doDelivery.TimeEnd!.Value - doDelivery.TimeBegin,
                    FullAddress = doOrder.ShippingAddress,
                    ActualDistance = doDelivery.Distance!.Value
                };
            }).AsEnumerable();


        if (filterType.HasValue)
        {
            listToFilter = listToFilter.Where(d => d.OrderType == filterType.Value);
        }


        if (!sortBy.HasValue)
        {
            listToFilter = listToFilter.OrderBy(d => d.CompletionTime);
        }
        else
        {
            listToFilter = sortBy.Value switch
            {
                ClosedDeliveryInListSort.DeliveryId =>
                    listToFilter.OrderBy(d => d.DeliveryId),

                ClosedDeliveryInListSort.OrderId =>
                    listToFilter.OrderBy(d => d.OrderId),

                ClosedDeliveryInListSort.ActualDistance =>
                    listToFilter.OrderBy(d => d.ActualDistance),

                ClosedDeliveryInListSort.CompletionTime =>
                    listToFilter.OrderBy(d => d.CompletionTime),

                _ => listToFilter.OrderBy(d => d.DeliveryId)
            };
        }

        return listToFilter;
    }

    /// <summary>
    /// Converts a DO order to BO order including computed metrics.
    /// </summary>
    internal static BO.Order convertToBO(int orderId)
    {
        try
        {
            var doOrder = s_dal.Order.Read(orderId);
            var Airdistance = CourierManager.CalculateAirDistance(doOrder.Latitude, doOrder.Longitude);
            var deliveries = s_dal.Delivery.ReadAll()
                            .Where(d => d.OrderId == orderId)
                            .OrderByDescending(d => d.TimeBegin)
                            .ToList();
            var lastDelivery = deliveries.First();
            var maxDeliveryTime = CourierManager.CalculateMaxDeliveryTime(doOrder.OrderInitiation);

            var boOrder = new BO.Order
            {
                Id = doOrder.OrderId,
                CustomerName = doOrder.CustomerName,
                CustomerPhone = doOrder.CustomerPhone,
                FullAddress = doOrder.ShippingAddress,
                Weight = doOrder.weight,
                Description = doOrder.VerbalDescription,
                Type = (BO.OrderType)doOrder.typeOfOrder,
                OrderStatus = CalculateOrderStatus(doOrder.OrderId),
                ScheduleStatus = CalculateScheduleStatus(doOrder.OrderId),
                OpenTime = doOrder.OrderInitiation,
                AirDistance = Airdistance,
                ExpectedDeliveryTime = CourierManager.CalculateExpectedDeliveryTime(lastDelivery.typeofshipment, Airdistance),
                MaxDeliveryTime = maxDeliveryTime,
                TimeRemaining = maxDeliveryTime - AdminManager.Now,
                Deliveries = deliveries
                .Where(d => d.endcour != Endofcourier.Delivered)
                .Select(d => new BO.DeliveryPerOrderInList
                {
                    DeliveryId = d.Id,
                    CourierId = d.CourierId,
                    CourierName = s_dal.Courier.Read(d.CourierId)?.Name ?? "Unknown Courier",
                    DeliveryType = (BO.DeliveryType)d.typeofshipment,
                    StartTime = d.TimeBegin,
                    EndType = d.endcour.HasValue ? (BO.DeliveryEndType?)d.endcour.Value : null,
                    EndTime = d.TimeEnd
                })
                .ToList(),


            };


            return boOrder;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlDoesNotExistException($"Order with ID={orderId} not found.", ex);
        }
    }

    /// <summary>
    /// Returns open orders suitable for a courier (no existing deliveries), with optional filters and sorting.
    /// </summary>
    internal static IEnumerable<OpenOrderInList> GetOpenOrdersForACourier(int courierId, OrderType? filterType, OpenOrderInListSort? sortBy)
    {
        var doCourier = s_dal.Courier.Read(courierId);
        var openOrders = s_dal.Order.ReadAll(o =>
        {
            bool hasDelivery = s_dal.Delivery.ReadAll(d => d.OrderId == o.OrderId).Any();
            return !hasDelivery;
        });


        var openOrdersList = openOrders
    .Select(o => new OpenOrderInList
    {
        CourierId = courierId,
        OrderId = o.OrderId,
        Type = (OrderType)o.typeOfOrder,
        FullAddress = o.ShippingAddress,
        Weight = o.weight,
        AirDistance = CourierManager.CalculateAirDistance(o.Latitude, o.Longitude),
        EstimatedActualTime = CourierManager.CalculateEstimatedActualTime(doCourier.typeofshipment, CourierManager.CalculateActualDistance(o.Latitude, o.Longitude, (DeliveryType)doCourier.typeofshipment)),
        TimeRemaining = CourierManager.CalculateMaxDeliveryTime(o.OrderInitiation) - DateTime.Now,
        MaxDeliveryTime = CourierManager.CalculateMaxDeliveryTime(o.OrderInitiation),
        ScheduleStatus = CalculateScheduleStatus(o.OrderId),

    })
    .AsEnumerable();

        if (filterType.HasValue)
            openOrdersList = openOrdersList.Where(o => o.Type == filterType.Value);
        if (!sortBy.HasValue)
        {
            openOrdersList = openOrdersList.OrderBy(d => d.ScheduleStatus);
        }
        else
        {
            openOrdersList = sortBy.Value switch
            {
                OpenOrderInListSort.Weight =>
                    openOrdersList.OrderBy(d => d.Weight),

                OpenOrderInListSort.OrderId =>
                    openOrdersList.OrderBy(d => d.OrderId),

                OpenOrderInListSort.ActualDistance =>
                    openOrdersList.OrderBy(d => d.ActualDistance),

                OpenOrderInListSort.AirDistance =>
                    openOrdersList.OrderBy(d => d.AirDistance),

                OpenOrderInListSort.EstimatedActualTime =>
                    openOrdersList.OrderBy(d => d.EstimatedActualTime),

                OpenOrderInListSort.TimeRemaining =>
                    openOrdersList.OrderBy(d => d.TimeRemaining),

                OpenOrderInListSort.MaxDeliveryTime =>
                    openOrdersList.OrderBy(d => d.MaxDeliveryTime),

                _ => openOrdersList.OrderBy(d => d.OrderId)
            };
        }

        return openOrdersList;
    }

    /// <summary>
    /// Returns a projection list of orders with last-delivery and scheduling metrics, supports filtering and sorting.
    /// </summary>
    internal static IEnumerable<OrderInList> GetordersList(OrderInListFilter? filterField, object? filterValue, OrderInListSort? sortBy)
    {
        var allOrders = s_dal.Order.ReadAll(o => true);
        var allDeliveries = s_dal.Delivery.ReadAll(d => true);

        var ordersWithLastDelivery = allOrders.Select(o =>
        {
            var deliveriesOfOrder = allDeliveries.Where(d => d.OrderId == o.OrderId).ToList();
            var lastDelivery = deliveriesOfOrder
                .OrderByDescending(d => d.TimeEnd ?? DateTime.MinValue)
                .FirstOrDefault();

            int totalDeliveries = deliveriesOfOrder.Count;

            TimeSpan totalCompletion = lastDelivery != null && lastDelivery.TimeEnd.HasValue
                ? lastDelivery.TimeEnd.Value - o.OrderInitiation
                : TimeSpan.Zero;

            bool isClosed = lastDelivery?.endcour.HasValue ?? false;

            TimeSpan timeRemaining = !isClosed
                ? o.OrderInitiation.Add(s_dal.Config.MaxDeliveryTime) - DateTime.Now
                : TimeSpan.Zero;

            double airDistance = CourierManager.CalculateAirDistance(o.Latitude, o.Longitude);

            return new OrderInList
            {
                DeliveryId = lastDelivery?.Id,
                OrderId = o.OrderId,
                Type = (OrderType)o.typeOfOrder,
                AirDistance = airDistance,
                OrderStatus = CalculateOrderStatus(o.OrderId),
                ScheduleStatus = CalculateScheduleStatus(o.OrderId),
                TimeRemaining = timeRemaining > TimeSpan.Zero ? timeRemaining : TimeSpan.Zero,
                TotalCompletionTime = totalCompletion > TimeSpan.Zero ? totalCompletion : TimeSpan.Zero,
                TotalDeliveries = totalDeliveries
            };
        });

     
        if (filterField.HasValue && filterValue != null)
        {
            ordersWithLastDelivery = filterField.Value switch
            {
                OrderInListFilter.Type => ordersWithLastDelivery.Where(o => o.Type.Equals((OrderType)filterValue)),
                OrderInListFilter.OrderStatus => ordersWithLastDelivery.Where(o => o.OrderStatus.Equals((OrderStatus)filterValue)),
                OrderInListFilter.ScheduleStatus => ordersWithLastDelivery.Where(o => o.ScheduleStatus.Equals((ScheduleStatus)filterValue)),
                _ => ordersWithLastDelivery
            };
        }

        
        ordersWithLastDelivery = sortBy switch
        {
            OrderInListSort.OrderId => ordersWithLastDelivery.OrderBy(o => o.OrderId),
            OrderInListSort.AirDistance => ordersWithLastDelivery.OrderBy(o => o.AirDistance),
            OrderInListSort.TimeRemaining => ordersWithLastDelivery.OrderBy(o => o.TimeRemaining),
            OrderInListSort.TotalCompletionTime => ordersWithLastDelivery.OrderBy(o => o.TotalCompletionTime),
            OrderInListSort.TotalDeliveries => ordersWithLastDelivery.OrderBy(o => o.TotalDeliveries),
            _ => ordersWithLastDelivery.OrderBy(o => o.OrderStatus)
        };

        return ordersWithLastDelivery.AsEnumerable();
    }
}






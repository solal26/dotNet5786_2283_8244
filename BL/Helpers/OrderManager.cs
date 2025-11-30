

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

    // In Helpers/OrderManager.cs



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

    internal static IEnumerable<ClosedDeliveryInList> GetClosedDeliveriesForCourier(int courierId, ClosedDeliveryInListFilter? filterType, ClosedDeliveryInListSort? sortBy)
    {

        var listToFilter = s_dal.Delivery.ReadAll(d =>
            d.CourierId == courierId && d.TimeEnd.HasValue)
            .Select(doDelivery =>
            {

                DO.Order doOrder = s_dal.Order.Read(doDelivery.OrderId)
                                   ?? throw new DalDoesNotExistException($"Order ID={doDelivery.OrderId} for Delivery ID={doDelivery.Id} not found.");



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
            listToFilter = filterType.Value switch
            {
                ClosedDeliveryInListFilter.OrderType =>
                    listToFilter.Where(d => d.OrderType == d.OrderType),

                ClosedDeliveryInListFilter.DeliveryType =>
                    listToFilter.Where(d => d.DeliveryType == d.DeliveryType),

                ClosedDeliveryInListFilter.EndType =>
                    listToFilter.Where(d => d.EndType == d.EndType),

                _ => listToFilter
            };
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
                ScheduleStatus = CourierManager.CalculateScheduleStatus(doOrder.OrderId),
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
}
    

//        // 4. Sorting Logic (using your ClosedDeliveryInListSort enum)
//        IOrderedEnumerable<BO.ClosedDeliveryInList> orderedList

//        if (sortBy == null)
//        {
//            // Default sort: Sort by ScheduleStatus (most logical for performance history)
//            orderedList = listToFilter.OrderBy(bdl => bdl.ScheduleStatus);
//        }
//        else
//        {
//            // Sort by the specific property requested
//            switch (sortBy.Value)
//            {
//                case BO.ClosedDeliveryInListSort.DeliveryId:
//                    orderedList = listToFilter.OrderBy(bdl => bdl.Id);
//                    break;
//                case BO.ClosedDeliveryInListSort.OrderId:
//                    orderedList = listToFilter.OrderBy(bdl => bdl.OrderId);
//                    break;
//                case BO.ClosedDeliveryInListSort.ActualDistance:
//                    orderedList = listToFilter.OrderByDescending(bdl => bdl.ActualDistance);
//                    break;
//                case BO.ClosedDeliveryInListSort.CompletionTime:
//                    orderedList = listToFilter.OrderBy(bdl => bdl.FinishTime);
//                    break;
//                default:
//                    // Fallback to ScheduleStatus if the enum value is not handled
//                    orderedList = listToFilter.OrderBy(bdl => bdl.ScheduleStatus);
//                    break;
//            }
//        }

//        return orderedList;
//    }
//}
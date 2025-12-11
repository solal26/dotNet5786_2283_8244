namespace BlImplementation;

using System.Collections.Generic;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

/// <summary>
/// BL façade implementing <see cref="BlApi.IOrder"/>.
/// Delegates validation and business rules to <see cref="OrderManager"/> and permission checks to <see cref="CourierManager"/>.
/// Methods perform minimal orchestration and do not mutate domain logic.
/// </summary>
internal class OrderImplementation : BlApi.IOrder
{
    /// <summary>
    /// Validate requester permission then create a new order via <see cref="OrderManager.AddOrdeDo"/>.
    /// </summary>
    public void AddOrder(int requesterId, BO.Order order)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");

        OrderManager.AddOrdeDo(order);

    }

    /// <summary>
    /// Forward update request to <see cref="OrderManager.UpdateOrder"/> (manager-only check is inside manager).
    /// </summary>
    public void UpdateOrder(int requesterId, BO.Order order)
    {
        OrderManager.UpdateOrder(requesterId, order);
    }

    /// <summary>
    /// Cancel an order after permission check; forwards to <see cref="OrderManager.CancelOrderDo"/>.
    /// </summary>
    public void CancelOrder(int requesterId, int orderId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");

        OrderManager.CancelOrderDo(orderId);

    }

    /// <summary>
    /// Assign an order to a courier after authorization of the requester.
    /// </summary>
    public void ChooseOrderForDelivery(int requesterId, int courierId, int orderId)
    {
        if (!CourierManager.HasPermission(requesterId, courierId))
            throw new BlUnauthorizedAccessException("Requester does not have permission to assign this order.");

        OrderManager.ChooseOrderForDeliveryDo(courierId, orderId);
    }

    /// <summary>
    /// Mark a delivery complete — ensures requester has permission then delegates to <see cref="OrderManager.CompleteOrderDeliveryDo"/>.
    /// </summary>
    public void CompleteOrderDelivery(int requesterId, int courierId, int deliveryId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        OrderManager.CompleteOrderDeliveryDo(courierId, deliveryId);

    }

    /// <summary>
    /// Deleting orders is not supported; this method always throws.
    /// </summary>
    public void DeleteOrder(int requesterId, int orderId)
    {
        throw new BlInvalidRequestException($"Order ID={orderId} cannot be deleted from the system. Orders can only be cancelled or completed.");

    }

    /// <summary>
    /// Return closed deliveries for a courier after permission check by requester.
    /// </summary>
    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveries(int requesterId, int courierId, OrderType? filterType=null, ClosedDeliveryInListSort? sortBy = null)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        return OrderManager.GetClosedDeliveriesForCourier(courierId, filterType, sortBy);
    }

    /// <summary>
    /// Return open orders suitable for a courier after permission check.
    /// </summary>
    public IEnumerable<OpenOrderInList> GetOpenOrdersForCourier(
    int requesterId,
    int courierId,
    OrderType? filterType = null,
    OpenOrderInListSort? sortBy = null)
    {
        
        bool permission = CourierManager.HasPermission(requesterId, courierId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to view open orders.");

       return OrderManager.GetOpenOrdersForACourier(courierId, filterType, sortBy);
        
    }

    /// <summary>
    /// Return a full BO order projection after permission check.
    /// </summary>
    public BO.Order GetOrder(int requesterId, int orderId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        return OrderManager.convertToBO(orderId);
    }

    /// <summary>
    /// Return orders list projection (filter/sort supported) after permission check.
    /// </summary>
    public IEnumerable<OrderInList> GetOrdersList(int requesterId, OrderInListFilter? filterField = null, object? filterValue = null, OrderInListSort? sortBy = null)
    {
        
        if (!CourierManager.HasPermission(requesterId))
            throw new BlUnauthorizedAccessException("Requester does not have permission to view orders.");

       return OrderManager.GetordersList(filterField, filterValue, sortBy);
    }

    /// <summary>
    /// Return aggregated orders summary after permission check.
    /// </summary>
    public int[] GetOrdersSummary(int requesterId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        return OrderManager.GetordersSummary();
    }
    public void AddObserver(Action listObserver) =>
   OrderManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
OrderManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
OrderManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
OrderManager.Observers.RemoveObserver(id, observer); //stage 5

}







namespace BlImplementation;

using System.Collections.Generic;
using BlApi;
using BO;
using DalApi;
using Helpers;

internal class OrderImplementation : IOrder
{
    public void AddOrder(int requesterId, BO.Order order)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");

        OrderManager.AddOrdeDo(order);

    }

    public void UpdateOrder(int requesterId, BO.Order order)
    {
        OrderManager.UpdateOrder(requesterId, order);
    }



    public void CancelOrder(int requesterId, int orderId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");

        OrderManager.CancelOrderDo(orderId);

    }
    public void ChooseOrderForDelivery(int requesterId, int courierId, int orderId)
    {
        if (!CourierManager.HasPermission(requesterId, courierId))
            throw new BlUnauthorizedAccessException("Requester does not have permission to assign this order.");

        OrderManager.ChooseOrderForDeliveryDo(courierId, orderId);
    }

    public void CompleteOrderDelivery(int requesterId, int courierId, int deliveryId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        OrderManager.CompleteOrderDeliveryDo(courierId, deliveryId);

    }

    public void DeleteOrder(int requesterId, int orderId)
    {
        throw new BlInvalidRequestException($"Order ID={orderId} cannot be deleted from the system. Orders can only be cancelled or completed.");

    }

    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveries(int requesterId, int courierId, ClosedDeliveryInListFilter? filterType = null, ClosedDeliveryInListSort? sortBy = null)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        return OrderManager.GetClosedDeliveriesForCourier(courierId, filterType, sortBy);
    }

    public IEnumerable<OpenOrderInList> GetOpenOrdersForCourier(int requesterId, int courierId, OpenOrderInListFilter? filterType = null, OpenOrderInListSort? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public Order GetOrder(int requesterId, int orderId)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add this order.");
        return OrderManager.convertToBO(orderId);
    }

    public IEnumerable<OrderInList> GetOrdersList(int requesterId, OrderInListFilter? filterField = null, object? filterValue = null, OrderInListSort? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public int[] GetOrdersSummary(int requesterId)
    {
        throw new NotImplementedException();
    }
}

  



   
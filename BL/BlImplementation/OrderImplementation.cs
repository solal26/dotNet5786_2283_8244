
namespace BlImplementation;

using System.Collections.Generic;
using BlApi;
using BO;   

internal class OrderImplementation : IOrder
{
    public void AddOrder(int requesterId, Order order)
    {
        throw new NotImplementedException();
    }

    public void CancelOrder(int requesterId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void ChooseOrderForDelivery(int requesterId, int courierId, int orderId)
    {
        throw new NotImplementedException();
    }

    public void CompleteOrderDelivery(int requesterId, int courierId, int deliveryId)
    {
        throw new NotImplementedException();
    }

    public void DeleteOrder(int requesterId, int orderId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ClosedDeliveryInList> GetClosedDeliveries(int requesterId, int courierId, ClosedDeliveryInListFilter? filterType = null, ClosedDeliveryInListSort? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OpenOrderInList> GetOpenOrdersForCourier(int requesterId, int courierId, OpenOrderInListFilter? filterType = null, OpenOrderInListSort? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public Order GetOrder(int requesterId, int orderId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OrderInList> GetOrdersList(int requesterId, OrderInListFilter? filterField = null, object? filterValue = null, OrderInListSort? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public int[] GetOrdersSummary(int requesterId)
    {
        throw new NotImplementedException();
    }

    public void UpdateOrder(int requesterId, Order order)
    {
        throw new NotImplementedException();
    }
}

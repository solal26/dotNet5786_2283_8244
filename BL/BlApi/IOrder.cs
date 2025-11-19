

using BO;

namespace BlApi;

public interface IOrder
{
    int[] GetOrdersSummary(int requesterId);

    IEnumerable<BO.OrderInList> GetOrdersList(
             int requesterId,
             OrderInListFilter? filterField = null,
             object? filterValue = null,
             OrderInListSort? sortBy = null);


    BO.Order GetOrder(int requesterId, int orderId);

    void UpdateOrder(int requesterId, BO.Order order);

    void CancelOrder(int requesterId, int orderId);

    void DeleteOrder(int requesterId, int orderId);

    void AddOrder(int requesterId, BO.Order order);

    void CompleteOrderDelivery(int requesterId, int courierId, int deliveryId);

    void ChooseOrderForDelivery(int requesterId, int courierId, int orderId);

    IEnumerable<BO.ClosedDeliveryInList> GetClosedDeliveries(
        int requesterId,
        int courierId,
        ClosedDeliveryInListFilter? filterType = null,
        ClosedDeliveryInListSort? sortBy = null);

    IEnumerable<BO.OpenOrderInList> GetOpenOrdersForCourier(
        int requesterId,
        int courierId,
        OpenOrderInListFilter? filterType = null,
        OpenOrderInListSort? sortBy = null);
}
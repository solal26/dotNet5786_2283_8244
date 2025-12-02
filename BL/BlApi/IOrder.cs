using BO;
using System.Collections.Generic;

namespace BlApi;

/// <summary>
/// Business-layer service surface for order-related operations.
/// Implementations perform validation, authorization (via the provided <paramref name="requesterId"/>),
/// mapping between BO/DO and orchestration of order/delivery workflows.
/// </summary>
/// <remarks>
/// - Most methods expect the caller to provide the id of the requester (<c>requesterId</c>) so the implementation can enforce permissions:
///   - manager-level operations require the configured manager id,
///   - courier-level operations permit the courier themselves when appropriate.
/// - Implementations should document thrown BL exceptions (for example: <c>BlDoesNotExistException</c>, <c>BlInvalidRequestException</c>,
///   <c>BlAlreadyExistsException</c>, <c>BlUnauthorizedAccessException</c>, <c>BlInvalidAddressException</c>).
/// </remarks>
public interface IOrder:IObservable<Order>
{
    /// <summary>
    /// Returns aggregated counts for orders and schedule-status buckets.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting the summary (used for permission checks).</param>
    /// <returns>Array of integers where indexes correspond to order/status enums (documented by the implementation).</returns>
    int[] GetOrdersSummary(int requesterId);

    /// <summary>
    /// Returns a list projection of orders for display, with optional filtering and sorting.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting the list (used for permission checks).</param>
    /// <param name="filterField">Optional filter field (type, order status or schedule status).</param>
    /// <param name="filterValue">Value for the selected filter field; interpretation is implementation-specific.</param>
    /// <param name="sortBy">Optional sort key for the returned list.</param>
    /// <returns>Enumerable of <see cref="BO.OrderInList"/> matching the filter and sort.</returns>
    IEnumerable<BO.OrderInList> GetOrdersList(
             int requesterId,
             OrderInListFilter? filterField = null,
             object? filterValue = null,
             OrderInListSort? sortBy = null);

    /// <summary>
    /// Returns a full business-layer order projection including deliveries and computed metrics.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting the order details.</param>
    /// <param name="orderId">Identifier of the requested order.</param>
    /// <returns><see cref="BO.Order"/> containing order details and computed fields.</returns>
    /// <exception cref="BlDoesNotExistException">When the specified order does not exist.</exception>
    BO.Order GetOrder(int requesterId, int orderId);

    /// <summary>
    /// Updates an existing order. Typically restricted to administrative users (implementation may enforce this).
    /// </summary>
    /// <param name="requesterId">Id of the user requesting the update (authorization will be enforced).</param>
    /// <param name="order">Business order data to apply (Id must match the target order).</param>
    void UpdateOrder(int requesterId, BO.Order order);

    /// <summary>
    /// Cancel an order. Behavior depends on order state (Open -> creates cancelled delivery; InProgress -> update active delivery).
    /// </summary>
    /// <param name="requesterId">Id of the user requesting cancellation.</param>
    /// <param name="orderId">Identifier of the order to cancel.</param>
    void CancelOrder(int requesterId, int orderId);

    /// <summary>
    /// Delete an order from the system. Implementations may disallow deletion and throw a business exception instead.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting deletion.</param>
    /// <param name="orderId">Identifier of the order to delete.</param>
    void DeleteOrder(int requesterId, int orderId);

    /// <summary>
    /// Create a new order after validation and optional geocoding.
    /// </summary>
    /// <param name="requesterId">Id of the user creating the order (used for permission/audit).</param>
    /// <param name="order">Business order payload; Id is usually ignored/assigned by the DAL.</param>
    void AddOrder(int requesterId, BO.Order order);

    /// <summary>
    /// Mark a delivery as completed by the courier.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting completion (usually the courier).</param>
    /// <param name="courierId">Courier identifier performing the completion; must match active assignment.</param>
    /// <param name="deliveryId">Delivery identifier to complete.</param>
    void CompleteOrderDelivery(int requesterId, int courierId, int deliveryId);

    /// <summary>
    /// Assigns (chooses) an open order for delivery by a courier and creates the delivery record.
    /// </summary>
    /// <param name="requesterId">Id of the caller (must be authorized to assign, typically the courier or manager).</param>
    /// <param name="courierId">Identifier of the courier that will perform the delivery.</param>
    /// <param name="orderId">Identifier of the order to assign.</param>
    void ChooseOrderForDelivery(int requesterId, int courierId, int orderId);

    /// <summary>
    /// Returns closed deliveries for a given courier (completed attempts), with optional filtering and sorting.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting the data (permission enforced).</param>
    /// <param name="courierId">Courier for which to return closed deliveries.</param>
    /// <param name="filterType">Optional order type filter.</param>
    /// <param name="sortBy">Optional sort key for the returned results.</param>
    /// <returns>Enumerable of <see cref="BO.ClosedDeliveryInList"/>.</returns>
    IEnumerable<BO.ClosedDeliveryInList> GetClosedDeliveries(
        int requesterId,
        int courierId,
        OrderType? filterType = null,
        ClosedDeliveryInListSort? sortBy = null);

    /// <summary>
    /// Returns open orders suitable for a courier (orders without an active delivery), with optional filtering and sorting.
    /// </summary>
    /// <param name="requesterId">Id of the user requesting the list.</param>
    /// <param name="courierId">Courier for which to compute suitability (used to estimate actual distance/time).</param>
    /// <param name="filterType">Optional order type filter.</param>
    /// <param name="sortBy">Optional sort key.</param>
    /// <returns>Enumerable of <see cref="BO.OpenOrderInList"/> available to the courier.</returns>
    IEnumerable<BO.OpenOrderInList> GetOpenOrdersForCourier(
        int requesterId,
        int courierId,
        OrderType? filterType = null,
        OpenOrderInListSort? sortBy = null);
}
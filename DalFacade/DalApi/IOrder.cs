namespace DalApi;
using DO;

/// <summary>
/// Data-access surface for order entities (CRUD).
/// Implementations manage persistence and retrieval of <see cref="Order"/> objects.
/// </summary>
/// <remarks>
/// - Implementations must document id-generation semantics (whether client-supplied <see cref="Order.OrderId"/> values are accepted or ignored).
/// - Returned collections are expected to be snapshots; callers should not assume they are live views into the store.
/// - Date/time values on orders should be expressed consistently (prefer UTC) and documented by the implementation.
/// </remarks>
    public interface IOrder : ICrud<Order>
{

}

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
    public interface IOrder
{
    void Create(Order item); //Creates new entity object in DAL
    Order? Read(int id); //Reads entity object by its ID 
    List<Order> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Order item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects

}

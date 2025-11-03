namespace DalApi;
using DO;

/// <summary>
/// Data access interface for courier entities (CRUD operations).
/// Implementations manage persistence and retrieval of <see cref="Courier"/> objects.
/// </summary>
public interface ICourier:ICrud <Courier>
{
}

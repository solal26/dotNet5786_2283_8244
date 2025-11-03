namespace DalApi;
using DO;

/// <summary>
/// Data access interface for courier entities (CRUD operations).
/// Implementations manage persistence and retrieval of <see cref="Courier"/> objects.
/// </summary>
public interface ICourier
{
    /// <summary>
    /// Creates a new courier entity in the data store.
    /// Implementations should throw if an entity with the same Id already exists.
    /// </summary>
    /// <param name="item">Courier to create.</param>
    void Create(Courier item); //Creates new entity object in DAL

    /// <summary>
    /// Reads a courier entity by its identifier.
    /// Returns <c>null</c> if not found.
    /// </summary>
    /// <param name="id">Courier identifier.</param>
    /// <returns>The courier or <c>null</c>.</returns>
    Courier? Read(int id); //Reads entity object by its ID 

    /// <summary>
    /// Reads all courier entities.
    /// Stage 1: returns a snapshot list of all couriers.
    /// </summary>
    /// <returns>List of all couriers.</returns>
    List<Courier> ReadAll(); //stage 1 only, Reads all entity objects

    /// <summary>
    /// Updates an existing courier entity.
    /// Implementations should throw if the entity does not exist.
    /// </summary>
    /// <param name="item">Courier with updated values (Id must match existing entity).</param>
    void Update(Courier item); //Updates entity object

    /// <summary>
    /// Deletes a courier by id. Implementations may be no-op if id not found.
    /// </summary>
    /// <param name="id">Identifier of the courier to delete.</param>
    void Delete(int id); //Deletes an object by its Id

    /// <summary>
    /// Deletes all courier entities from the store.
    /// Use with caution; typically used in tests or reset scenarios.
    /// </summary>
    void DeleteAll(); //Delete all entity objects
}

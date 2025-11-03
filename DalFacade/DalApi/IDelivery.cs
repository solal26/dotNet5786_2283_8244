namespace DalApi;
using DO;

/// <summary>
/// Data-access surface for delivery entities (CRUD).
/// Implementations manage persistence and retrieval of <see cref="Delivery"/> objects.
/// </summary>
public interface IDelivery
{
    /// <summary>
    /// Creates a new delivery record.
    /// Implementations may assign a new <see cref="Delivery.Id"/> and should document whether client-supplied Ids are accepted.
    /// </summary>
    /// <param name="item">Delivery to create.</param>
    void Create(Delivery item); //Creates new entity object in DAL

    /// <summary>
    /// Reads a delivery by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the delivery to read.</param>
    /// <returns>The matching <see cref="Delivery"/> or <c>null</c> if not found.</returns>
    Delivery? Read(int id); //Reads entity object by its ID 

    /// <summary>
    /// Reads all delivery records.
    /// </summary>
    /// <returns>A snapshot list of all deliveries. Modifying the returned list should not affect the underlying store.</returns>
    List<Delivery> ReadAll(); //stage 1 only, Reads all entity objects

    /// <summary>
    /// Updates an existing delivery record.
    /// </summary>
    /// <param name="item">Delivery with updated values. The <see cref="Delivery.Id"/> identifies the record to update.</param>
    /// <remarks>Implementations should throw if the specified Id does not exist.</remarks>
    void Update(Delivery item); //Updates entity object

    /// <summary>
    /// Deletes a delivery by id.
    /// </summary>
    /// <param name="id">Identifier of the delivery to delete.</param>
    /// <remarks>Implementations may either be a no-op when the id is not found or throw; behaviour must be documented by the implementation.</remarks>
    void Delete(int id); //Deletes an object by its Id

    /// <summary>
    /// Deletes all delivery records from the store.
    /// </summary>
    /// <remarks>Typically used for tests or reset operations. Implementations should document whether this affects persisted storage.</remarks>
    void DeleteAll(); //Delete all entity objects

}

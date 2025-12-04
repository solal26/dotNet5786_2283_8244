using System.Collections.Generic;
using BO;

namespace BlApi;

/// <summary>
/// Business-layer service surface for courier operations.
/// Implementations enforce validation, authorization (via <paramref name="requesterId"/>), and map between BO/DO.
/// </summary>
/// <remarks>
/// - Methods accept a <c>requesterId</c> to allow the implementation to enforce permissions (manager vs. courier).
/// - Implementations should document which BL exceptions they throw (for example: <see cref="BlDoesNotExistException"/>, 
///   <see cref="BlAlreadyExistsException"/>, <see cref="BlUnauthorizedAccessException"/>, <see cref="BlInvalidValueException"/>).
/// </remarks>
public interface ICourier: IObservable
{
    /// <summary>
    /// Authenticate a user and return role string ("Admin" or "Courier").
    /// </summary>
    /// <param name="iduser">User identifier (manager or courier id).</param>
    /// <param name="password">Credential to validate.</param>
    /// <returns>"Admin" when credentials match the manager, "Courier" when matching a courier account.</returns>
    /// <exception cref="BlUnauthorizedAccessException">Thrown when authentication fails.</exception>
    string Login(int iduser, string password);

    /// <summary>
    /// Returns a projected list of couriers for UI lists with optional filtering and sorting.
    /// </summary>
    /// <param name="requesterId">Id of the caller (used for authorization/audit).</param>
    /// <param name="isActive">Optional filter by active status; null returns all.</param>
    /// <param name="sortBy">Optional sort key for the returned sequence.</param>
    /// <returns>Enumerable of <see cref="CourierInList"/> suitable for listing and sorting.</returns>
    IEnumerable<BO.CourierInList> GetList(int requesterId,
                                          bool? isActive = null,
                                          BO.CourierSortBy? sortBy = null);

    /// <summary>
    /// Returns a detailed business-layer courier record.
    /// </summary>
    /// <param name="requesterId">Id of the caller (manager or the courier themself).</param>
    /// <param name="courierId">Identifier of the courier to retrieve.</param>
    /// <returns>Business-layer <see cref="BO.Courier"/> with computed metrics.</returns>
    /// <exception cref="BlDoesNotExistException">Thrown when the courier does not exist.</exception>
    /// <exception cref="BlUnauthorizedAccessException">Thrown when the requester is not authorized to view this courier.</exception>
    BO.Courier GetCourier(int requesterId, int courierId);

    /// <summary>
    /// Update a courier's mutable fields (contact, availability, delivery preferences).
    /// </summary>
    /// <param name="requesterId">Id of the caller (manager or the courier themself).</param>
    /// <param name="courier">Business-layer courier instance with updated values (Id identifies target).</param>
    /// <remarks>
    /// - Implementations should enforce validation and business rules (for example: only manager may change <c>Active</c>).
    /// </remarks>
    void UpdateCourier(int requesterId, BO.Courier courier);

    /// <summary>
    /// Delete a courier after performing necessary permission and business-rule checks.
    /// </summary>
    /// <param name="requesterId">Id of the caller (must be authorized).</param>
    /// <param name="courierId">Identifier of the courier to delete.</param>
    /// <exception cref="BlInvalidRequestException">Thrown when deletion violates business rules (e.g., delivery history).</exception>
    /// <exception cref="BlDoesNotExistException">Thrown when the courier does not exist.</exception>
    void DeleteCourier(int requesterId, int courierId);

    /// <summary>
    /// Create a new courier after validation and mapping to DO.
    /// </summary>
    /// <param name="requesterId">Id of the caller (typically the manager).</param>
    /// <param name="courier">Business-layer courier to add (Id must be validated/unique).</param>
    /// <exception cref="BlAlreadyExistsException">Thrown when a courier with the same Id already exists.</exception>
    void AddCourier(int requesterId, BO.Courier courier);

    IEnumerable<BO.CourierInList> GetListOfCourier(int requesterId = 0, DeliveryType? filter = null);
}

namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Business-layer implementation exposing courier operations from <see cref="BlApi.ICourier"/>.
/// Delegates validation and business rules to <see cref="CourierManager"/> and maps between BO/DO where needed.
/// </summary>
internal class CourierImplementation : BlApi.ICourier
{
    /// <summary>
    /// Adds a new courier after permission check (manager only).
    /// Throws on validation or DAL-level errors propagated from <see cref="CourierManager.AddCourierDo"/>.
    /// </summary>
    public void AddCourier(int requesterId, BO.Courier courier)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add a courier.");

        CourierManager.AddCourierDo(courier);
    }

    /// <summary>
    /// Deletes a courier after permission and business-rule checks performed inside <see cref="CourierManager.DeleteCourier"/>.
    /// </summary>
    public void DeleteCourier(int requesterId, int courierId)
    {
        CourierManager.DeleteCourier(requesterId, courierId);
    }

    /// <summary>
    /// Returns a courier's full BO record if the requester is authorized (manager or the courier themself).
    /// </summary>
    public BO.Courier GetCourier(int requesterId, int courierId)
    {
        bool permission = CourierManager.HasPermission(requesterId, courierId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to access this courier's information.");

        var boCourier = CourierManager.ConvertToBO(courierId);
        return boCourier;
    }

    /// <summary>
    /// Returns a filtered and sorted list projection suitable for UI lists.
    /// </summary>
    public IEnumerable<BO.CourierInList> GetList(int requesterId, bool? isActive = null, BO.CourierSortBy? sortBy = null)
    {
        var list = CourierManager.GetAllCouriersInList();

        if (isActive.HasValue)
            list = list.Where(c => c.Active == isActive.Value);

        list = sortBy switch
        {
            CourierSortBy.FullName => list.OrderBy(c => c.FullName),
            CourierSortBy.StartDate => list.OrderBy(c => c.StartDate),
            CourierSortBy.DeliveredOnTime => list.OrderByDescending(c => c.DeliveredOnTime),
            CourierSortBy.DeliveredLate => list.OrderByDescending(c => c.DeliveredLate),
            CourierSortBy.Active => list.OrderByDescending(c => c.Active),
            CourierSortBy.DeliveryType => list.OrderBy(c => c.DeliveryType),
            _ => list.OrderBy(c => c.Id),
        };

        return list;
    }

    /// <summary>
    /// Authenticates a user. Returns "Admin" or "Courier" on success; throws on failure.
    /// </summary>
    public string Login(int iduser, string password)
    {
        return CourierManager.Login(iduser, password);
    }

    /// <summary>
    /// Updates a courier after performing validation and business-rule checks in <see cref="CourierManager.UpdateCourier"/>.
    /// </summary>
    public void UpdateCourier(int requesterId, BO.Courier boCourier)
    {
        CourierManager.UpdateCourier(requesterId, boCourier);
    }
    public IEnumerable<BO.CourierInList> GetListOfCourier(int requesterId, BO.DeliveryType filter )
    {
        var list = CourierManager.GetAllCouriersInList();
        if (filter.Equals(BO.DeliveryType.All) == false)
        {
            list = list.Where(c => c.DeliveryType == filter);
        }
        return list;
    }


    public void AddObserver(Action listObserver) =>
    CourierManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
CourierManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
CourierManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
CourierManager.Observers.RemoveObserver(id, observer); //stage 5

}

namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using Helpers;

internal class CourierImplementation : BlApi.ICourier
{
    public void AddCourier(int requesterId, BO.Courier courier)
    {
        bool permission = CourierManager.HasPermission(requesterId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to add a courier.");

        CourierManager.AddCourierDo(courier);

    }
    public void DeleteCourier(int requesterId, int courierId)
    {
        CourierManager.DeleteCourier(requesterId, courierId);
    }

    public BO.Courier GetCourier(int requesterId, int courierId)
    {
        
        bool permission = CourierManager.HasPermission(requesterId, courierId);
        if (!permission)
            throw new BlUnauthorizedAccessException("Requester does not have permission to access this courier's information.");
       
        var boCourier = CourierManager.ConvertToBO(courierId);

        return boCourier;
    }

    public IEnumerable<BO.CourierInList> GetList(string requesterId, bool? isActive = null, BO.CourierSortBy? sortBy = null)
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

    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }
}

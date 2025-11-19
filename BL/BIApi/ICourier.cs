namespace BlApi;

public interface ICourier
{
    string Login(string username, string password);

    IEnumerable<BO.CourierInList> GetList(string requesterId,
                                          bool? isActive = null,
                                          BO.CourierSortBy? sortBy = null);

    BO.Courier GetCourier(int requesterId, int courierId);

    void UpdateCourier(int requesterId, BO.Courier courier);

    void DeleteCourier(int requesterId, int courierId);

    void AddCourier(int requesterId, BO.Courier courier);
}

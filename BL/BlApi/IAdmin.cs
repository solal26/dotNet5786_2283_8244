

using BO;

namespace BlApi;

public interface IAdmin
{

    void ResetDB();
    void InitializeDB();
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
    Task<Config> GetConfig();
    void SetConfig(BO.Config config);


    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5


}


namespace BlImplementation;
using BlApi;
using BO;
using Helpers;

internal class AdminImplementation : IAdmin
{
    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
    }

    public DateTime GetClock()=> DateTime.Now;
   
    public Config GetConfig() => AdminManager.GetConfig();
  
    public void InitializeDB()
    {
        AdminManager.InitializeDB();    
    }

    public void ResetDB()
    {
        AdminManager.ResetDB();
    }

    public void SetConfig(Config config)=>
        AdminManager.SetConfig(config);

}

using BO;
using PL.Courier;
using PL.Helpers;
using PL.Order;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public int  Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow));

        public bool IsRunning
        {
            get { return (bool)GetValue(IsRunningProperty); }
            set { SetValue(IsRunningProperty, value); }
        }
        public static readonly DependencyProperty IsRunningProperty =
            DependencyProperty.Register("IsRunning", typeof(bool), typeof(MainWindow));


        public Config Configuration
        {
            get { return (Config)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        public static readonly DependencyProperty ConfigurationProperty =
    DependencyProperty.Register(
        "Configuration",
        typeof(Config),
        typeof(MainWindow)
);


        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        private readonly ObserverMutex _clockMutex = new(); //stage 7
        private void clockObserver()
        {
            #region Stage 7 (for multithreading)
            if (_clockMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {

                CurrentTime = s_bl.Admin.GetClock();

                // Check if a restart was requested while we were working
                if (await _clockMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    clockObserver();
            });
            #endregion Stage 7 (for multithreading)

        }


        private readonly ObserverMutex configMutex = new(); //stage 7
        private async void configObserver()
        {
            if (configMutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;    
            await Dispatcher.BeginInvoke(async () =>
            {
                Configuration = await s_bl.Admin.GetConfig();
                // Check if a restart was requested while we were working
                if (await configMutex.UnsetLoadInProgressAndCheckRestartRequested())
                    configObserver();
            });
                

        }

        private async void MainWindow_Loaded(object Sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = await s_bl.Admin.GetConfig();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Order.AddObserver(LoadOrderSummary);
            buttonText= IsRunning ? "Stop Simulator" : "Run Simulator";
            LoadOrderSummary();
            Interval = 1000;
        }

        private void MainWindow_Closed(object Sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Order.RemoveObserver(LoadOrderSummary);
            s_bl.Admin.StopSimulator();
        }

        private void btnCouriers_Click(object sender, RoutedEventArgs e)
        { new CourierListWindow().Show(); }
        private void btnOrders_Click(object sender, RoutedEventArgs e)
        { new OrderListWindow().Show(); }
        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?",
                                         "Confirm Initialization",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {

                Mouse.OverrideCursor = Cursors.Wait;


                foreach (Window win in Application.Current.Windows)
                {
                    if (win != this)
                        win.Close();
                }


                s_bl.Admin.InitializeDB();
            }
            finally
            {

                Mouse.OverrideCursor = null;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database?",
                                 "Confirm Reset",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                foreach (Window win in Application.Current.Windows)
                {
                    if (win != this)
                        win.Close();
                }

                s_bl.Admin.ResetDB();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<object> OrdersSummary
        {
            get { return (IEnumerable<object>)GetValue(OrdersSummaryProperty); }
            set { SetValue(OrdersSummaryProperty, value); }
        }

        public static readonly DependencyProperty OrdersSummaryProperty =
            DependencyProperty.Register(
                "OrdersSummary",
                typeof(IEnumerable<object>),
                typeof(MainWindow)
            );

        private void LoadOrderSummary()
        {
            int[] summary = s_bl.Order.GetOrdersSummary(123456789);

            int orderStatusCount = Enum.GetValues(typeof(BO.OrderStatus)).Length;
            int scheduleStatusCount = Enum.GetValues(typeof(BO.ScheduleStatus)).Length;

            var list = new List<object>();

            // OrderStatus
            for (int i = 0; i <orderStatusCount; i++)
            {
                var status = (BO.OrderStatus)i;
                if (status == BO.OrderStatus.All)
                    continue;
                list.Add(new
                {
                    DisplayText = $"{status}: {summary[i]}",
                    Tag = status
                });
            }

            // ScheduleStatus
            for (int i = 0; i < scheduleStatusCount; i++)
            {
                var status = (BO.ScheduleStatus)i;
                list.Add(new
                {
                    DisplayText = $"{status}: {summary[orderStatusCount-1 + i]}",
                    Tag = status
                });
            }

            OrdersSummary = list;
        }
        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag == null)
                return;

            // אם זה OrderStatus
            if (button.Tag is BO.OrderStatus orderStatus)
            {
                new OrderListWindow(orderStatus).Show();
                return;
            }

            // אם זה ScheduleStatus
            if (button.Tag is BO.ScheduleStatus scheduleStatus)
            {
                new OrderListWindow(null, scheduleStatus).Show();
                return;
            }
        }
       

        private void UpdateCompanyAddress_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateMaxRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateAvgCarSpeed_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateAvgMotorBikeSpeed_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateAvgBicycleSpeed_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateAvgWalkingSpeed_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateMaxDeliveryTime_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void UpdateInactivityRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if(IsRunning)
            {
                s_bl.Admin.StopSimulator();
                IsRunning = false;
                buttonText = "Run Simulator";

            }
            else
            {
                s_bl.Admin.StartSimulator(Interval);
                IsRunning = true;
                buttonText = "Stop Simulator";
            }

        }

        public string buttonText
        {
            get { return (string)GetValue(buttonTextProperty); }
            set { SetValue(buttonTextProperty, value); }
        }       
        public static readonly DependencyProperty buttonTextProperty =
            DependencyProperty.Register(
                "buttonText",
                typeof(string),
                typeof(MainWindow),
                new PropertyMetadata("Run Simulator")
            );



    }
}
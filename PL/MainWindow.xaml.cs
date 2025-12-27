using BO;
using PL.Courier;
using PL.Order;
using System.Configuration;
using System.Text;
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
        private void btnMaxDeliveryTime_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

        private void btnInactivityRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetConfig(Configuration);
        }

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

        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void configObserver()
        {
            Configuration = s_bl.Admin.GetConfig();
        }

        private void MainWindow_Loaded(object Sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = s_bl.Admin.GetConfig();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Order.AddObserver(LoadOrderSummary);
            LoadOrderSummary();
        }

        private void MainWindow_Closed(object Sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Order.RemoveObserver(LoadOrderSummary);
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
            int[] summary = s_bl.Order.GetOrdersSummary(123456789); // הפונקציה שלך

            int orderStatusCount = Enum.GetValues(typeof(BO.OrderStatus)).Length;
            int scheduleStatusCount = Enum.GetValues(typeof(BO.ScheduleStatus)).Length;

            var list = new List<object>();

            // סטטוס הזמנה – כל סוגים
            for (int i = 0; i < orderStatusCount; i++)
            {
                var status = (BO.OrderStatus)i;
                list.Add(new
                {
                    DisplayText = $"{status}: {summary[i]}",
                    OrderStatus = status,
                    ScheduleStatus = (BO.ScheduleStatus?)null
                });
            }

            // סטטוס עמידה בזמנים – כל סוגים
            for (int i = 0; i < scheduleStatusCount; i++)
            {
                var status = (BO.ScheduleStatus)i;
                list.Add(new
                {
                    DisplayText = $"{status}: {summary[orderStatusCount + i]}",
                    OrderStatus = (BO.OrderStatus?)null,
                    ScheduleStatus = status
                });
            }

            OrdersSummary = list;
        }

        private void SummaryButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic item = (sender as Button)?.DataContext;
            if (item == null) return;

            BO.OrderStatus? orderStatus = item.OrderStatus;
            BO.ScheduleStatus? scheduleStatus = item.ScheduleStatus;

            var window = new OrderListWindow(orderStatus, scheduleStatus);
            window.Show();
        }


    }
}
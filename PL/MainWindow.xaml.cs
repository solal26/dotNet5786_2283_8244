using BO;
using PL.Courier;
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

        private void clockObserver (){
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void configObserver()
        {
            Configuration=s_bl.Admin.GetConfig();
        }

        private void MainWindow_Loaded(object Sender,RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            Configuration = s_bl.Admin.GetConfig();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }

        private void MainWindow_Closed(object Sender,RoutedEventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void btnCouriers_Click(object sender, RoutedEventArgs e)
        { new CourierListWindow().Show(); }

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

        

    }
}
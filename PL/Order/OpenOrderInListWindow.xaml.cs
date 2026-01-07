using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PL.Courier;
using PL.Helpers;

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OpenOrderInListWindow.xaml
    /// </summary>
    public partial class OpenOrderInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Courier? CurrentCourier { get; set; }
        
        public IEnumerable<BO.OpenOrderInList> OpenOrderList
        {
            get { return (IEnumerable<BO.OpenOrderInList>)GetValue(OpenOrderListProperty); }
            set { SetValue(OpenOrderListProperty, value); }
        }
        public static readonly DependencyProperty OpenOrderListProperty =
            DependencyProperty.Register("OpenOrderList", typeof(IEnumerable<BO.OpenOrderInList>), typeof(OpenOrderInListWindow), new PropertyMetadata(null));


        private readonly ObserverMutex _mutex = new(); //stage 7
        private void OpenOrderListObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                OpenOrderList = s_bl.Order.GetOpenOrdersForCourier(CurrentCourier.Id, CurrentCourier.Id);

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    OpenOrderListObserver();
            });

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(OpenOrderListObserver);
            OpenOrderListObserver();
        }
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(OpenOrderListObserver);
        private void btnChooseOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            int OrderId = (int)button.Tag;

            var result = MessageBox.Show($"Are you sure you want to Choose Order {OrderId}?",
                                         "Choose Order",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                s_bl.Order.ChooseOrderForDelivery(CurrentCourier.Id,CurrentCourier.Id,OrderId);

            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (BO.BlInvalidRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public OpenOrderInListWindow(int id)
        {
            InitializeComponent();
            CurrentCourier =
            s_bl.Courier.GetCourier(id, id);
            this.DataContext = this;
        }

        
    }
}

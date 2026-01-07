using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BlApi;
using BO;
using PL.Helpers;


namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
    
        static readonly IBl s_bl = Factory.Get();
        private const int RequesterId = 123456789;

        public IEnumerable<BO.OrderInList> OrderList
        {
            get { return (IEnumerable<BO.OrderInList>)GetValue(OrderListProperty); }
            set { SetValue(OrderListProperty, value); }
        }
        public static readonly DependencyProperty OrderListProperty =
            DependencyProperty.Register("OrderList", typeof(IEnumerable<BO.OrderInList>), typeof(OrderListWindow), new PropertyMetadata(null));


        public BO.OrderStatus oStatus { get; set; } = BO.OrderStatus.All;
        public BO.OrderInListSort orderS { get; set; } 
        public BO.OrderInList? SelectedOrder { get; set; }
        public BO.ScheduleStatus? sStatus { get; set; } 
        public string selectedFilterType { get; set; } 


        private void FilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterTypeComboBox.SelectedItem is ComboBoxItem selected)
            {
                selectedFilterType = (string)selected.Content;
                if ((string)selected.Content == "Order Status")
                {
                    FilterValueComboBox.ItemsSource = Enum.GetValues(typeof(BO.OrderStatus))
                                                          .Cast<BO.OrderStatus>();
                                                       
                }
                else 
                {
                    FilterValueComboBox.ItemsSource = Enum.GetValues(typeof(BO.ScheduleStatus))
                                                          .Cast<BO.ScheduleStatus>();
                }
                FilterValueComboBox.SelectedIndex = 0;
            }
        }
        private void FilterValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterValueComboBox.SelectedItem != null)
            {
                if (selectedFilterType == "Order Status")
                    oStatus = (BO.OrderStatus)FilterValueComboBox.SelectedItem;
                else
                    sStatus = (BO.ScheduleStatus)FilterValueComboBox.SelectedItem;

                orderListObserver();
            }
        }
        private void ChooseSort(object sender, SelectionChangedEventArgs e)=> orderListObserver();

        private readonly ObserverMutex _mutex = new(); //stage 7
        private void orderListObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                BO.OrderInListFilter? filterField = null;
                BO.OrderInListSort? so = orderS;
                object? filterValue = null;

                if (selectedFilterType == "Order Status" && oStatus != BO.OrderStatus.All)
                {
                    filterField = BO.OrderInListFilter.OrderStatus;
                    filterValue = oStatus;
                }
                else if (selectedFilterType == "Schedule Status" && sStatus != null)
                {
                    filterField = BO.OrderInListFilter.ScheduleStatus;
                    filterValue = sStatus;
                }

                try
                {
                    OrderList = s_bl.Order.GetOrdersList(
                        requesterId: RequesterId,
                        filterField: filterField,
                        filterValue: filterValue,
                        sortBy: so
                    );
                }
                catch (BO.BlUnauthorizedAccessException ex)
                {
                    MessageBox.Show(ex.Message, "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Stop);
                    OrderList = new List<BO.OrderInList>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading orders: {ex.Message}", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    OrderList = new List<BO.OrderInList>();
                }

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    orderListObserver();
            });
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(orderListObserver);
            orderListObserver();
        }
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(orderListObserver);
        private void lsvOrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedOrder != null)
            {
                new OrderWindow(SelectedOrder.OrderId).Show();
            }
        }

        // Click on the "Add New Order" button
        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
        }

        // Click on the "Cancel" button (as per requirement)
        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            int orderId = (int)button.Tag;

            var result = MessageBox.Show($"Are you sure you want to cancel order {orderId}? An email might be sent to the courier if applicable.",
                                         "Cancel Order",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Call the BL method to cancel the order
                s_bl.Order.CancelOrder(RequesterId, orderId);
                // The BL is responsible for validation (not closed) and sending the courier email.
            }
            // Capture specific BL exceptions
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show($"Error: Order {orderId} not found or could not be cancelled. {ex.Message}");
            }
            catch (BO.BlInvalidRequestException ex)
            {
                MessageBox.Show($"Cancellation failed: {ex.Message}");
            }
          



        }
        public OrderListWindow(BO.OrderStatus? orderStatus = null, BO.ScheduleStatus? scheduleStatus = null)
        {
            InitializeComponent();
            if (orderStatus != null)
            {
                oStatus = orderStatus.Value;
                selectedFilterType = "Order Status";
            }
            if (scheduleStatus != null)
            {
                sStatus = scheduleStatus.Value;
                selectedFilterType = "Schedule Status";
            }
        }
    }
}
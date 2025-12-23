using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;


namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderListWindow.xaml
    /// </summary>
    public partial class OrderListWindow : Window
    {
        // 1. Access to the Business Logic (BL) Layer
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



        // Main method to query the BL for the list
        private void queryOrderList()
        {
            BO.OrderInListFilter? filterField = null;
            BO.OrderInListSort? so = orderS;
            object? filterValue = null;

            if (oStatus != BO.OrderStatus.All)
            {
                filterField = BO.OrderInListFilter.OrderStatus;
                filterValue = oStatus;
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
        }

        // Handler for the filter change (ComboBox SelectionChanged)
        private void ChooseStatus(object sender, SelectionChangedEventArgs e)
        {
            queryOrderList();
        }
        private void ChooseSort(object sender, SelectionChangedEventArgs e)
        {
            queryOrderList();
        }

        // Observer callback method (called by the BL upon changes)
        private void orderListObserver()
            => queryOrderList();

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to the Observer for external updates
            s_bl.Order.AddObserver(orderListObserver);
            queryOrderList(); // Initial loading of the list
        }

        private void Window_Closed(object sender, EventArgs e)
            // Unsubscribe from the Observer to prevent memory leaks
            => s_bl.Order.RemoveObserver(orderListObserver);


    

        // Double-click to open the "Single Order Management" (OrderWindow)
        private void lsvOrderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedOrder != null)
            {
                // Open the details window in Update mode, passing the Order ID
                new OrderWindow(SelectedOrder.OrderId).Show();
            }
        }

        // Click on the "Add New Order" button
        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            // Open the details window in Add mode (default ID 0)
            new OrderWindow().Show();
            // The Observer will handle list update once the addition is successful.
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
        public OrderListWindow()
        {
            InitializeComponent();
        }
    }
}
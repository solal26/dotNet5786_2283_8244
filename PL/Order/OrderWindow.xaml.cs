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

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
        public bool _isAddMode { get; }
        public BO.Order? CurrentOrder
        {
            get => (BO.Order?)GetValue(CurrentOrderProperty);
            set => SetValue(CurrentOrderProperty, value);
        }

        public static readonly DependencyProperty CurrentOrderProperty =
            DependencyProperty.Register("CurrentOrder",
                typeof(BO.Order),
                typeof(OrderWindow),
                new PropertyMetadata(null));
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText",
            typeof(string),
            typeof(OrderWindow),
            new PropertyMetadata(""));

        private void btnAllDeliveries_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentOrder.Deliveries != null)
            {
                new DeliveryPerOrderInListWindow(CurrentOrder!.Id).Show();
            }
            else
            {
                MessageBox.Show("No last deliveris assigned");
            }
        }
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isAddMode)  // מצב הוספה
                {
                    bl.Order.AddOrder(123456789, CurrentOrder);
                    this.Close();
                    MessageBox.Show("Order added successfully");
                }
                else   // מצב עדכון
                {
                    bl.Order.UpdateOrder(123456789, CurrentOrder);
                    this.Close();
                    MessageBox.Show("Order updated successfully");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            int orderId = CurrentOrder.Id;

            var result = MessageBox.Show($"Are you sure you want to cancel order {orderId}? An email might be sent to the courier if applicable.",
                                         "Cancel Order",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                bl.Order.CancelOrder(123456789, orderId);

            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show($"Error: Order {orderId} not found or could not be cancelled. {ex.Message}");
            }
            catch (BO.BlInvalidRequestException ex)
            {
                MessageBox.Show($"Cancellation failed: {ex.Message}");
            }
        }
        public IEnumerable<BO.OrderStatus> OrderStatuses => Enum.GetValues(typeof(BO.OrderStatus)).Cast<BO.OrderStatus>();
        public IEnumerable<BO.OrderType> OrderTypes => Enum.GetValues(typeof(BO.OrderType)).Cast<BO.OrderType>();
        public IEnumerable<BO.ScheduleStatus> ScheduleStatuses => Enum.GetValues(typeof(BO.ScheduleStatus)).Cast<BO.ScheduleStatus>();
        private void OrderObserver()
        {
            int id = CurrentOrder!.Id;
            CurrentOrder = null;
            CurrentOrder = bl.Order.GetOrder(123456789, id);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            bl.Courier.AddObserver(CurrentOrder.Id, OrderObserver);
            bl.Order.AddObserver(CurrentOrder.Id, OrderObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            bl.Courier.RemoveObserver(CurrentOrder.Id, OrderObserver);
            bl.Order.RemoveObserver(CurrentOrder.Id, OrderObserver);
        }
    


        public OrderWindow(int Id = 0)
        {
            ButtonText = Id == 0 ? "Add" : "Update";
            _isAddMode = Id == 0;
            InitializeComponent();
            CurrentOrder = Id == 0
           ? new BO.Order()
           : bl.Order.GetOrder(123456789, Id);
        }
    }
}

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
using PL.Order;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierWindow.xaml
    /// </summary>
    public partial class CourierWindow : Window
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
        public bool _isAddMode { get; }
        public BO.Courier? CurrentCourier
        {
            get => (BO.Courier?)GetValue(CurrentCourierProperty);
            set => SetValue(CurrentCourierProperty, value);
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier",
                typeof(BO.Courier),
                typeof(CourierWindow),
                new PropertyMetadata(null));
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText",
            typeof(string),
            typeof(CourierWindow),
            new PropertyMetadata(""));

        private void btnCurrentOrder_Click(object sender, RoutedEventArgs e)
        {

            if (CurrentCourier!.CurrentOrder != null)
            {
                new OrderInProgressWindow(CurrentCourier!.Id).Show();
            }
            else
            {
                MessageBox.Show("No current order assigned");
            }
        }
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isAddMode)  // מצב הוספה
                {
                    bl.Courier.AddCourier(123456789, CurrentCourier);
                    this.Close();
                    MessageBox.Show("Courier added successfully");
                }

                else   // מצב עדכון
                {
                    bl.Courier.UpdateCourier(CurrentCourier.Id, CurrentCourier);
                    MessageBox.Show("Courier updated successfully");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public IEnumerable<BO.DeliveryType> DeliveryTypes => Enum.GetValues(typeof(BO.DeliveryType)).Cast<BO.DeliveryType>();

        public CourierWindow(int courierId = 0)
        {
            ButtonText = courierId == 0 ? "Add" : "Update";
            _isAddMode = courierId == 0;

            InitializeComponent();
            CurrentCourier = courierId == 0
           ? new BO.Courier()
           : bl.Courier.GetCourier(courierId, courierId);
            
        }

        private void CourierObserver()
        {
            int id = CurrentCourier!.Id;
            CurrentCourier = null;
            CurrentCourier = bl.Courier.GetCourier(id, id);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            bl.Courier.AddObserver(CurrentCourier.Id, CourierObserver);
            bl.Order.AddObserver(CurrentCourier.Id, CourierObserver);
         
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
            bl.Courier.RemoveObserver(CurrentCourier.Id, CourierObserver);
            bl.Order.RemoveObserver(CurrentCourier.Id, CourierObserver);
        }

        private void btnDeleteCourier_Click(object sender, RoutedEventArgs e)
        {
            

            int courierId = CurrentCourier.Id;

            var result = MessageBox.Show($"Are you sure you want to delete courier {courierId}?",
                                         "Delete Courier",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                bl.Courier.DeleteCourier(courierId, courierId);
                this.Close();


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
    }
}

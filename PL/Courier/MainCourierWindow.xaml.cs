using PL.Order;
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

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for MainCourierWindow.xaml
    /// </summary>
    public partial class MainCourierWindow : Window
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
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              // מצב עדכון
                {
                    bl.Courier.UpdateCourier(CurrentCourier.Id, CurrentCourier);
                    MessageBox.Show("Courier updated successfully");
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public IEnumerable<BO.DeliveryType> DeliveryTypes => Enum.GetValues(typeof(BO.DeliveryType)).Cast<BO.DeliveryType>();

        public MainCourierWindow(int courierId )
        {
            InitializeComponent();
            ButtonText = CurrentCourier.CurrentOrder == null ? "New delivery" : "EndDelivery";
            _isAddMode = courierId == 0;
            
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
            if (CurrentCourier != null)
                bl.Courier.AddObserver(CurrentCourier.Id, CourierObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCourier != null)
                bl.Courier.RemoveObserver(CurrentCourier.Id, CourierObserver);

        }
        private void btnChooseOrEnd_Click(object sender, RoutedEventArgs e)
        {
            if(ButtonText== "New delivery")
            {
                new OpenOrderInListWindow().Show();
            }
        }
    }

}


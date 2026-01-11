using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using BO;
using PL.Helpers;
using PL.Order;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for MainCourierWindow.xaml
    /// </summary>
    public partial class MainCourierWindow : Window
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
       
        public bool haveCurrentOlder
        {
            get => (bool)GetValue(HaveCurrentOlderProperty);
            set => SetValue(HaveCurrentOlderProperty, value);
        }

        public static readonly DependencyProperty HaveCurrentOlderProperty =
            DependencyProperty.Register("haveCurrentOlder", typeof(bool), typeof(MainCourierWindow), new PropertyMetadata(false));
        public BO.Courier? CurrentCourier
        {
            get => (BO.Courier?)GetValue(CurrentCourierProperty);
            set => SetValue(CurrentCourierProperty, value);
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier",
                typeof(BO.Courier),
                typeof(MainCourierWindow),
                new PropertyMetadata(null));
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText",
            typeof(string),
            typeof(MainCourierWindow),
            new PropertyMetadata(""));

        private void btnCurrenrOrder_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCourier!.CurrentOrder != null)
            {
                new OrderInProgressWindow(CurrentCourier!.Id).Show();
            }
        }
        private void btnChooseOrEnd_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText == "New delivery")
            {
              
                new OpenOrderInListWindow(CurrentCourier!.Id).Show();
            }
            else
            {
                try
                {
                    bl.Order.CompleteOrderDelivery(CurrentCourier!.Id, CurrentCourier!.Id, CurrentCourier!.CurrentOrder.DeliveryId);
                    MessageBox.Show("Delivery ended successfully");
                }
                catch (BLTemporaryNotAvailableException ex_)
                {
                    MessageBox.Show(ex_.Message);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void btnDeliveryHistory_Click(object sender, RoutedEventArgs e)
        {
            new DeliveryHistoryWindow(CurrentCourier!.Id).Show();
        }
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
            catch (BLTemporaryNotAvailableException ex_)
            {
                MessageBox.Show(ex_.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public IEnumerable<BO.DeliveryType> DeliveryTypes => Enum.GetValues(typeof(BO.DeliveryType)).Cast<BO.DeliveryType>();

        public MainCourierWindow(int courierId)
        {

            InitializeComponent();
           
            CurrentCourier = 
           bl.Courier.GetCourier(courierId, courierId);
          
        }
        private readonly ObserverMutex _mutex = new(); //stage 7
        private void CourierObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                int id = CurrentCourier!.Id;
                CurrentCourier = null;


                CurrentCourier = bl.Courier.GetCourier(id, id);
                ButtonText = CurrentCourier.CurrentOrder == null ? "New delivery" : "EndDelivery";
                haveCurrentOlder = CurrentCourier!.CurrentOrder == null;

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    CourierObserver();
            });
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            bl.Courier.AddObserver(CurrentCourier.Id, CourierObserver);
            bl.Order.AddObserver(CurrentCourier.Id, CourierObserver);
            bl.Order.AddObserver(CourierObserver);
            ButtonText = CurrentCourier!.CurrentOrder == null ? "New delivery" : "EndDelivery";
            haveCurrentOlder = CurrentCourier!.CurrentOrder == null;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCourier != null)
                bl.Courier.RemoveObserver(CurrentCourier.Id, CourierObserver);
            bl.Order.RemoveObserver(CurrentCourier.Id, CourierObserver);
            bl.Order.RemoveObserver(CourierObserver);
        }


    }
}


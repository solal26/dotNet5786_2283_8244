using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BO;

namespace PL.Courier
{
    /// <summary>
    /// Interaction logic for CourierListWindow.xaml
    /// </summary>
    public partial class CourierListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public bool haveAcurrentOrder { get; set; } 
        public IEnumerable<BO.CourierInList> CourierList
        {
            get { return (IEnumerable<BO.CourierInList>)GetValue(CourierListProperty); }
            set { SetValue(CourierListProperty, value); }
        }
        public static readonly DependencyProperty CourierListProperty =
            DependencyProperty.Register("CourierList", typeof(IEnumerable<BO.CourierInList>), typeof(CourierListWindow), new PropertyMetadata(null));
        public BO.DeliveryType dtype { get; set; } = BO.DeliveryType.All;
        public BO.CourierSortBy courierSortBy { get; set; }
        private void ChooseDelivery(object sender, SelectionChangedEventArgs e) => courierListObserver();


        private void ChooseSort(object sender, SelectionChangedEventArgs e) => courierListObserver();


        private readonly ObserverMutex _mutex = new(); //stage 7

        private void courierListObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                CourierList = s_bl.Courier.GetListOfCourier(0, dtype, courierSortBy);
                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    courierListObserver();
            });
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
             => s_bl.Courier.AddObserver(courierListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Courier.RemoveObserver(courierListObserver);

        public BO.CourierInList? SelectedCourier { get; set; }
        private void lsvCourierList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCourier != null)
                new CourierWindow(SelectedCourier.Id).Show();
        }
        private void btnAddCourier_Click(object sender, RoutedEventArgs e)
        {
            new CourierWindow().Show();
        }
        private void btnDeleteCourier_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            int courierId = (int)button.Tag;

            var result = MessageBox.Show($"Are you sure you want to delete courier {courierId}?",
                                         "Delete Courier",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                s_bl.Courier.DeleteCourier(courierId, courierId);
                
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch( BO.BlInvalidRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public CourierListWindow()
        {
            InitializeComponent();
        }
    }
}

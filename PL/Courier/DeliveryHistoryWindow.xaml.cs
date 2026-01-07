using PL.Helpers;
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
    /// Interaction logic for DeliveryHistoryWindow.xaml
    /// </summary>
    public partial class DeliveryHistoryWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Courier? CurrentCourier { get; set; }
        public IEnumerable<BO.ClosedDeliveryInList> ClosedDeliveryList
        {
            get { return (IEnumerable<BO.ClosedDeliveryInList>)GetValue(ClosedDeliveryInListProperty); }
            set { SetValue(ClosedDeliveryInListProperty, value); }
        }
        public static readonly DependencyProperty ClosedDeliveryInListProperty =
            DependencyProperty.Register("ClosedDeliveryList", typeof(IEnumerable<BO.ClosedDeliveryInList>), typeof(DeliveryHistoryWindow), new PropertyMetadata(null));

        private readonly ObserverMutex _mutex = new(); //stage 7
        private void ClosedDeliveryInListObserver()
        {

            #region Stage 7 (for multithreading)
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {

                ClosedDeliveryList = s_bl.Order.GetClosedDeliveries(CurrentCourier.Id, CurrentCourier.Id);
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    ClosedDeliveryInListObserver();
            });
            #endregion Stage 7 (for multithreading)
        }
     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(ClosedDeliveryInListObserver);
            ClosedDeliveryInListObserver();
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(ClosedDeliveryInListObserver);

        public DeliveryHistoryWindow(int id)
        {
            InitializeComponent();
            CurrentCourier =
            s_bl.Courier.GetCourier(id, id);
            this.DataContext = this;
        }
    }
}

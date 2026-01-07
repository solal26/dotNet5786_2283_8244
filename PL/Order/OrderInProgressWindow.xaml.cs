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
using PL.Courier;
using PL.Helpers;

namespace PL.Order
{
    /// <summary>
    /// Interaction logic for OrderInProgressWindow.xaml
    /// </summary>
    public partial class OrderInProgressWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Courier? CurrentCourier { get; set; }
        public BO.OrderInProgress? Orderinprogress
        {
            get => (BO.OrderInProgress?)GetValue(OrderinprogressProperty);
            set => SetValue(OrderinprogressProperty, value);
        }

        public static readonly DependencyProperty OrderinprogressProperty =
            DependencyProperty.Register("Orderinprogress",
                typeof(BO.OrderInProgress),
                typeof(OrderInProgressWindow),
                new PropertyMetadata(null));


        private readonly ObserverMutex _mutex = new(); //stage 7
        private void OrderinprogressObserver()
        {
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                Orderinprogress = CurrentCourier.CurrentOrder;

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    OrderinprogressObserver();
            });
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Courier.AddObserver(OrderinprogressObserver);
            
        }
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Courier.RemoveObserver(OrderinprogressObserver);

        
    
        public OrderInProgressWindow(int ID)
        {
            InitializeComponent();
            CurrentCourier =
           s_bl.Courier.GetCourier(ID, ID);
            Orderinprogress = CurrentCourier.CurrentOrder;  
        }
    }
}

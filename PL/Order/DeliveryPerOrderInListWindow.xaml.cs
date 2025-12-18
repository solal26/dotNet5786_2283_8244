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
    /// Interaction logic for DeliveryPerOrderInListWindow.xaml
    /// </summary>
    public partial class DeliveryPerOrderInListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Order? CurrentOrder { get; set; }
        public IEnumerable<BO.DeliveryPerOrderInList> DeliveryPerOrderList
        {
            get { return (IEnumerable<BO.DeliveryPerOrderInList>)GetValue(DeliveryPerOrderListProperty); }
            set { SetValue(DeliveryPerOrderListProperty, value); }
        }
        public static readonly DependencyProperty DeliveryPerOrderListProperty =
            DependencyProperty.Register("DeliveryPerOrderList", typeof(IEnumerable<BO.DeliveryPerOrderInList>), typeof(DeliveryPerOrderInListWindow), new PropertyMetadata(null));

        private void queryDeliveryPerOrderList()
             => DeliveryPerOrderList = s_bl.Order.GetDeliveriesForOrder(123456789, CurrentOrder.Id);

        private void DeliveryPerOrderListObserver()
            => queryDeliveryPerOrderList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Order.AddObserver(DeliveryPerOrderListObserver);
            queryDeliveryPerOrderList();
        }
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Order.RemoveObserver(DeliveryPerOrderListObserver);


        public DeliveryPerOrderInListWindow(int id)
        {
            InitializeComponent();
            CurrentOrder =
            s_bl.Order.GetOrder(123456789, id);
            this.DataContext = this;
        }
    }
}

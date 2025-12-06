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
    /// Interaction logic for CourierWindow.xaml
    /// </summary>
    public partial class CourierWindow : Window
    {
        private readonly BlApi.IBl bl = BlApi.Factory.Get();
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
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCourier!.Id == 0)  // מצב הוספה
                {
                    bl.Courier.AddCourier(0, CurrentCourier);
                    MessageBox.Show("Courier added successfully");
                }
                else   // מצב עדכון
                {
                    bl.Courier.UpdateCourier(0, CurrentCourier);
                    MessageBox.Show("Courier updated successfully");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public CourierWindow(int courierId = 0)
        {
            ButtonText = courierId == 0 ? "Add" : "Update";
            InitializeComponent();
            CurrentCourier = courierId == 0
           ? new BO.Courier()
           : bl.Courier.GetCourier(0, courierId);
        }

 


    }
}

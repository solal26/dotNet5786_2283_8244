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
using BO;
using PL.Courier;

namespace PL
{
    public partial class AdminChoiceWindow : Window
    {
        public int userName { get; set; }
        public AdminChoiceWindow(int id)
        {
            userName = id;
            InitializeComponent();
        }

        private void System_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }

        private void Couriers_Click(object sender, RoutedEventArgs e)
        {
            new MainCourierWindow(userName).Show();
            Close();
        }
    }
}

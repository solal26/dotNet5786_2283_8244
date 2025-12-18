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

namespace PL
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        static BO.Config conf = s_bl.Admin.GetConfig();
        int id= conf.ManagerId;
        public string Username { get; set; }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(Username) == id)
                    new MainWindow().Show();
                else
                    if (s_bl.Courier.GetCourier(int.Parse(Username), int.Parse(Username)) != null)//l(d=>d.Id==(int.Parse(Username)).any())
                    new Courier.MainCourierWindow(int.Parse(Username)).Show();


            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show("invalid Id");
            }
        }
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;

        }
    }
}

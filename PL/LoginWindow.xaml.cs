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

        
      
        public string Username { get; set; }
        public string Password { get; set; }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (s_bl.Courier.Login(int.Parse(Username), Password)=="Admin")
                    new MainWindow().Show();
                else
                    
                    new Courier.MainCourierWindow(int.Parse(Username)).Show();


            }
            catch (UnauthorizedAccessException ex) { 
                MessageBox.Show(ex.Message);
            }
            catch(FormatException ex)
            {
                MessageBox.Show("Invalid ID format. Please enter a numeric ID.");
            }
        }
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;

        }
    }
}

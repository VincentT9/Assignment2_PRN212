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

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for CustomerDashboard.xaml
    /// </summary>
    public partial class CustomerDashboard : Window
    {
        public CustomerDashboard()
        {
            InitializeComponent();

            // Set welcome message
            txtWelcomeMessage.Text = $"Welcome, {App.CurrentCustomer?.ContactName}";

            // Navigate to Order History by default
            NavigateTo("Order History");
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            string destination = (sender as Button).Content.ToString();
            NavigateTo(destination);
        }

        private void NavigateTo(string destination)
        {
            switch (destination)
            {
                case "Order History":
                    mainFrame.Navigate(new CustomerOrderHistory());
                    break;
                case "My Profile":
                    mainFrame.Navigate(new CustomerProfile());
                    break;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Clear current user
            App.CurrentEmployee = null;
            App.CurrentCustomer = null;

            // Navigate back to login
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

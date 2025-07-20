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
    /// Interaction logic for AdminDashboard.xaml
    /// </summary>
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();

            // Set welcome message
            txtWelcomeMessage.Text = $"Welcome, {App.CurrentEmployee?.Name}";

            // Navigate to Customers by default
            NavigateTo("Customers");
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
                case "Customers":
                    mainFrame.Navigate(new CustomerManagement());
                    break;
                case "Products":
                    mainFrame.Navigate(new ProductManagement());
                    break;
                case "Orders":
                    mainFrame.Navigate(new OrderManagement());
                    break;
                case "Employees":
                    mainFrame.Navigate(new EmployeeManagement());
                    break;
                case "Reports":
                    mainFrame.Navigate(new SalesReports());
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

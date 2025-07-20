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
using Microsoft.Extensions.DependencyInjection;
using Repositories;

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICustomerRepository _customerRepository;
        private bool _isAdminMode = true;

        public LoginWindow()
        {
            InitializeComponent();

            rbAdmin.Checked += rbAdmin_Checked;
            rbCustomer.Checked += rbCustomer_Checked;

            // Get repositories from service provider
            _employeeRepository = App.ServiceProvider.GetService<IEmployeeRepository>();
            _customerRepository = App.ServiceProvider.GetService<ICustomerRepository>();
        }

        private void rbAdmin_Checked(object sender, RoutedEventArgs e)
        {
            _isAdminMode = true;
            if (txtUsernameLabel != null)
                txtUsernameLabel.Text = "Username";
            ClearFields();
        }

        private void rbCustomer_Checked(object sender, RoutedEventArgs e)
        {
            _isAdminMode = false;
            if (txtUsernameLabel != null)
                txtUsernameLabel.Text = "Phone";
            ClearFields();
        }

        private void ClearFields()
        {
            if (txtUsername != null)
                txtUsername.Text = string.Empty;
            if (txtPassword != null)
                txtPassword.Password = string.Empty;
            if (txtErrorMessage != null)
                txtErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isAdminMode)
                {
                    // Admin login using username and password
                    var employee = _employeeRepository.Login(txtUsername.Text, txtPassword.Password);
                    if (employee != null)
                    {
                        // Store logged in user info and navigate to admin dashboard
                        App.CurrentEmployee = employee;

                        var adminDashboard = new AdminDashboard();
                        adminDashboard.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError("Invalid username or password");
                    }
                }
                else
                {
                    // Customer login using phone as username and password
                    var customer = _customerRepository.GetCustomerByPhone(txtUsername.Text);
                    if (customer != null && txtUsername.Text == txtPassword.Password) // Phone = password for customer
                    {
                        // Store logged in user info and navigate to customer dashboard
                        App.CurrentCustomer = customer;

                        var customerDashboard = new CustomerDashboard();
                        customerDashboard.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowError("Invalid phone number or password");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Login error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            txtErrorMessage.Visibility = Visibility.Visible;
        }
    }
}

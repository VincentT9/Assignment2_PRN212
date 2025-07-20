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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessObjects;
using Microsoft.Extensions.DependencyInjection;
using Repositories;

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for CustomerProfile.xaml
    /// </summary>
    public partial class CustomerProfile : Page
    {
        private readonly ICustomerRepository _customerRepository;
        private Customer _currentCustomer;

        public CustomerProfile()
        {
            InitializeComponent();
            _customerRepository = App.ServiceProvider.GetService<ICustomerRepository>();

            LoadCustomerProfile();
        }

        private void LoadCustomerProfile()
        {
            try
            {
                _currentCustomer = _customerRepository.GetCustomerById(App.CurrentCustomer.CustomerId);

                if (_currentCustomer != null)
                {
                    txtCompanyName.Text = _currentCustomer.CompanyName;
                    txtContactName.Text = _currentCustomer.ContactName;
                    txtContactTitle.Text = _currentCustomer.ContactTitle;
                    txtPhone.Text = _currentCustomer.Phone;
                    txtAddress.Text = _currentCustomer.Address;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
            {
                MessageBox.Show("Company Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCompanyName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContactName.Text))
            {
                MessageBox.Show("Contact Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtContactName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Phone is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return;
            }

            try
            {
                // Update customer object
                _currentCustomer.CompanyName = txtCompanyName.Text.Trim();
                _currentCustomer.ContactName = txtContactName.Text.Trim();
                _currentCustomer.ContactTitle = txtContactTitle.Text?.Trim();
                _currentCustomer.Phone = txtPhone.Text.Trim();
                _currentCustomer.Address = txtAddress.Text?.Trim();

                // Save changes
                _customerRepository.Update(_currentCustomer);

                // Update the current customer in the application
                App.CurrentCustomer = _currentCustomer;

                MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

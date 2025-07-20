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
    /// Interaction logic for CustomerManagement.xaml
    /// </summary>
    public partial class CustomerManagement : Page
    {
        private readonly ICustomerRepository _customerRepository;
        private Customer _selectedCustomer;

        public CustomerManagement()
        {
            InitializeComponent();
            _customerRepository = App.ServiceProvider.GetService<ICustomerRepository>();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerRepository.GetAll();
                dgCustomers.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCustomer = dgCustomers.SelectedItem as Customer;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadCustomers();
                    return;
                }

                var searchResults = _customerRepository.SearchCustomers(txtSearch.Text);
                dgCustomers.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadCustomers();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomerDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _customerRepository.Add(dialog.Customer);
                    LoadCustomers();
                    MessageBox.Show("Customer added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new CustomerDialog();
            dialog.Customer = new Customer
            {
                CustomerId = _selectedCustomer.CustomerId,
                CompanyName = _selectedCustomer.CompanyName,
                ContactName = _selectedCustomer.ContactName,
                ContactTitle = _selectedCustomer.ContactTitle,
                Address = _selectedCustomer.Address,
                Phone = _selectedCustomer.Phone
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _customerRepository.Update(dialog.Customer);
                    LoadCustomers();
                    MessageBox.Show("Customer updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the customer '{_selectedCustomer.CompanyName}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _customerRepository.Delete(_selectedCustomer.CustomerId);
                    LoadCustomers();
                    MessageBox.Show("Customer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

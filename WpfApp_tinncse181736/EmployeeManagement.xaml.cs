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
    /// Interaction logic for EmployeeManagement.xaml
    /// </summary>
    public partial class EmployeeManagement : Page
    {
        private readonly IEmployeeRepository _employeeRepository;
        private Employee _selectedEmployee;

        public EmployeeManagement()
        {
            InitializeComponent();
            _employeeRepository = App.ServiceProvider.GetService<IEmployeeRepository>();

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeRepository.GetAll();
                dgEmployees.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedEmployee = dgEmployees.SelectedItem as Employee;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadEmployees();
                    return;
                }

                var searchResults = _employeeRepository.SearchEmployees(txtSearch.Text);
                dgEmployees.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadEmployees();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EmployeeDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _employeeRepository.Add(dialog.Employee);
                    LoadEmployees();
                    MessageBox.Show("Employee added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                MessageBox.Show("Please select an employee to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Do not allow editing of the logged-in employee
            if (_selectedEmployee.EmployeeId == App.CurrentEmployee?.EmployeeId)
            {
                MessageBox.Show("You cannot edit your own account from here. Use the profile section.",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new EmployeeDialog();
            dialog.Employee = new Employee
            {
                EmployeeId = _selectedEmployee.EmployeeId,
                Name = _selectedEmployee.Name,
                UserName = _selectedEmployee.UserName,
                Password = _selectedEmployee.Password, // Will be replaced if changed
                JobTitle = _selectedEmployee.JobTitle,
                BirthDate = _selectedEmployee.BirthDate,
                HireDate = _selectedEmployee.HireDate,
                Address = _selectedEmployee.Address
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _employeeRepository.Update(dialog.Employee);
                    LoadEmployees();
                    MessageBox.Show("Employee updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                MessageBox.Show("Please select an employee to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Do not allow deleting the logged-in employee
            if (_selectedEmployee.EmployeeId == App.CurrentEmployee?.EmployeeId)
            {
                MessageBox.Show("You cannot delete your own account.",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the employee '{_selectedEmployee.Name}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _employeeRepository.Delete(_selectedEmployee.EmployeeId);
                    LoadEmployees();
                    MessageBox.Show("Employee deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

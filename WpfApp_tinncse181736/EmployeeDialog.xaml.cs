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
using BusinessObjects;

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for EmployeeDialog.xaml
    /// </summary>
    public partial class EmployeeDialog : Window
    {
        public Employee Employee { get; set; }
        private bool _isNewEmployee = true;

        public EmployeeDialog()
        {
            InitializeComponent();
            Employee = new Employee();

            // Set default hire date to today
            dpHireDate.SelectedDate = DateTime.Now;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // If this is an edit operation, populate fields
            if (Employee.EmployeeId != 0)
            {
                _isNewEmployee = false;
                txtName.Text = Employee.Name;
                txtUsername.Text = Employee.UserName;
                // Password is not shown for security, but will be required if changed
                txtPassword.Password = "********"; // Placeholder
                txtConfirmPassword.Password = "********"; // Placeholder
                txtJobTitle.Text = Employee.JobTitle;
                dpBirthDate.SelectedDate = Employee.BirthDate;
                dpHireDate.SelectedDate = Employee.HireDate;
                txtAddress.Text = Employee.Address;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (_isNewEmployee && string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Password is required for new employees.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            if (_isNewEmployee && txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Password and confirmation do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtJobTitle.Text))
            {
                MessageBox.Show("Job Title is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtJobTitle.Focus();
                return;
            }

            if (dpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Birth Date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpBirthDate.Focus();
                return;
            }

            if (dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Hire Date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpHireDate.Focus();
                return;
            }

            // Validate age (must be at least 18)
            if (dpBirthDate.SelectedDate.HasValue)
            {
                var age = DateTime.Now.Year - dpBirthDate.SelectedDate.Value.Year;
                if (dpBirthDate.SelectedDate.Value.Date > DateTime.Now.AddYears(-age)) age--;

                if (age < 18)
                {
                    MessageBox.Show("Employee must be at least 18 years old.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpBirthDate.Focus();
                    return;
                }
            }

            // Update employee object with form values
            Employee.Name = txtName.Text.Trim();
            Employee.UserName = txtUsername.Text.Trim();

            // Only update password if a new one was provided (not the placeholder)
            if (_isNewEmployee || txtPassword.Password != "********")
            {
                Employee.Password = txtPassword.Password;
            }

            Employee.JobTitle = txtJobTitle.Text.Trim();
            Employee.BirthDate = dpBirthDate.SelectedDate;
            Employee.HireDate = dpHireDate.SelectedDate;
            Employee.Address = txtAddress.Text?.Trim();

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

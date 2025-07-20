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
    /// Interaction logic for CustomerDialog.xaml
    /// </summary>
    public partial class CustomerDialog : Window
    {
        public Customer Customer { get; set; }

        public CustomerDialog()
        {
            InitializeComponent();
            Customer = new Customer();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // If this is an edit operation, populate fields
            if (Customer.CustomerId != 0)
            {
                txtCompanyName.Text = Customer.CompanyName;
                txtContactName.Text = Customer.ContactName;
                txtContactTitle.Text = Customer.ContactTitle;
                txtAddress.Text = Customer.Address;
                txtPhone.Text = Customer.Phone;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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

            // Update customer object with form values
            Customer.CompanyName = txtCompanyName.Text.Trim();
            Customer.ContactName = txtContactName.Text.Trim();
            Customer.ContactTitle = txtContactTitle.Text?.Trim();
            Customer.Address = txtAddress.Text?.Trim();
            Customer.Phone = txtPhone.Text.Trim();

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

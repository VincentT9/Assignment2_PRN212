using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Extensions.DependencyInjection;
using Repositories;

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for OrderDialog.xaml
    /// </summary>
    public partial class OrderDialog : Window
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;

        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        private ObservableCollection<OrderDetailViewModel> _orderItems;
        private decimal _totalOrderAmount = 0;

        public OrderDialog(ICustomerRepository customerRepository, IEmployeeRepository employeeRepository, IOrderDetailRepository orderDetailRepository)
        {
            InitializeComponent();

            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = App.ServiceProvider.GetService<IProductRepository>();

            Order = new Order();
            OrderDetails = new List<OrderDetail>();
            _orderItems = new ObservableCollection<OrderDetailViewModel>();

            dgOrderItems.ItemsSource = _orderItems;

            // Set default order date to today
            dpOrderDate.SelectedDate = DateTime.Now;

            LoadCustomers();
            LoadEmployees();
            LoadProducts();
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerRepository.GetAll();
                cmbCustomer.ItemsSource = customers;

                // If customer role, preselect the current customer
                if (App.CurrentCustomer != null)
                {
                    cmbCustomer.SelectedValue = App.CurrentCustomer.CustomerId;
                    cmbCustomer.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _employeeRepository.GetAll();
                cmbEmployee.ItemsSource = employees;

                // Preselect current employee if admin role
                if (App.CurrentEmployee != null)
                {
                    cmbEmployee.SelectedValue = App.CurrentEmployee.EmployeeId;
                    cmbEmployee.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productRepository.GetAll()
                    .Where(p => !p.Discontinued && p.UnitsInStock > 0);
                cmbProduct.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProduct.SelectedItem is Product selectedProduct)
            {
                txtUnitPrice.Text = selectedProduct.UnitPrice?.ToString("C");
            }
            else
            {
                txtUnitPrice.Text = string.Empty;
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Please select a product.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Quantity must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQuantity.Focus();
                return;
            }

            if (!decimal.TryParse(txtDiscount.Text, out decimal discountPercent) || discountPercent < 0 || discountPercent > 100)
            {
                MessageBox.Show("Discount must be a number between 0 and 100.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDiscount.Focus();
                return;
            }

            var product = cmbProduct.SelectedItem as Product;

            // Check if there's enough stock
            if (product.UnitsInStock < quantity)
            {
                MessageBox.Show($"Not enough stock. Only {product.UnitsInStock} units available.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if product already exists in the order
            var existingItem = _orderItems.FirstOrDefault(item => item.ProductID == product.ProductId);
            if (existingItem != null)
            {
                MessageBox.Show("This product is already in the order. Please remove it first if you want to change the quantity.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Convert discount from percentage to decimal (0-1)
            decimal discount = discountPercent / 100;

            // Calculate total
            decimal unitPrice = product.UnitPrice ?? 0;
            decimal total = unitPrice * quantity * (1 - discount);

            // Add to order items collection
            _orderItems.Add(new OrderDetailViewModel
            {
                ProductID = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = unitPrice,
                Quantity = (short)quantity,
                Discount = discount,
                Total = total
            });

            // Update total amount
            UpdateTotalAmount();

            // Reset product selection fields
            cmbProduct.SelectedIndex = -1;
            txtUnitPrice.Text = string.Empty;
            txtQuantity.Text = "1";
            txtDiscount.Text = "0";
        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).Tag as OrderDetailViewModel;
            if (item != null)
            {
                _orderItems.Remove(item);
                UpdateTotalAmount();
            }
        }

        private void UpdateTotalAmount()
        {
            _totalOrderAmount = _orderItems.Sum(item => item.Total);
            txtTotalAmount.Text = _totalOrderAmount.ToString("C");
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCustomer.Focus();
                return;
            }

            if (cmbEmployee.SelectedItem == null)
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbEmployee.Focus();
                return;
            }

            if (dpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Please select an order date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpOrderDate.Focus();
                return;
            }

            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Please add at least one product to the order.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create order
            Order.CustomerId = (int)cmbCustomer.SelectedValue;
            Order.EmployeeId = (int)cmbEmployee.SelectedValue;
            Order.OrderDate = (DateTime)dpOrderDate.SelectedDate;

            // Convert order items to order details
            OrderDetails.Clear();
            foreach (var item in _orderItems)
            {
                OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductID,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Discount = (float)item.Discount
                });
            }

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        // View model for order details in the UI
        public class OrderDetailViewModel
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public decimal UnitPrice { get; set; }
            public short Quantity { get; set; }
            public decimal Discount { get; set; } // Store as decimal (0-1)
            public decimal Total { get; set; }
        }
    }
}

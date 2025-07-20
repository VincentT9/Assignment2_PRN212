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
    /// Interaction logic for CustomerOrderHistory.xaml
    /// </summary>
    public partial class CustomerOrderHistory : Page
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private List<OrderViewModel> _orders;

        public CustomerOrderHistory()
        {
            InitializeComponent();

            _orderRepository = App.ServiceProvider.GetService<IOrderRepository>();
            _orderDetailRepository = App.ServiceProvider.GetService<IOrderDetailRepository>();
            _customerRepository = App.ServiceProvider.GetService<ICustomerRepository>();
            _employeeRepository = App.ServiceProvider.GetService<IEmployeeRepository>();

            // Set default date range to last 3 months
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-3);
            dpEndDate.SelectedDate = DateTime.Now;

            LoadCustomerOrders();
        }

        private void LoadCustomerOrders()
        {
            try
            {
                // Get date range
                DateTime? startDate = dpStartDate.SelectedDate;
                DateTime? endDate = dpEndDate.SelectedDate?.AddDays(1).AddSeconds(-1); // End of the selected day

                // Get orders for current customer
                var orders = _orderRepository.GetOrdersByCustomerId(App.CurrentCustomer.CustomerId);

                // Ensure the orders are cast to the correct type
                var typedOrders = orders.Cast<Order>().ToList();

                // Filter by date range if provided
                if (startDate.HasValue && endDate.HasValue)
                {
                    typedOrders = typedOrders.Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).ToList();
                }

                // Calculate total for each order
                _orders = new List<OrderViewModel>();
                foreach (var order in typedOrders)
                {
                    var details = _orderDetailRepository.GetByOrderId(order.OrderId);
                    decimal totalAmount = details.Sum(d => (d.UnitPrice) * d.Quantity * (1 - (decimal)d.Discount));

                    _orders.Add(new OrderViewModel
                    {
                        OrderID = order.OrderId,
                        OrderDate = order.OrderDate,
                        TotalAmount = totalAmount,
                        Order = order
                    });
                }

                // Sort by most recent orders first
                _orders = _orders.OrderByDescending(o => o.OrderDate).ToList();
                dgOrders.ItemsSource = _orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Validate date range
            if (dpStartDate.SelectedDate != null && dpEndDate.SelectedDate != null &&
                dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Start date must be before or equal to end date.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoadCustomerOrders();
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Do something when an order is selected
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            var orderViewModel = (sender as Button).Tag as OrderViewModel;
            if (orderViewModel != null)
            {
                try
                {
                    var orderDetails = _orderDetailRepository.GetByOrderId(orderViewModel.OrderID);
                    var detailsDialog = new OrderDetailsDialog(orderViewModel.Order, orderDetails);
                    detailsDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading order details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnPlaceNewOrder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OrderDialog(_customerRepository, _employeeRepository, _orderDetailRepository);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // First add the order
                    _orderRepository.Add(dialog.Order);

                    // Then add the order details
                    foreach (var detail in dialog.OrderDetails)
                    {
                        detail.OrderId = dialog.Order.OrderId;
                        _orderDetailRepository.Add(detail);
                    }

                    LoadCustomerOrders();
                    MessageBox.Show("Order placed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error placing order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // View model for orders with calculated total
        public class OrderViewModel
        {
            public int OrderID { get; set; }
            public DateTime? OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public Order Order { get; set; }
        }
    }
}

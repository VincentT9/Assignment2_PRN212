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
    /// Interaction logic for OrderManagement.xaml
    /// </summary>
    public partial class OrderManagement : Page
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private Order _selectedOrder;

        public OrderManagement()
        {
            InitializeComponent();
            _orderRepository = App.ServiceProvider.GetService<IOrderRepository>();
            _orderDetailRepository = App.ServiceProvider.GetService<IOrderDetailRepository>();
            _customerRepository = App.ServiceProvider.GetService<ICustomerRepository>();
            _employeeRepository = App.ServiceProvider.GetService<IEmployeeRepository>();

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                var orders = _orderRepository.GetAll();
                dgOrders.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOrder = dgOrders.SelectedItem as Order;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadOrders();
                    return;
                }

                var searchResults = _orderRepository.SearchOrders(txtSearch.Text);
                dgOrders.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadOrders();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
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

                    LoadOrders();
                    MessageBox.Show("Order added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order to view details.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ShowOrderDetails(_selectedOrder);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            var order = (sender as Button).Tag as Order;
            ShowOrderDetails(order);
        }

        private void ShowOrderDetails(Order order)
        {
            try
            {
                var orderDetails = _orderDetailRepository.GetByOrderId(order.OrderId);
                var detailsDialog = new OrderDetailsDialog(order, orderDetails);
                detailsDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Please select an order to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete Order #{_selectedOrder.OrderId}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // First delete all related order details
                    _orderDetailRepository.Delete(_selectedOrder.OrderId);

                    // Then delete the order
                    _orderRepository.Delete(_selectedOrder.OrderId);

                    LoadOrders();
                    MessageBox.Show("Order deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

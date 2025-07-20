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
    /// Interaction logic for OrderDetailsDialog.xaml
    /// </summary>
    public partial class OrderDetailsDialog : Window
    {
        public OrderDetailsDialog(Order order, IEnumerable<OrderDetail> orderDetails)
        {
            InitializeComponent();

            // Set window title and header
            txtHeader.Text = $"Order Details - Order #{order.OrderId}";

            // Set order information
            txtCustomer.Text = order.Customer?.CompanyName;
            txtEmployee.Text = order.Employee?.Name;
            txtOrderDate.Text = order.OrderDate.ToString("MM/dd/yyyy");

            // Calculate total for each item and set data source
            var detailsWithTotal = orderDetails.Select(static d => new OrderDetailWithTotal
            {
                OrderId = d.OrderId,
                ProductId = d.ProductId,
                Product = d.Product,
                UnitPrice = d.UnitPrice,
                Quantity = d.Quantity,
                Discount = d.Discount,
                Total = (d.UnitPrice) * d.Quantity * (1 - (decimal)d.Discount)
            }).ToList();

            dgOrderDetails.ItemsSource = detailsWithTotal;

            // Calculate and display total order amount
            decimal totalAmount = detailsWithTotal.Sum(d => d.Total);
            txtTotalAmount.Text = totalAmount.ToString("C");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Helper class to display calculated total in the DataGrid
        public class OrderDetailWithTotal : OrderDetail
        {
            public decimal Total { get; set; }
        }
    }
}

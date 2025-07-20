using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Repositories;

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for SalesReports.xaml
    /// </summary>
    public partial class SalesReports : Page
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;

        private List<ReportItem> _currentReportData;

        public SalesReports()
        {
            InitializeComponent();

            _orderRepository = App.ServiceProvider.GetService<IOrderRepository>();
            _orderDetailRepository = App.ServiceProvider.GetService<IOrderDetailRepository>();
            _productRepository = App.ServiceProvider.GetService<IProductRepository>();
            _customerRepository = App.ServiceProvider.GetService<ICustomerRepository>();
            _employeeRepository = App.ServiceProvider.GetService<IEmployeeRepository>();

            // Set default date range to current month
            dpStartDate.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dpEndDate.SelectedDate = DateTime.Now;

            // Setup initial columns for sales by product
            SetupSalesByProductColumns();
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            // Validate date range
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a start and end date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Start date must be before or equal to end date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Determine which report to generate
                if (rbSalesByProduct.IsChecked == true)
                {
                    GenerateSalesByProductReport();
                }
                else if (rbSalesByCustomer.IsChecked == true)
                {
                    GenerateSalesByCustomerReport();
                }
                else if (rbSalesByEmployee.IsChecked == true)
                {
                    GenerateSalesByEmployeeReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Fix for the CS1503 error: Modify the method call to handle a list of order IDs correctly.
        private void GenerateSalesByProductReport()
        {
            // Setup columns
            SetupSalesByProductColumns();

            // Get orders in the date range
            var startDate = dpStartDate.SelectedDate.Value;
            var endDate = dpEndDate.SelectedDate.Value.AddDays(1).AddSeconds(-1); // End of the selected day

            var orders = _orderRepository.GetByDateRange(startDate, endDate);
            var orderIds = orders.Select(o => o.OrderId).ToList();

            // Fix: Iterate through the list of order IDs and retrieve order details for each ID
            var orderDetails = orderIds
                .SelectMany(orderId => _orderDetailRepository.GetByOrderId(orderId))
                .ToList();

            // Group and calculate sales by product
            var salesByProduct = orderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new ReportItem
                {
                    Id = g.Key,
                    Name = g.First().Product?.ProductName ?? "Unknown Product",
                    Quantity = (short)g.Sum(od => od.Quantity),
                    TotalSales = g.Sum(od => (od.UnitPrice) * od.Quantity * (1 - (decimal)od.Discount))
                })
                .OrderByDescending(x => x.TotalSales)
                .ToList();

            _currentReportData = salesByProduct;
            dgReport.ItemsSource = salesByProduct;
        }

        private void GenerateSalesByCustomerReport()
        {
            // Setup columns
            SetupSalesByCustomerColumns();

            // Get orders in the date range
            var startDate = dpStartDate.SelectedDate.Value;
            var endDate = dpEndDate.SelectedDate.Value.AddDays(1).AddSeconds(-1); // End of the selected day

            var orders = _orderRepository.GetByDateRange(startDate, endDate);

            // Calculate total sales for each order
            var orderTotals = new Dictionary<int, decimal>();
            foreach (var order in orders)
            {
                var details = _orderDetailRepository.GetByOrderId(order.OrderId);
                decimal total = details.Sum(d => (d.UnitPrice) * d.Quantity * (1 - (decimal)d.Discount));
                orderTotals[order.OrderId] = total;
            }

            // Group orders by customer
            var salesByCustomer = orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new ReportItem
                {
                    Id = g.Key,
                    Name = g.First().Customer?.CompanyName ?? "Unknown Customer",
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => orderTotals.ContainsKey(o.OrderId) ? orderTotals[o.OrderId] : 0)
                })
                .OrderByDescending(x => x.TotalSales)
                .ToList();

            _currentReportData = salesByCustomer;
            dgReport.ItemsSource = salesByCustomer;
        }

        private void GenerateSalesByEmployeeReport()
        {
            // Setup columns
            SetupSalesByEmployeeColumns();

            // Get orders in the date range
            var startDate = dpStartDate.SelectedDate.Value;
            var endDate = dpEndDate.SelectedDate.Value.AddDays(1).AddSeconds(-1); // End of the selected day

            var orders = _orderRepository.GetByDateRange(startDate, endDate);

            // Calculate total sales for each order
            var orderTotals = new Dictionary<int, decimal>();
            foreach (var order in orders)
            {
                var details = _orderDetailRepository.GetByOrderId(order.OrderId);
                decimal total = details.Sum(d => (d.UnitPrice) * d.Quantity * (1 - (decimal)d.Discount));
                orderTotals[order.OrderId] = total;
            }

            // Group orders by employee
            var salesByEmployee = orders
                .GroupBy(o => o.EmployeeId)
                .Select(g => new ReportItem
                {
                    Id = g.Key,
                    Name = g.First().Employee?.Name ?? "Unknown Employee",
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => orderTotals.ContainsKey(o.OrderId) ? orderTotals[o.OrderId] : 0)
                })
                .OrderByDescending(x => x.TotalSales)
                .ToList();

            _currentReportData = salesByEmployee;
            dgReport.ItemsSource = salesByEmployee;
        }

        private void SetupSalesByProductColumns()
        {
            dgReport.Columns.Clear();

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Product ID",
                Binding = new Binding("Id"),
                Width = 100
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Product Name",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Quantity Sold",
                Binding = new Binding("Quantity"),
                Width = 120
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Total Sales",
                Binding = new Binding("TotalSales") { StringFormat = "{0:C}" },
                Width = 120
            });
        }

        private void SetupSalesByCustomerColumns()
        {
            dgReport.Columns.Clear();

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Customer ID",
                Binding = new Binding("Id"),
                Width = 100
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Customer Name",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Order Count",
                Binding = new Binding("OrderCount"),
                Width = 120
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Total Sales",
                Binding = new Binding("TotalSales") { StringFormat = "{0:C}" },
                Width = 120
            });
        }

        private void SetupSalesByEmployeeColumns()
        {
            dgReport.Columns.Clear();

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Employee ID",
                Binding = new Binding("Id"),
                Width = 100
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Employee Name",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Order Count",
                Binding = new Binding("OrderCount"),
                Width = 120
            });

            dgReport.Columns.Add(new DataGridTextColumn
            {
                Header = "Total Sales",
                Binding = new Binding("TotalSales") { StringFormat = "{0:C}" },
                Width = 120
            });
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (_currentReportData == null || !_currentReportData.Any())
            {
                MessageBox.Show("No report data to export.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Create save file dialog
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    DefaultExt = "csv",
                    Title = "Save Report As"
                };

                // Get report title based on selected report type
                string reportTitle = "Sales Report";
                if (rbSalesByProduct.IsChecked == true)
                    reportTitle = "Sales by Product";
                else if (rbSalesByCustomer.IsChecked == true)
                    reportTitle = "Sales by Customer";
                else if (rbSalesByEmployee.IsChecked == true)
                    reportTitle = "Sales by Employee";

                saveFileDialog.FileName = $"{reportTitle} {dpStartDate.SelectedDate:yyyy-MM-dd} to {dpEndDate.SelectedDate:yyyy-MM-dd}";

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Build CSV content
                    StringBuilder csv = new StringBuilder();

                    // Add header row based on report type
                    if (rbSalesByProduct.IsChecked == true)
                    {
                        csv.AppendLine("Product ID,Product Name,Quantity Sold,Total Sales");

                        foreach (var item in _currentReportData)
                        {
                            csv.AppendLine($"{item.Id},\"{item.Name.Replace("\"", "\"\"")}\",{item.Quantity},{item.TotalSales}");
                        }
                    }
                    else if (rbSalesByCustomer.IsChecked == true)
                    {
                        csv.AppendLine("Customer ID,Customer Name,Order Count,Total Sales");

                        foreach (var item in _currentReportData)
                        {
                            csv.AppendLine($"{item.Id},\"{item.Name.Replace("\"", "\"\"")}\",{item.OrderCount},{item.TotalSales}");
                        }
                    }
                    else if (rbSalesByEmployee.IsChecked == true)
                    {
                        csv.AppendLine("Employee ID,Employee Name,Order Count,Total Sales");

                        foreach (var item in _currentReportData)
                        {
                            csv.AppendLine($"{item.Id},\"{item.Name.Replace("\"", "\"\"")}\",{item.OrderCount},{item.TotalSales}");
                        }
                    }

                    // Save to file
                    File.WriteAllText(saveFileDialog.FileName, csv.ToString());

                    MessageBox.Show("Report exported successfully.", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Report item class for data binding
        private class ReportItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public short Quantity { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalSales { get; set; }
        }
    }
}

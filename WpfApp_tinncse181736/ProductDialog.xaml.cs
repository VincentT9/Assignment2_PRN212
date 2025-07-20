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
using Repositories;

namespace WpfApp_tinncse181736
{
    /// <summary>
    /// Interaction logic for ProductDialog.xaml
    /// </summary>
    public partial class ProductDialog : Window
    {
        private readonly ICategoryRepository _categoryRepository;
        public Product Product { get; set; }

        public ProductDialog(ICategoryRepository categoryRepository)
        {
            InitializeComponent();
            _categoryRepository = categoryRepository;
            Product = new Product();

            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryRepository.GetAll();
                cmbCategory.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // If this is an edit operation, populate fields
            if (Product.ProductId != 0)
            {
                txtProductName.Text = Product.ProductName;
                cmbCategory.SelectedValue = Product.CategoryId;
                txtQuantityPerUnit.Text = Product.QuantityPerUnit;
                txtUnitPrice.Text = Product.UnitPrice?.ToString();
                txtUnitsInStock.Text = Product.UnitsInStock?.ToString();
                txtUnitsOnOrder.Text = Product.UnitsOnOrder?.ToString();
                txtReorderLevel.Text = Product.ReorderLevel?.ToString();
                chkDiscontinued.IsChecked = Product.Discontinued;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Product Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProductName.Focus();
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Category is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUnitPrice.Text) || !decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice))
            {
                MessageBox.Show("Unit Price must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUnitPrice.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUnitsInStock.Text) || !short.TryParse(txtUnitsInStock.Text, out short unitsInStock))
            {
                MessageBox.Show("Units In Stock must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUnitsInStock.Focus();
                return;
            }

            short unitsOnOrder = 0;
            if (!string.IsNullOrWhiteSpace(txtUnitsOnOrder.Text) && !short.TryParse(txtUnitsOnOrder.Text, out unitsOnOrder))
            {
                MessageBox.Show("Units On Order must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUnitsOnOrder.Focus();
                return;
            }

            short reorderLevel = 0;
            if (!string.IsNullOrWhiteSpace(txtReorderLevel.Text) && !short.TryParse(txtReorderLevel.Text, out reorderLevel))
            {
                MessageBox.Show("Reorder Level must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtReorderLevel.Focus();
                return;
            }

            // Update product object with form values
            Product.ProductName = txtProductName.Text.Trim();
            Product.CategoryId = (int)cmbCategory.SelectedValue;
            Product.QuantityPerUnit = txtQuantityPerUnit.Text?.Trim();
            Product.UnitPrice = unitPrice;
            Product.UnitsInStock = unitsInStock;
            Product.UnitsOnOrder = unitsOnOrder;
            Product.ReorderLevel = reorderLevel;
            Product.Discontinued = chkDiscontinued.IsChecked ?? false;

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

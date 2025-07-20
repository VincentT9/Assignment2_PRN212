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
    /// Interaction logic for ProductManagement.xaml
    /// </summary>
    public partial class ProductManagement : Page
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private Product _selectedProduct;

        public ProductManagement()
        {
            InitializeComponent();
            _productRepository = App.ServiceProvider.GetService<IProductRepository>();
            _categoryRepository = App.ServiceProvider.GetService<ICategoryRepository>();

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productRepository.GetAll();
                dgProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = dgProducts.SelectedItem as Product;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    LoadProducts();
                    return;
                }

                var searchResults = _productRepository.SearchProducts(txtSearch.Text);
                dgProducts.ItemsSource = searchResults;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadProducts();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductDialog(_categoryRepository);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _productRepository.Add(dialog.Product);
                    LoadProducts();
                    MessageBox.Show("Product added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product to edit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new ProductDialog(_categoryRepository);
            dialog.Product = new Product
            {
                ProductId = _selectedProduct.ProductId,
                ProductName = _selectedProduct.ProductName,
                CategoryId = _selectedProduct.CategoryId,
                QuantityPerUnit = _selectedProduct.QuantityPerUnit,
                UnitPrice = _selectedProduct.UnitPrice,
                UnitsInStock = _selectedProduct.UnitsInStock,
                UnitsOnOrder = _selectedProduct.UnitsOnOrder,
                ReorderLevel = _selectedProduct.ReorderLevel,
                Discontinued = _selectedProduct.Discontinued
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    _productRepository.Update(dialog.Product);
                    LoadProducts();
                    MessageBox.Show("Product updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete the product '{_selectedProduct.ProductName}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _productRepository.Delete(_selectedProduct.ProductId);
                    LoadProducts();
                    MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

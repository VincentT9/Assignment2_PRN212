using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly LucySalesDataContext _context;

        public ProductRepository(LucySalesDataContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = GetById(id);
            if (product == null) return;

            // Kiểm tra sản phẩm đã được đặt hàng chưa
            bool isReferenced = _context.OrderDetails.Any(od => od.ProductId == id);
            if (isReferenced)
            {
                throw new InvalidOperationException("Không thể xóa sản phẩm vì đã tồn tại trong đơn hàng.");
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products
                    .Where(p => !p.Discontinued)
                    .OrderBy(p => p.ProductName)
                    .ToList();
        }

        public IEnumerable<Product> GetByCategory(int categoryId)
        {
            return _context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .OrderBy(p => p.ProductName)
                    .ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products
                    .FirstOrDefault(p => p.ProductId == id);
        }

        public IEnumerable<Product> SearchByName(string keyword)
        {
            return _context.Products
                    .Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase) && !p.Discontinued)
                    .OrderBy(p => p.ProductName)
                    .ToList();
        }

        public IEnumerable SearchProducts(string text)
        {
            return _context.Products
                .Where(p => p.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase) && !p.Discontinued)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        public void Update(Product product)
        {
            _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}

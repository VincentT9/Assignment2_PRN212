using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService()
        {
        }

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Add(Product product)
        {
            _productRepository.Add(product);
        }

        public void Delete(int id)
        {
            _productRepository.Delete(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public IEnumerable<Product> GetByCategory(int categoryId)
        {
            return _productRepository.GetByCategory(categoryId);
        }

        public Product? GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public IEnumerable<Product> SearchByName(string keyword)
        {
            return _productRepository.SearchByName(keyword);
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }
    }
}

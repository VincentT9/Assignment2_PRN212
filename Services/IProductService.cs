using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
        IEnumerable<Product> SearchByName(string keyword);
        IEnumerable<Product> GetByCategory(int categoryId);
        IEnumerable SearchProducts(string text);
    }
}

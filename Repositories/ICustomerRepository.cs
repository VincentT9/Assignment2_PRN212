using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll();
        Customer? GetById(int id);
        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(int id);
        IEnumerable SearchCustomers(string text);
        Customer GetCustomerById(int customerId);
        Customer GetCustomerByPhone(string text);
    }
}

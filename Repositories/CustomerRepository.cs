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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly LucySalesDataContext _context;
        public CustomerRepository(LucySalesDataContext context)
        {
            _context = context;
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var customer = GetById(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public Customer? GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == id);
        }

        public Customer GetCustomerById(int customerId)
        {
            return _context.Customers
                .FirstOrDefault(c => c.CustomerId == customerId) ?? throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
        }

        public Customer GetCustomerByPhone(string text)
        {
            return _context.Customers
                .FirstOrDefault(c => c.Phone == text) ?? throw new KeyNotFoundException($"Customer with phone {text} not found.");
        }

        public IEnumerable SearchCustomers(string text)
        {
            return _context.Customers
                .Where(c => c.ContactName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                            c.CompanyName.Contains(text, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void Update(Customer customer)
        {
            var existingCustomer = GetById(customer.CustomerId);
            if (existingCustomer != null)
            {
                _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
                _context.SaveChanges();
            }
        }
    }
}

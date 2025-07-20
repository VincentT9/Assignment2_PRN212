using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService()
        {
        }

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public void Add(Customer customer)
        {
            _customerRepository.Add(customer);
        }

        public void Delete(int id)
        {
            _customerRepository.Delete(id); 
        }

        public IEnumerable<Customer> GetAll()
        {
            return _customerRepository.GetAll();
        }

        public Customer? GetById(int id)
        {
            return _customerRepository.GetById(id);
        }

        public Customer GetCustomerById(int customerId)
        {
            return _customerRepository.GetCustomerById(customerId);
        }

        public Customer GetCustomerByPhone(string text)
        {
            return _customerRepository.GetCustomerByPhone(text);
        }

        public IEnumerable SearchCustomers(string text)
        {
            return _customerRepository.SearchCustomers(text);
        }

        public void Update(Customer customer)
        {
            _customerRepository.Update(customer);
        }
    }
}

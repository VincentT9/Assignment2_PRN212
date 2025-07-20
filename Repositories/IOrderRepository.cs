using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order? GetById(int id);
        void Add(Order order);
        void Update(Order order);
        void Delete(int id);
        IEnumerable<Order> GetByDateRange(DateTime start, DateTime end);
        IEnumerable SearchOrders(string text);
        IEnumerable<object> GetOrdersByCustomerId(int customerId);
    }
}

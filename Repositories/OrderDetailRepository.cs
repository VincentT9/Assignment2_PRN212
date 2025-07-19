using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly LucySalesDataContext _context;
        public OrderDetailRepository(LucySalesDataContext context)
        {
            _context = context;
        }

        public void Add(OrderDetail detail)
        {
            _context.OrderDetails.Add(detail);
            _context.SaveChanges();
        }

        public void Delete(int orderId, int productId)
        {
            var od = _context.OrderDetails
                .FirstOrDefault(od => od.OrderId == orderId && od.ProductId == productId);
            if (od != null)
            {
                _context.OrderDetails.Remove(od);
                _context.SaveChanges();
            }
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            var list = _context.OrderDetails.ToList();

            foreach (var item in list)
            {
                Console.WriteLine($"DEBUG: OrderID = {item.OrderId}, ProductID = {item.ProductId}, Qty = {item.Quantity}");
            }

            return list;
        }

        public IEnumerable<OrderDetail> GetByOrderId(int orderId)
        {
            return _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class OrderService : IOderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderDetailRepository _detailRepo;

        public OrderService()
        {
        }

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepo = orderRepository;
        }

        public IEnumerable<Order> GetAll() => _orderRepo.GetAll();
        public IEnumerable<Order> GetByDateRange(DateTime start, DateTime end) => _orderRepo.GetByDateRange(start, end);
        public Order? GetById(int id) => _orderRepo.GetById(id);
        public void Add(Order order)
        {
            // Tạo OrderID mới vì OrderID không phải Identity
            var maxId = _orderRepo.GetAll().Any() ? _orderRepo.GetAll().Max(o => o.OrderId) : 0;
            order.OrderId = maxId + 1;

            // Tạo bản ghi Order không chứa OrderDetails để lưu
            var baseOrder = new Order
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                EmployeeId = order.EmployeeId
            };

            _orderRepo.Add(baseOrder); // Gọi đúng repository

            // Lưu chi tiết
            foreach (var detail in order.OrderDetails)
            {
                detail.OrderId = order.OrderId;
                _detailRepo.Add(detail);
            }
        }

        public void Update(Order order) => _orderRepo.Update(order);
        public void Delete(int id) => _orderRepo.Delete(id);


        public IEnumerable<OrderDetail> GetDetails(int orderId)
        {
            var details = _detailRepo.GetAll().Where(d => d.OrderId == orderId).ToList();
            Console.WriteLine($"OrderService: {details.Count} bản ghi từ repo");
            return details;
        }
        public void AddDetail(OrderDetail detail) => _detailRepo.Add(detail);
        public void DeleteDetail(int orderId, int productId) => _detailRepo.Delete(orderId, productId);
    }
}

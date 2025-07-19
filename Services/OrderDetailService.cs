using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService()
        {
        }

        public OrderDetailService(IOrderDetailRepository orderDetailRepository)
        {
            _orderDetailRepository = orderDetailRepository;
        }

        public void Add(OrderDetail detail)
        {
            _orderDetailRepository.Add(detail);    
        }

        public void Delete(int orderId, int productId)
        {
            _orderDetailRepository.Delete(orderId, productId);
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            return _orderDetailRepository.GetAll();
        }

        public IEnumerable<OrderDetail> GetByOrderId(int orderId)
        {
            return _orderDetailRepository.GetByOrderId(orderId);
        }
    }
}

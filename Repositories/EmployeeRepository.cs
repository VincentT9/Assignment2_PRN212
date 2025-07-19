using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LucySalesDataContext _context;

        public EmployeeRepository(LucySalesDataContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees.ToList();
        }

        public Employee? GetById(int id)
        {
            return _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
        }

        public Employee? Login(string username, string password)
        {
            return _context.Employees
                .FirstOrDefault(e => e.UserName == username && e.Password == password);
        }
    }
}

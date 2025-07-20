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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly LucySalesDataContext _context;

        public EmployeeRepository(LucySalesDataContext context)
        {
            _context = context;
        }

        public void Add(object employee)
        {
            _context.Employees.Add((Employee)employee);
        }

        public void Delete(int employeeId)
        {
            _context.Employees.Remove(_context.Employees.FirstOrDefault(e => e.EmployeeId == employeeId));
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

        public IEnumerable SearchEmployees(string text)
        {
            return _context.Employees
                .Where(e => e.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                            e.UserName.Contains(text, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void Update(object employee)
        {
            _context.Employees.Update((Employee)employee);
        }
    }
}

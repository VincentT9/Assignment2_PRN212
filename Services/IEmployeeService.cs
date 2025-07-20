using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IEmployeeService
    {
        void Add(object employee);
        void Delete(int employeeId);
        IEnumerable<Employee> GetAll();
        Employee? GetById(int id);
        Employee? Login(string username, string password);
        IEnumerable SearchEmployees(string text);
        void Update(object employee);
    }
}

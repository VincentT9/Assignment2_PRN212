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
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService()
        {
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public void Add(object employee)
        {
            _employeeRepository.Add(employee);
        }

        public void Delete(int employeeId)
        {
            _employeeRepository.Delete(employeeId);
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employeeRepository.GetAll();
        }

        public Employee? GetById(int id)
        {
            return _employeeRepository.GetById(id);
        }

        public Employee? Login(string username, string password)
        {
            return _employeeRepository.Login(username, password);
        }

        public IEnumerable SearchEmployees(string text)
        {
            return _employeeRepository.SearchEmployees(text);
        }

        public void Update(object employee)
        {
            _employeeRepository.Update(employee);
        }
    }
}

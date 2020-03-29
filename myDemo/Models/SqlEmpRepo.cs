using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Models
{
    public class SqlEmpRepo : IEmpRepo
    {
        private readonly AppDbContext context;

        public SqlEmpRepo(AppDbContext db)
        {
            this.context = db;
        }

        public Employee AddEmp(Employee employee)
        {
            context.Employee.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            Employee employee = context.Employee.Find(id);
            if (employee != null)
            {
                context.Employee.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }

        public Employee GetEmpById(int Id)
        {
            return context.Employee.Find(Id);

        }

        public IEnumerable<Employee> GetEmpList()
        {
            return context.Employee;
        }

        public Employee Update(Employee employeeChanges)
        {
            var employee = context.Employee.Attach(employeeChanges);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
        }
    }
}

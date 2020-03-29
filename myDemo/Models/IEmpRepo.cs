using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Models
{
     public interface IEmpRepo
    {
       

        Employee GetEmpById(int Id);
        IEnumerable<Employee> GetEmpList();
        Employee AddEmp(Employee employee);
        Employee Update(Employee employeeChanges);
        Employee Delete(int id);
    }
}

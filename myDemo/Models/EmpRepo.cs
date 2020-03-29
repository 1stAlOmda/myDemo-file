using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Models
{
    public class EmpRepo : IEmpRepo
    {
        List<Employee> EmpList;

        public EmpRepo()
        {
             EmpList = new List<Employee>
             {
                 new Employee { Id = 1 , Name = "Ahmed", Email="EngAhmed@gmail.com",Department = Dep.IT },
                 new Employee { Id = 2 , Name = "Moh", Email="EngMoh@gmail.com",Department = Dep.HR},
                 new Employee { Id = 3 , Name = "Hesham", Email="EngHesham@gmail.com",Department = Dep.IT},
                 new Employee { Id = 4 , Name = "Waled", Email="EngAhmed@gmail.com",Department = Dep.HR},

             };
        }

        public Employee AddEmp(Employee EmpObj)
        {
           EmpObj.Id= EmpList.Max(x => x.Id)+1;
            EmpList.Add(EmpObj);
            return EmpObj;

                }

        public Employee Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Employee GetEmpById(int EmpId)
        {
            return EmpList.FirstOrDefault(x => x.Id == EmpId);
        }

        public Employee Update(Employee employeeChanges)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Employee> GetEmpList()
        {
            return EmpList.ToList();
        }
    }
}

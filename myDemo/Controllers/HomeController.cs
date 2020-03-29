using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myDemo.Models;
using myDemo.ViewModels;
using ORMEFCoreDA.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace myDemo.Controllers
{
     public class HomeController : Controller
    {
        private readonly IEmpRepo empRepo;
        private readonly IHostingEnvironment HostEnv;

        public HomeController(IEmpRepo _empRepo, IHostingEnvironment _HostEnv)
        {
            empRepo = _empRepo;
            HostEnv = _HostEnv;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            IEnumerable<Employee> emp = empRepo.GetEmpList();
            return View("Views/Home/Index.cshtml", emp);
        }

        public ViewResult Details(int id)
        {
            //throw new Exception("Error In Detail View");
            Employee emp = empRepo.GetEmpById(id);

            if (emp == null)
            {
                Response.StatusCode = 404;
                return View("EmpNotFound", id);
            }
            return View("Views/Home/EmpDetails.cshtml", emp);
        }

        public ViewResult Create()
        {
            return View("../Home/View");
        }

        [HttpPost]
        public IActionResult Create(EmpVM EmpObj)
        {
            if (ModelState.IsValid)
            {
                string ImgUniqeName = "";
                if (EmpObj.Photo != null )
                {
                        string fileRootPath = Path.Combine(HostEnv.WebRootPath, "img");
                        ImgUniqeName = Guid.NewGuid().ToString() + "_" + EmpObj.Photo.FileName;
                        string filePath = Path.Combine(fileRootPath, ImgUniqeName);
                    EmpObj.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                   
                }
                Employee savedEmp = new Employee
                {
                    Name = EmpObj.Name,
                    Department = EmpObj.Department,
                    Email= EmpObj.Email,
                    PhotoName = ImgUniqeName
                };
                empRepo.AddEmp(savedEmp);
                return RedirectToAction("Details", new { id = savedEmp.Id });
            }
            return View("Views/Home/View.cshtml");
        }

        public ViewResult Edit(int id)
        {
            var emp = empRepo.GetEmpById(id);
            EmpEditVM emp4Edit = new EmpEditVM
            {
                Id = emp.Id,
                Name = emp.Name,
                Department = emp.Department,
                Email = emp.Email,
                ExistingPhoto = emp.PhotoName
            };
            return View(emp4Edit);
        }

        [HttpPost]
        public IActionResult Edit(EmpEditVM EmpObj)
        {
            if (ModelState.IsValid)
            {
               var EditedEmp = empRepo.GetEmpById(EmpObj.Id);
                EditedEmp.Name = EmpObj.Name;
                EditedEmp.Email = EmpObj.Email;
                EditedEmp.Department = EmpObj.Department;
                if (EmpObj.Photo != null)
                {
                    DelteItemFromRootPath(EmpObj.ExistingPhoto);
                    EditedEmp.PhotoName = SetupImg(EmpObj); ;
                }
                empRepo.Update(EditedEmp);
                return RedirectToAction("Details", new { id = EditedEmp.Id });
            }
            return View("Views/Home/View.cshtml");
        }

        private void DelteItemFromRootPath(string existingPhoto)
        {
            if (existingPhoto !=null)
            {
                string ItemPath = Path.Combine(HostEnv.WebRootPath, "img", existingPhoto);
                System.IO.File.Delete(ItemPath);
            }
          
        }

        private string SetupImg(EmpEditVM EmpObj)
        {
            string ImgUniqeName = "";
            if (EmpObj.Photo != null)
            {
                string fileRootPath = Path.Combine(HostEnv.WebRootPath, "img");
                ImgUniqeName = Guid.NewGuid().ToString() + "_" + EmpObj.Photo.FileName;
                string filePath = Path.Combine(fileRootPath, ImgUniqeName);
                using (FileStream PhotoStream = new FileStream(filePath, FileMode.Create))
                {
                    EmpObj.Photo.CopyTo(PhotoStream);
                }
            }

            return ImgUniqeName;
        }




        /// <summary>
        /// //
        /// </summary>
        /// <returns></returns>

        public ViewResult testAbsPath() => View("/Views/Home/Index.cshtml");

        public ViewResult testAbsPath2()=> View("~/MyViews/Index.cshtml");
        
        public ViewResult testRelPath() => View("../Home/Index");
        
        public string testRelPath2()
        {
            return "defualt route for Home Controller";
        }

        public ViewResult LooslyTypedViewTest()
        {
            ViewBag.title = "viewDataViewBagTest page";
            ViewData["title"] = "viewDataViewBagTest page";
            ViewData["test"] = empRepo.GetEmpById(2);
            ViewBag.test = empRepo.GetEmpById(2);
            return View("/MyViews/Index.cshtml");
        }

        public ViewResult StronglyTypedViewTest(int? id)
        {

            Employee emp = empRepo.GetEmpById(id ?? 1);
            return View("Views/Home/Index.cshtml", emp);
        }

        public ViewResult ListView()
        {

            IEnumerable<Employee> emp = empRepo.GetEmpList();
            return View("Views/Home/EmpList.cshtml", emp);
        }
    }
}

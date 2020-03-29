using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace myDemo.Controllers
{
    public class DefaultController : Controller
    {
        public string Index()
        {
            return "DefaultController/index";
        }

        public string Details()
        {
            return "DefaultController/Details";
        }
    }
}
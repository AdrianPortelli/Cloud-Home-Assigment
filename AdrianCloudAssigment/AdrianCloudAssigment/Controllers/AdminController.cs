using Common;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdrianCloudAssigment.Controllers
{
    public class AdminController : Controller
    {
        private ICacheRepository cacheRepo;

        public AdminController(ICacheRepository _cacheRepo)
        {
            cacheRepo = _cacheRepo;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(MenuItem menuItem  )
        {
            cacheRepo.AddMenu(menuItem);
            return View();
        }


        [HttpGet]
        public IActionResult Update()
        {
            return View(cacheRepo.GetMenus());
        }

        [HttpPost]
        public IActionResult Update(List<MenuItem> menuItems)
        {
            cacheRepo.UpdateMenus(menuItems);
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            return View(cacheRepo.GetMenus());
        }
    }
}

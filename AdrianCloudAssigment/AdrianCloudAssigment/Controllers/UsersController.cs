using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdrianCloudAssigment.Controllers
{
    public class UsersController : Controller
    {
        private IFireStoreDataAccess fireStore;

        public UsersController(IFireStoreDataAccess _firestore)
        {
            fireStore = _firestore;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var myUser = await fireStore.GetUser(User.Claims.ElementAt(4).Value);

            if(myUser == null)
            {
                myUser = new Common.User();
                myUser.Email = User.Claims.ElementAt(4).Value;
            }

            return View(myUser);
        }
    }
}

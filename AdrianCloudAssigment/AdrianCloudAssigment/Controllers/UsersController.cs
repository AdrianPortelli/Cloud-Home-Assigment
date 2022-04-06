using Common;
using DataAccess.Interfaces;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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

        public IActionResult Register(User user)
        {
            user.Email = User.Claims.ElementAt(4).Value;
            user.Credit = 0;

            fireStore.AddUser(user);
            return RedirectToAction("Index");

        }
        [HttpGet]
        [Authorize]
        public IActionResult upload()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public  IActionResult upload(Common.File file, IFormFile fileupload)
        {
            string bucketName = "cloudhomeassigmentbucket";
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileupload.FileName);
            var storage = StorageClient.Create();


            using(Stream fsIN = fileupload.OpenReadStream())
            {
                storage.UploadObject(bucketName, fileName, null,fsIN);
            }

            file.Id = fileName;
            file.FileName = fileupload.FileName;
            file.FileLink = $"https://storage.googleapis.com/{bucketName}/{fileName}";
            fireStore.UploadFile(User.Claims.ElementAt(4).Value, file);

            return RedirectToAction("List");
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var files = await fireStore.GetFiles(User.Claims.ElementAt(4).Value);
            return View(files);
        }
    }
}

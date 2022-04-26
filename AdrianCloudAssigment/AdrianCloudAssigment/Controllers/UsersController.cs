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
        private ICacheRepository cacheRepo;
        private IPubSubRepository pubSubRepo;

        public UsersController(IFireStoreDataAccess _firestore, ICacheRepository _cacheRepo, IPubSubRepository _pubSubRepo)
        {
            pubSubRepo = _pubSubRepo;
            fireStore = _firestore;
            cacheRepo = _cacheRepo;
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


   /*         using(Stream fsIN = fileupload.OpenReadStream())
            {
               
            }*/

            using ( var ms = new MemoryStream())
            {
                fileupload.CopyTo(ms);
                Byte[] fileBytes = ms.ToArray();

                storage.UploadObject(bucketName, fileName, null, ms);

                string s = Convert.ToBase64String(fileBytes);
                file.fileBase64 = s;
            }

            file.Id = fileName;
            file.FileName = fileupload.FileName;
            file.FileLink = $"https://storage.googleapis.com/{bucketName}/{fileName}";
            file.FileOwnerEmail = User.Claims.ElementAt(4).Value;



            fireStore.UploadFile(User.Claims.ElementAt(4).Value, file);

            return RedirectToAction("List");
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var files = await fireStore.GetFiles(User.Claims.ElementAt(4).Value);
            return View(files);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddCredit()
        {
            var menuitem = cacheRepo.GetMenus();
            return View(menuitem);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCredit(int value,int price)
        {
            if (price >= value) {

                User user =  await fireStore.GetUser(User.Claims.ElementAt(4).Value);

                int newCredit = user.Credit + value;

                await fireStore.UpdateUserCredit(User.Claims.ElementAt(4).Value, newCredit);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> ConvertToPDF(string fileId)
        {
            if(reduceUserCredit().Result == false)
                return RedirectToAction("List");

           Common.File fileInfo = await fireStore.GetFile(User.Claims.ElementAt(4).Value, fileId);
           await pubSubRepo.Publish(fileInfo);

            return RedirectToAction("List");
        }

        public async Task<bool> reduceUserCredit()
        {

            User user = await fireStore.GetUser(User.Claims.ElementAt(4).Value);

            if(user.Credit <= 0)
                return false;

            await fireStore.UpdateUserCredit(User.Claims.ElementAt(4).Value, user.Credit - 1);

            return true;

        }
    }
}

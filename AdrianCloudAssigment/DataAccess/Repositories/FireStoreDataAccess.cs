using Common;
using DataAccess.Interfaces;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class FireStoreDataAccess : IFireStoreDataAccess
    {
        private FirestoreDb db { get; set; }
        public FireStoreDataAccess(string project)
        {
            db = FirestoreDb.Create(project);
        }

        public async void AddUser(User user)
        {
            DocumentReference docRef = db.Collection("users").Document(user.Email);
            await docRef.SetAsync(user);
        }

        public void DeleteUser(string email)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserCredit(User user)
        {
            throw new NotImplementedException();
        }

        public void UploadFile(string email, File file)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUser(string email)
        {
            DocumentReference docRef = db.Collection("users").Document(email);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
               User myUser =  snapshot.ConvertTo<User>();
               return myUser;

            }
            else
            {
                return null;
               
            }
        }
    }
}

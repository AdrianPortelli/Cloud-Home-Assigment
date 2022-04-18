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

        public async Task<WriteResult> UpdateUserCredit(string email, int value)
        {
            DocumentReference docRef = db.Collection("users").Document(email);
            return await docRef.UpdateAsync("Credit", value);
        }





        public async Task<WriteResult> UploadFile(string email, File file)
        {
            DocumentReference docRef = db.Collection("users").Document(email).Collection("files").Document(file.Id);

            return await docRef.SetAsync(file);
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

        public async Task<List<File>> GetFiles(string email)
        {

            if ((await GetUser(email)) == null) return new List<File>();

            Query fileQuery = db.Collection("users").Document(email).Collection("files");
            QuerySnapshot fileQuerySnapshot = await fileQuery.GetSnapshotAsync();

            List<File> files = new List<File>();

            foreach(DocumentSnapshot documentSnapshot in fileQuerySnapshot)
            {
                files.Add(documentSnapshot.ConvertTo<File>());
            }

            return files;

        }

        public async Task<Common.File> GetFile(string email, string fileID)
        {
            DocumentReference docRef = db.Collection("users").Document(email).Collection("files").Document(fileID);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                Common.File myFile = snapshot.ConvertTo<Common.File>();
                return myFile;

            }
            else
            {
                return null;

            }
        }
    }
}

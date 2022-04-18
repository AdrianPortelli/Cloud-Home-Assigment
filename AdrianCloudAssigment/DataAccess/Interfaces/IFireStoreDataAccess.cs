using Common;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IFireStoreDataAccess
    {
        Task<User> GetUser(string email);
        void AddUser(User user);

        Task<WriteResult>  UpdateUserCredit(string email, int value);

        Task<WriteResult> UploadFile(string email, File file);

        void DeleteUser(string email);

        Task<List<File>> GetFiles(string email);

        Task<Common.File> GetFile(string email, string fileID);

    }
}

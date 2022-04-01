using Common;
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

        void UpdateUserCredit(User user);

        void UploadFile(string email, File file);

        void DeleteUser(string email);

    }
}

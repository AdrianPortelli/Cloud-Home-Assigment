using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public int Credit { get; set; }


    }

    [FirestoreData]
    public class File
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string FileName { get; set; }

        [FirestoreProperty]
        public string FileLink { get; set; }

        [FirestoreProperty]

        public string fileBase64 { get; set; }

        [FirestoreProperty]
        public string ConvertedFileLink { get; set; }

        [FirestoreProperty]
        public string FileOwnerEmail { get; set; }

    }

}

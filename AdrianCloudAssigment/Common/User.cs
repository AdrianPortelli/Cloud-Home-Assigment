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

    //missing file 
    //missing converted files
    //missing download links

    [FirestoreData]
    public class File
    {
        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string DateUploaded { get; set; }

        [FirestoreProperty]
        public string FileName { get; set; }

        [FirestoreProperty]
        public string FileUrl { get; set; }

        [FirestoreProperty]
        public string DownloadLink { get; set; }
    }

}

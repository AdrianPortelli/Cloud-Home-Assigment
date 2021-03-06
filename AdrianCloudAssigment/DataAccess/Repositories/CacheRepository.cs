using Common;
using DataAccess.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private IDatabase myDatabase;

        public CacheRepository(string connectionString)
        {
            ConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(connectionString);

            myDatabase = multiplexer.GetDatabase();

        }

        public List<MenuItem> GetMenus()
        {
            var myList = myDatabase.StringGet("menuItems");

            /*myDatabase.KeyDelete("menuItems");*/

            if (myList.IsNullOrEmpty)
            {
                return new List<MenuItem>();

            }
            else
            {
                var myList_fromString = JsonConvert.DeserializeObject<List<MenuItem>>(myList);
                return myList_fromString;
            }
        }

        public void UpdateMenus(List<MenuItem> Items)
        {
            /*           string myList = JsonConvert.SerializeObject(Items);

                        myDatabase.StringSet("menuItems", myList);*/

            myDatabase.KeyDelete("menuItems");
            foreach (var item in Items)
            {
                AddMenu(item);
            }
        }

        public void AddMenu(MenuItem item)
        {
            var myList = GetMenus();
            myList.Add(item); 
            string myjsonstring = JsonConvert.SerializeObject(myList);
            myDatabase.StringSet("menuItems", myjsonstring);
        }

    }
}


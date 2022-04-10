using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interfaces
{
    public interface ICacheRepository
    {
        public List<MenuItem> GetMenus();

        public void UpdateMenus(List<MenuItem> Items);

        public void AddMenu(MenuItem item);

    }
}

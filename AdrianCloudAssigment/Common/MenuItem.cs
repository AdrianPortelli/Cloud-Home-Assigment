using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class MenuItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }

    public class MenuItemList
    {
        public List<MenuItem> menuitemsUpdate { get; set; }
    }
}

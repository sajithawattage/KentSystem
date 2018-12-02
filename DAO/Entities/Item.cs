using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO.Entities
{
    public class Item
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
    }
    public class Items : List<Item> { }
}

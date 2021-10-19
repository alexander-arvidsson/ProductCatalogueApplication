using System;
using System.Collections.Generic;

namespace ProductCatalogueApplication.Data
{
    public class Customer
    {
        //+Id : int
        public int Id;
        //+Name : string
        public string Name;
        //+Phone : string
        public string Phone;
        //+Email : string
        public string Email;
        //+Orders : List<Order>
        List<Order> orders = new List<Order>();
    }
}

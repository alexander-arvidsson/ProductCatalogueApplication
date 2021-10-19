using System;
using System.Collections.Generic;

namespace ProductCatalogueApplication.Data
{
    //Fixa service-klass f?r alla klasser.
    public class Customer
    {
        //+Id : int
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        //+Name : string
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        //+Phone : string
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        //+Email : string
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        //+Orders : List<Order> 
        //List<Order> orders = new List<Order>();

        private int _id;
        private string _name;
        private string _phone;
        private string _email;

    }
}

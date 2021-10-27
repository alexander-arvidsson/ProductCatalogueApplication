using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogueApplication.Data
{
    //Fixa service-klass f?r alla klasser.
    public class Customer
    {
        private int _id;
        private string _name;
        private string _phone;
        private string _email;
        private List<Order> _orders;

        public Customer()
        {
            Orders = new List<Order>();
        }

        //+Id : int
       
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        //+Name : string
        [Required(ErrorMessage = "A name is required.")]        //[StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 3)]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        //+Phone : string
        [Required(ErrorMessage = "A phone number is required.")]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        //+Email : string
        [Required(ErrorMessage = "An email is required.")]
        [StringLength(40, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        //+Orders : List<Order> 
        public List<Order> Orders
        {
            get { return _orders; }
            set { _orders = value; }
        }
    }
}

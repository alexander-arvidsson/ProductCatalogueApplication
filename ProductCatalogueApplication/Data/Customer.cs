using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogueApplication.Data
{
 
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

        /// <summary>
        /// The customer ID saved as an int.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        
        [Required(ErrorMessage = "A name is required.")]
        [StringLength(30, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        /// <summary>
        /// The customer name saved as a string.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        
        [Required(ErrorMessage = "A phone number is required.")]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        /// <summary>
        /// The customer's phone number saved as a string.
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }
        
        [Required(ErrorMessage = "An email is required.")]
        [StringLength(50, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        /// <summary>
        /// The customer's email saved as a string.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        /// <summary>
        /// A list of orders.
        /// </summary>
        public List<Order> Orders
        {
            get { return _orders; }
            set { _orders = value; }
        }
    }
}

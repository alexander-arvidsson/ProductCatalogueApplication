using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIntroduktionTest.Data
{
    public class Customer
    {
        private int id;
        private string name;
        private string email;
        private string phone;
        public List<Order> orders; //Kan initiera listan istället när klassen skapas

        public Customer()
        {
            orders = new List<Order>();

        }

        [Required]
        [Range(0, int.MaxValue)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required]        //[StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 3)]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        [StringLength(30, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Required]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        //[Required]
       // public List<Order> Orders
        //{
        //    get { return orders; }
        //    set { orders = value; }
       // }
    }
}

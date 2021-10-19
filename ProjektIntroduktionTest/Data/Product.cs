using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIntroduktionTest.Data
{
    public class Product
    {
        private int id;
        private string name;
        private string description;
        private double price;
        private int stock;
        private DateTime restockingDate;

        [Required]        //[StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 3)]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Required]
        [Range(0, int.MaxValue)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Required]
        [Range(0, double.MaxValue)]
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        [Required]
        [Range(0, int.MaxValue)] //tog bara ett random värde som säger att vi inte kan gå under 0 i stock eller över 9999
        public int Stock
        {
            get { return stock; }
            set { stock = value; }
        }

        [Required]
        public DateTime RestockingDate
        {
            get { return restockingDate; }
            set { restockingDate = value; }
        }



    }
}

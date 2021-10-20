﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data
{
    public class Product
    {
        private int _id;
        private string _name;
        private string _description;
        private double _price;
        private int _stock;
        private DateTime _restockingdate;

        [Required]
        [Range(1, int.MaxValue)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]        //[StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 3)]
        [StringLength(20, ErrorMessage = " {0} length must be between {2} and {1}. ", MinimumLength = 1)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Required]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [Required]
        [Range(0, double.MaxValue)]
        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        [Required]
        [Range(0, int.MaxValue)] //tog bara ett random värde som säger att vi inte kan gå under 0 i stock eller över 9999
        public int Stock
        {
            get { return _stock; }
            set { _stock = value; }
        }

        [Required]
        public DateTime RestockingDate
        {
            get { return _restockingdate; }
            set { _restockingdate = value; }
        }
    }
}
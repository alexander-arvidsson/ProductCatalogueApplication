using System;
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

        /// <summary>
        /// The unique product ID saved as an int.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required(ErrorMessage = "A product name is required.")]
        [StringLength(30, ErrorMessage = " {0} length must be between {2} and {1}.", MinimumLength = 1)]
        /// <summary>
        /// The product name saved as a string.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Required(ErrorMessage = "A product description is required.")]
        /// <summary>
        /// The product description saved as a string.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The product price saved as a double.
        /// </summary>

        [Range(0, double.MaxValue, ErrorMessage ="{0} can not be a negative value")]
        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        /// <summary>
        /// The product stock number saved as an int.
        /// </summary>
        [Range(0, 9999, ErrorMessage = " {0} can not be a negative value or more than 9999")]
        public int Stock
        {
            get { return _stock; }
            set { _stock = value; }
        }

        [Required]
        /// <summary>
        /// The product restocking date saved as a DateTime data type.
        /// </summary>
        public DateTime RestockingDate
        {
            get { return _restockingdate; }
            set { _restockingdate = value; }
        }
    }
}
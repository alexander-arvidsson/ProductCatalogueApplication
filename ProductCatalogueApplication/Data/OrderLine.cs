using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogueApplication.Data
{
    public class OrderLine
    {
        private int _id;
        private int _productid;
        private Product _product;
        private int _orderid;
        private Order _order;
        private int _quantity;

        /// <summary>
        /// The unique orderline stored in an int.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        /// <summary>
        /// The product ID stored in an int.
        /// </summary>
        public int ProductId
        {
            get { return _productid; }
            set { _productid = value; }
        }

        /// <summary>
        /// A product object of the Product class.
        /// </summary>
        public Product Product
        {
            get { return _product; }
            set { _product = value; }
        }

        /// <summary>
        /// The order ID stored in an int.
        /// </summary>
        public int OrderId
        {
            get { return _orderid; }
            set { _orderid = value; }
        }

        /// <summary>
        /// An order object of the Order class
        /// </summary>
        public Order Order
        {
            get { return _order; }
            set { _order = value; }
        }

        [Required(ErrorMessage = "A quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "At least 1 product is required.")]
        /// <summary>
        /// The quantity of a product stored in an int.
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }
}
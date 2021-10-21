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

        //[Required]
        //[Range(1, int.MaxValue)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        public int ProductId
        {
            get { return _productid; }
            set { _productid = value; }
        }

        //[Required]
        public Product Product
        {
            get { return _product; }
            set { _product = value; }
        }

        //[Required]
        public int OrderId
        {
            get { return _orderid; }
            set { _orderid = value; }
        }

        //[Required]
        public Order Order
        {
            get { return _order; }
            set { _order = value; }
        }

        [Required]
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }
}
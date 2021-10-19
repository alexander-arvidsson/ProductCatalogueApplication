using System;
using System.Collections.Generic;
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
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public int ProductId
        {
            get { return _productid; }
            set { _productid = value; }
        }
        public Product Product
        {
            get { return _product; }
            set { _product = value; }
        }
        public int OrderId
        {
            get { return _orderid; }
            set { _orderid = value; }
        }
        public Order Order
        {
            get { return _order; }
            set { _order = value; }
        }
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        public OrderLine()
        {

        }
    }
}
using System;
using System.Collections.Generic;
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
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }
        public int Stock
        {
            get { return _stock; }
            set { _stock = value; }
        }
        public DateTime RestockingDate
        {
            get { return _restockingdate; }
            set { _restockingdate = value; }
        }
        public Product()
        {

        }
    }
}
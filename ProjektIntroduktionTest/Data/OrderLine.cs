using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjektIntroduktionTest.Data
{
    public class OrderLine
    {
        private int id;
        private int productId;
        private Product product;
        private int orderId;
        private Order order;
        private int quanitity;

        [Required]
        [Range(0, int.MaxValue)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [Required]
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }

        [Required]
        public Product Product
        {
            get { return product; }
            set { product = value; }
        }

        [Required]
        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        [Required]
        public Order Order
        {
            get { return order; }
            set { order = value; }
        }

        [Required]
        public int Quantity
        {
            get { return quanitity; }
            set { quanitity = value; }
        }




    }
}

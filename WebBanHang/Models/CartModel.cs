using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHang.Context;

namespace WebBanHang.Models
{
    public class CartModel
    {
   
        public Product Product { get; set; }
        public int Quantity { get; set; }

        // Xuất đơn đã 
    }
    public class CartItemModel
    {
        public int itemId { get; set; }
        public int quantity { get; set; }
    }

}
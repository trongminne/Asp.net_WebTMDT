    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHang.Context;

namespace WebBanHang.Models
{
    public class HomeModel
    {
        public List<Product> listProduct { get; set; }
        public List<Category> listCategory { get; set; }
        public List<Brand> listBrand { get; set; }
        public Product Product { get; set; }
        public object ProductDetail { get; internal set; }
        public List<Category> Categories { get; internal set; }
    }
 

}
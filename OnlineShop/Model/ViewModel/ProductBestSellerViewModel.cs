using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class ProductBestSellerViewModel
    {
        public long ID { set; get; }
        public string Name { set; get; }
        public int Quantity { set; get; }
        public decimal TotalPrice { set; get; }
    }
}

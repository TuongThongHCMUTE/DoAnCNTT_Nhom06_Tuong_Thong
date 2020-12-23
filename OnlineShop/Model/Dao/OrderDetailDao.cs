using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class OrderDetailDao
    {
        private OnlineShopDbContext db = null;

        public OrderDetailDao()
        {
            db = new OnlineShopDbContext();
        }

        public bool Insert(OrderDetail detail)
        {
            try
            {
                db.OrderDetails.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteByProduct(long productID)
        {
            try
            {
                foreach (var orderDetail in db.OrderDetails.Where(x => x.ProductID == productID).ToList())
                {
                    db.OrderDetails.Remove(orderDetail);
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

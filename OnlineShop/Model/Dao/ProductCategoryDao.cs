using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace Model.Dao
{
    public class ProductCategoryDao
    {
        private OnlineShopDbContext db = null;

        public ProductCategoryDao()
        {
            db = new OnlineShopDbContext();
        }
        public ProductCategory ViewDetail(long id)
        {
            return db.ProductCategories.Find(id);
        }

        public List<ProductCategory> ListAll()
        {
            return db.ProductCategories.OrderBy(x => x.DisplayOrder).ToList();
        }

        public IEnumerable<ProductCategory> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<ProductCategory> model = db.ProductCategories;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.MetaTitle.Contains(searchString));
            }
            return model.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }

        public ProductCategory ViewDetail(int id)
        {
            return db.ProductCategories.Find(id);
        }

        public long Insert(ProductCategory entity)
        {
            db.ProductCategories.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public bool Update(ProductCategory entity)
        {
            try
            {
                var productCategory = db.ProductCategories.Find(entity.ID);
                productCategory.Name = entity.Name;
                productCategory.MetaTitle = entity.MetaTitle;
                productCategory.ParentID = entity.ParentID;
                productCategory.DisplayOrder = entity.DisplayOrder;
                productCategory.SeoTitle = entity.SeoTitle;
                productCategory.ModifiedBy = entity.ModifiedBy;
                productCategory.ModifiedDate = entity.ModifiedDate;
                productCategory.MetaKeywords = entity.MetaKeywords;
                productCategory.MetaDescriptions = entity.MetaDescriptions;
                productCategory.ShowOnHome = entity.ShowOnHome;
                productCategory.Status = entity.Status;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                foreach (Product product in db.Products.Where(x => x.CategoryID == id).ToList())
                {
                    new ProductDao().Delete(Convert.ToInt32(product.ID));
                }

                var productCategory = db.ProductCategories.Find(id);
                db.ProductCategories.Remove(productCategory);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangeStatus(long id)
        {
            var productCategory = db.ProductCategories.Find(id);

            // Nếu ngừng kinh doanh danh mục thì phải ngừng kinh doanh tất cả sản phẩm.
            if (productCategory.Status == false)
            {
                foreach (Product product in db.Products.Where(x => x.CategoryID == productCategory.ID).ToList())
                {
                    product.Status = false;
                    db.SaveChanges();
                }
            }

            productCategory.Status = !productCategory.Status;
            db.SaveChanges();

            return productCategory.Status == true;
        }
    }
}

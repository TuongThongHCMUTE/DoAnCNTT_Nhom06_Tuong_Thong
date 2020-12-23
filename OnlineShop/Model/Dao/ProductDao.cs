using Model.EF;
using Model.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ProductDao
    {
        private OnlineShopDbContext db = null;

        public ProductDao()
        {
            db = new OnlineShopDbContext();
        }

        public List<Product> ListAll()
        {
            return db.Products.Where(x => x.Status == true).OrderBy(x => x.CreatedDate).ToList();
        }

        public IEnumerable<Product> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.MetaTitle.Contains(searchString));
            }
            return model.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }

        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<ProductBestSellerViewModel> ListBestSellerProduct(int month, int year)
        {
            List<ProductBestSellerViewModel> model = new List<ProductBestSellerViewModel>();

            foreach (var product in db.Products)
            {
                ProductBestSellerViewModel viewModel = new ProductBestSellerViewModel();
                viewModel.ID = product.ID;
                viewModel.Name = product.Name;
                viewModel.Quantity = 0;

                foreach (var orderDetail in db.OrderDetails)
                {
                    if (orderDetail.ProductID == product.ID)
                    {
                        viewModel.Quantity += orderDetail.Quantity;
                    }
                }

                model.Add(viewModel);
            }

            return model.OrderByDescending(x => x.Quantity).Take(10).ToList();
        }

        public List<ProductViewModel> ListByCategoryID(long categoryID, ref int totalRecord, int pageIndex = 1, int pageSize = 2)
        {
            totalRecord = db.Products.Where(x => x.CategoryID == categoryID).Count();
            var model = from a in db.Products
                        join b in db.ProductCategories
                        on a.CategoryID equals b.ID
                        where a.CategoryID == categoryID
                        select new ProductViewModel()
                        {
                            CateMetaTitle = b.MetaTitle,
                            CateName = b.Name,
                            CreatedDate = a.CreatedDate,
                            ID = a.ID,
                            Images = a.Image,
                            Name = a.Name,
                            MetaTitle = a.MetaTitle,
                            Price = a.Price
                        };
            model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return model.ToList();
        }

        public List<Product> ListFeaturesProduct(int top)
        {
            return db.Products.Where(x => x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<Product> ListRelatedProduct(long productId)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.CategoryID == product.CategoryID).ToList();
        }

        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }

        public long Insert(Product entity)
        {
            db.Products.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public bool Update(Product entity)
        {
            try
            {
                var product = db.Products.Find(entity.ID);
                product.Name = entity.Name;
                product.Code = entity.Code;
                product.MetaTitle = entity.MetaTitle;
                product.Description = entity.Description;
                product.Image = entity.Image;
                product.MoreImages = entity.MoreImages;
                product.Price = entity.Price;
                product.PromotionPrice = entity.PromotionPrice;
                product.IncludeVAT = entity.IncludeVAT;
                product.Quantity = entity.Quantity;
                product.CategoryID = entity.CategoryID;
                product.Detail = entity.Detail;
                product.Warranty = entity.Warranty;
                product.ModifiedBy = entity.ModifiedBy;
                product.ModifiedDate = entity.ModifiedDate;
                product.MetaKeywords = entity.MetaKeywords;
                product.MetaDescriptions = entity.MetaDescriptions;
                product.ShowOnHome = entity.ShowOnHome;
                product.Status = entity.Status;
                product.TopHot = entity.TopHot;
                product.ViewCount = entity.ViewCount;

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
                var product = db.Products.Find(id);

                new OrderDetailDao().DeleteByProduct(product.ID);

                db.Products.Remove(product);
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
            var product = db.Products.Find(id);
            product.Status = !product.Status;
            db.SaveChanges();
            return product.Status == true;
        }

        public bool ChangeShowOnHome(long id)
        {
            var product = db.Products.Find(id);
            product.ShowOnHome = !product.ShowOnHome;
            db.SaveChanges();
            return product.ShowOnHome == true;
        }
    }
}

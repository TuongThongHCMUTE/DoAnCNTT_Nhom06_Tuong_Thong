using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(int mode)
        {

            List<Product> model = new List<Product>();
            // 0 List All
            // 1 Sản phẩm mới
            // 2 Sản phẩm nổi bật
            if (mode == 0)
            {
                model = new ProductDao().ListAll();
                ViewBag.Title = "Tất cả sản phẩm";
            }
            else if (mode == 1)
            {
                model = new ProductDao().ListNewProduct(100);
                ViewBag.Title = "Sản phẩm mới";
            }
            else if (mode == 2)
            {
                model = new ProductDao().ListFeaturesProduct(100);
                ViewBag.Title = "Sản phẩm nổi bật";
            }


            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            var model = new ProductCategoryDao().ListAll();
            return PartialView(model);
        }

        public ActionResult Category(long cateId, int page = 1, int pageSize = 2)
        {
            int totalRecord = 0;
            var category = new ProductCategoryDao().ViewDetail(cateId);
            ViewBag.Category = category;
            var model = new ProductDao().ListByCategoryID(cateId,ref totalRecord, page, pageSize);

            ViewBag.Total = totalRecord;
            ViewBag.Page = page;

            int maxPage = 5;
            int totalPage = 0;
            totalPage = (int)Math.Ceiling((double)totalRecord /pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            
            return View(model);
        }

        public ActionResult Detail(long id)
        {
            var product = new ProductDao().ViewDetail(id);
            ViewBag.Category = new ProductCategoryDao().ViewDetail(product.CategoryID);
            ViewBag.RelatedProducts = new ProductDao().ListRelatedProduct(id);
            return View(product);
        }
    }
}
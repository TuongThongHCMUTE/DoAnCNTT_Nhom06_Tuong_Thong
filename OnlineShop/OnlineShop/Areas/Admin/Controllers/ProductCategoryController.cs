using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {
        // GET: Admin/ProductCategory
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new ProductCategoryDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                productCategory.CreatedDate = DateTime.Now;
                productCategory.CreatedBy = Global.admin.Username;

                var dao = new ProductCategoryDao();

                long id = dao.Insert(productCategory);
                if (id > 0)
                {
                    SetAlert("Thêm danh mục sản phẩm thành công", "success");
                    return RedirectToAction("Index", "ProductCategory");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm danh mục sản phẩm không thành công");
                }
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var productCategory = new ProductCategoryDao().ViewDetail(id);
            return View(productCategory);
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                productCategory.ModifiedDate = DateTime.Now;
                productCategory.ModifiedBy = Global.admin.Username;

                var dao = new ProductCategoryDao();
                var result = dao.Update(productCategory);
                if (result)
                {
                    SetAlert("Sửa danh mục sản phẩm thành công", "success");
                    return RedirectToAction("Index", "ProductCategory");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật danh mục sản phẩm không thành công");
                }
            }
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            new ProductCategoryDao().Delete(id);
            return RedirectToAction("Index", "ProductCategory");
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ProductCategoryDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
    }
}
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
    public class ContentController : BaseController
    {
        // GET: Admin/Content
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new ContentDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Content content)
        {
            if (ModelState.IsValid)
            {
                content.CreatedDate = DateTime.Now;
                content.CreatedBy = Global.admin.Username;

                var dao = new ContentDao();

                long id = dao.Insert(content);
                if (id > 0)
                {
                    SetAlert("Thêm Content thành công", "success");
                    return RedirectToAction("Index", "Content");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Content không thành công");
                }
            }

            SetViewBag();
            return View();
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var dao = new ContentDao();
            var content = dao.GetByID(id);
            var model = dao.GetByID(id);

            SetViewBag(content.CategoryID);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Edit(Content model)
        {
            model.ModifiedDate = DateTime.Now;
            model.ModifiedBy = Global.admin.Username;

            var dao = new ContentDao();
            var result = dao.Update(model);
            if (result)
            {
                SetAlert("Sửa Content thành công", "success");
                return RedirectToAction("Index", "Content");
            }
            else
            {
                ModelState.AddModelError("", "Cập nhật user không thành công");
            }
            SetViewBag(model.CategoryID);
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            new ContentDao().Delete(id);
            return RedirectToAction("Index", "Content");
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ContentDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }


        public void SetViewBag(long? selectedId = null)
        {
            var dao = new CategoryDao();
            ViewBag.CategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        }
    }
}
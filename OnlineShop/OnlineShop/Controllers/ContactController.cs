using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        public ActionResult Index()
        {
            var model = new ContactDao().GetActiveContact();
            return View(model);
        }

        [HttpPost]
        public JsonResult Send(string name, string mobile, string address, string email, string content)
        {
            var feefBack = new Feedback()
            {
                Name = name,
                Email = email,
                CreatedDate = DateTime.Now,
                Phone = mobile,
                Content = content,
                Address = address
            };
            var id = new ContactDao().InsertFeedBack(feefBack);

            if(id>0)
            {
                return Json(new
                {
                    status = true
                });

                //Gởi mail cho quản trị
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }    
        }
    }
}
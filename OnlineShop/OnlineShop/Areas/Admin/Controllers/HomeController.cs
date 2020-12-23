using Model.Dao;
using Model.EF;
using Model.ViewModel;
using Newtonsoft.Json;
using OnlineShop.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            // Sản phẩm còn trong kho
            var model = new List<ThongKeProductCategory>();
            model = ThongKeDanhMucSanPham();

            ViewBag.TongSoLuongTonKho = model.Sum(x => x.SoLuongTonKho);
            ViewBag.ThongKeDonHang = ThongKeDonHang();

            // Top sản phẩm bán chạy
            var productDao = new ProductDao();
            var bestsellerModel = productDao.ListBestSellerProduct(DateTime.Now.Month, DateTime.Now.Year);

            List<DataPoint> productDataPoints = new List<DataPoint>();

            foreach (ProductBestSellerViewModel item in bestsellerModel)
            {
                productDataPoints.Add(new DataPoint(item.Name, item.Quantity));
            }

            productDataPoints = productDataPoints.OrderBy(x => x.Y).ToList();

            ViewBag.productDataPoints = JsonConvert.SerializeObject(productDataPoints);


            // Quản lý đơn hàng
            var orderDao = new OrderDao();
            var orderModel = orderDao.ListAll();

            List<DataPoint> orderDataPoints = new List<DataPoint>();

            int shippedStatus = orderModel.Where(x => x.Status == 1).Count();
            int shippingStatus = orderModel.Where(x => x.Status == 0).Count();
            int total_Order = shippedStatus + shippingStatus;

            orderDataPoints.Add(new DataPoint("Giao thành công", (int)((Convert.ToDouble(shippedStatus) / total_Order) * 100)));
            orderDataPoints.Add(new DataPoint("Đang giao hàng", (int)((Convert.ToDouble(shippingStatus) / total_Order) * 100)));

            ViewBag.orderDataPoints = JsonConvert.SerializeObject(orderDataPoints);

            return View(model);
        }

        public List<Order> ThongKeDonHang()
        {
            var orderDao = new OrderDao();
            return orderDao.ListAll();
        }

        public List<ThongKeProductCategory> ThongKeDanhMucSanPham()
        {
            var productDao = new ProductDao();
            var productModel = productDao.ListAll();

            var productCategoryDao = new ProductCategoryDao();
            var productCategoryModel = productCategoryDao.ListAll();

            List<ThongKeProductCategory> model = new List<ThongKeProductCategory>();
            foreach (var item in productCategoryModel)
            {
                ThongKeProductCategory thongKe = new ThongKeProductCategory();
                thongKe.TenDanhMuc = item.Name;
                int so_luong = 0;
                foreach (var p in productModel.Where(x => x.CategoryID == item.ID).ToList())
                {
                    try { so_luong += p.Quantity; } catch { }
                }
                thongKe.SoLuongTonKho = so_luong;
                model.Add(thongKe);
            }

            return model.OrderByDescending(x => x.SoLuongTonKho).Take(10).ToList();
        }

    }
}
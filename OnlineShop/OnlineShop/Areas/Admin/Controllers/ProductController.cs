using ClosedXML.Excel;
using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Admin/Product
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new ProductDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            SetViewBag();
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                product.CreatedBy = Global.admin.Username;

                var dao = new ProductDao();

                long id = dao.Insert(product);
                if (id > 0)
                {
                    SetAlert("Thêm sản phẩm thành công", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm sản phẩm không thành công");
                }
            }
            SetViewBag();
            return View();
        }

        public ActionResult Edit(int id)
        {
            var product = new ProductDao().ViewDetail(id);
            SetViewBag();
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                product.ModifiedDate = DateTime.Now;
                product.ModifiedBy = Global.admin.Username;

                var dao = new ProductDao();
                var result = dao.Update(product);
                if (result)
                {
                    SetAlert("Sửa sản phẩm thành công", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật sản phẩm không thành công");
                }
            }
            SetViewBag();
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            new ProductDao().Delete(id);
            return RedirectToAction("Index", "Product");
        }

        public void SetViewBag(long? selectedId = null)
        {
            var dao = new ProductCategoryDao();
            ViewBag.ProductCategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        }

        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = new ProductDao().ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }

        [HttpPost]
        public JsonResult ChangeShowOnHome(long id)
        {
            var result = new ProductDao().ChangeShowOnHome(id);
            return Json(new
            {
                status = result
            });
        }

        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[24]
            {
                new DataColumn("ID"),
                new DataColumn("Name"),
                new DataColumn("Code"),
                new DataColumn("MetaTittle"),
                new DataColumn("Description"),
                new DataColumn("Image"),
                new DataColumn("MoreImage"),
                new DataColumn("Price"),
                new DataColumn("PromotionPrice"),
                new DataColumn("IncludeVAT"),
                new DataColumn("Quantity"),
                new DataColumn("CategoryID"),
                new DataColumn("Detail"),
                new DataColumn("Warranty"),
                new DataColumn("CreatedDate"),
                new DataColumn("CreatedBy"),
                new DataColumn("ModifiedDate"),
                new DataColumn("ModifiedBy"),
                new DataColumn("MetaKeywords"),
                new DataColumn("MetaDescription"),
                new DataColumn("Status"),
                new DataColumn("ShowOnHome"),
                new DataColumn("TopHot"),
                new DataColumn("ViewCount")
            });

            var dao = new ProductDao();
            var model = dao.ListAll();

            foreach (var product in model)
            {
                dt.Rows.Add(
                    product.ID,
                    product.Name,
                    product.Code,
                    product.MetaTitle,
                    product.Description,
                    product.Image,
                    product.MoreImages,
                    product.Price,
                    product.PromotionPrice,
                    product.IncludeVAT,
                    product.Quantity,
                    product.CategoryID,
                    product.Detail,
                    product.Warranty,
                    product.CreatedDate,
                    product.CreatedBy,
                    product.ModifiedDate,
                    product.ModifiedBy,
                    product.MetaKeywords,
                    product.MetaDescriptions,
                    product.Status,
                    product.ShowOnHome,
                    product.TopHot,
                    product.ViewCount
                    );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelFile.xlsx");
                }
            }
        }

        [HttpPost]
        public ActionResult ImportFromExcel(HttpPostedFileBase postedFile)
        {
            if (ModelState.IsValid)
            {
                if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50)) // 50MB limit
                {
                    ModelState.AddModelError("postedFile", "Your file is to large. Maximum size allowed is 50MB !");
                }
                else
                {
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFile.FileName);
                    string extension = Path.GetExtension(postedFile.FileName);
                    postedFile.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //For Excel 97-03.  
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //For Excel 07 and above.  
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }

                    try
                    {
                        DataTable dt = new DataTable();
                        conString = string.Format(conString, filePath);

                        using (OleDbConnection connExcel = new OleDbConnection(conString))
                        {
                            using (OleDbCommand cmdExcel = new OleDbCommand())
                            {
                                using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                                {
                                    cmdExcel.Connection = connExcel;

                                    //Get the name of First Sheet.  
                                    connExcel.Open();
                                    DataTable dtExcelSchema;
                                    dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    connExcel.Close();

                                    //Read Data from First Sheet.  
                                    connExcel.Open();
                                    cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                    odaExcel.SelectCommand = cmdExcel;
                                    odaExcel.Fill(dt);
                                    connExcel.Close();
                                }
                            }
                        }

                        conString = ConfigurationManager.ConnectionStrings["Onlineshop"].ConnectionString;
                        using (SqlConnection con = new SqlConnection(conString))
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                            {
                                //Set the database table name.  
                                sqlBulkCopy.DestinationTableName = "Product";
                                con.Open();
                                sqlBulkCopy.WriteToServer(dt);
                                con.Close();
                                return Json("File uploaded successfully");
                            }
                        }
                    }

                    //catch (Exception ex)  
                    //{  
                    //    throw ex;  
                    //}  
                    catch (Exception e)
                    {
                        return Json("error" + e.Message);
                    }
                    //return RedirectToAction("Index");  
                }
            }
            //return View(postedFile);  
            return Json("no files were selected !");
        }
    }
}
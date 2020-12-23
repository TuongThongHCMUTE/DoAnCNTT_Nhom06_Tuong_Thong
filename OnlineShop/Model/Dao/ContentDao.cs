using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ContentDao
    {
        OnlineShopDbContext db = null;

        public ContentDao()
        {
            db = new OnlineShopDbContext();
        }
        public long Insert(Content entity)
        {
            db.Contents.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public bool Update(Content entity)
        {
            try
            {
                var content = db.Contents.Find(entity.ID);

                content.Name = entity.Name;
                content.MetaTitle = entity.MetaTitle;
                content.Description = entity.Description;
                content.Image = entity.Image;
                content.CategoryID = entity.CategoryID;
                content.Detail = entity.Detail;
                content.Warranty = entity.Warranty;
                content.ModifiedBy = entity.ModifiedBy;
                content.ModifiedDate = DateTime.Now;
                content.MetaDescriptions = entity.MetaDescriptions;
                content.Status = entity.Status;
                content.ShowOnHome = entity.ShowOnHome;
                content.TopHot = entity.TopHot;
                content.Tags = entity.Tags;

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Content GetByID(long id)
        {
            return db.Contents.Find(id);
        }
        public List<Content> GetListContent()
        {
            return db.Contents.Where(x => x.Status == true).ToList();
        }

        public IEnumerable<Content> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Content> model = db.Contents;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Description.Contains(searchString));
            }
            return model.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public bool ChangeStatus(long id)
        {
            var content = db.Contents.Find(id);
            content.Status = !content.Status;
            db.SaveChanges();
            return content.Status.Value;
        }

        public bool Delete(int id)
        {
            try
            {
                var content = db.Contents.Find(id);
                db.Contents.Remove(content);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

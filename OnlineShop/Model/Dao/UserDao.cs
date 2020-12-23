using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.EF;
using PagedList;

namespace Model.Dao
{
    public class UserDao
    {
        OnlineShopDbContext db = null;

        public UserDao()
        {
            db = new OnlineShopDbContext();
        }
        public long Insert(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
            return entity.ID;
        }

        public User InsertForFacebook(User entity)
        {
            var user = db.Users.SingleOrDefault(x => x.Username == entity.Username);
            if (user == null)
            {
                db.Users.Add(entity);
                db.SaveChanges();
                return entity;
            }
            else
            {
                return user;
            }    
        }

        public bool Update(User entity)
        {
            try
            {
                var user = db.Users.Find(entity.ID);
                if(!string.IsNullOrEmpty(entity.Password))
                {
                    user.Password = entity.Password;
                }    
                user.Name = entity.Name;
                user.Address = entity.Address;
                user.Phone = entity.Phone;
                user.Email = entity.Email;
                user.ModifiedBy = entity.ModifiedBy;
                user.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<User> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<User> model = db.Users;
            if(!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Username.Contains(searchString) || x.Name.Contains(searchString));
            }    
            return model.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public User GetById(string userName)
        {
            return db.Users.SingleOrDefault(x => x.Username == userName);
        }

        public User ViewDetail(int id)
        {
            return db.Users.Find(id);
        }

        public int Login(string userName, string passWord)
        {
            var result = db.Users.SingleOrDefault(x => x.Username == userName);

            if (result == null)
            {
                return 0;
            }    
            else
            {
                if(result.Status == false)
                {
                    return -1;
                }    
                else
                {
                    if (result.Password == passWord)
                    {
                        return 1;
                    }    
                    else
                    {
                        return -2;
                    }    
                }    
            }    
        }

        public bool ChangeStatus(long id)
        {
            var user = db.Users.Find(id);
            user.Status = !user.Status;
            db.SaveChanges();
            return user.Status;
        }
        public bool Delete(int id)
        {
            try
            {
                var user = db.Users.Find(id);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckUserName(string userName)
        {
            return db.Users.Count(x => x.Username == userName) > 0;
        }

        public bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.Email == email) > 0;
        }

    }
}

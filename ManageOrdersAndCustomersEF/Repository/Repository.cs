using ManageOrdersAndCustomersBL;
using ManageOrdersAndCustomersBL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersEF.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        AppDBContext dbcontext;
        DbSet<T> table;

        public Repository(AppDBContext _dbcontext)
        {
            dbcontext = _dbcontext;
            table = dbcontext.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }

        public void AddNew(T obj)
        {
            table.Add(obj);
        }

        public T GetByID(int id)
        {
            return table.Find(id);
        }

        public void Update(T obj)
        {
            table.Attach(obj);
            dbcontext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public void Delete(int id)
        {
            T obj = table.Find(id);
            table.Remove(obj);
        }




        //public int Save()
        //{
        //   return dbcontext.SaveChanges();
        //}

    }
}

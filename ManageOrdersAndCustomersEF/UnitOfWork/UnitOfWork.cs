using ManageOrdersAndCustomersBL.Repository;
using ManageOrdersAndCustomersBL.UnitOfWork;
using ManageOrdersAndCustomersEF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersEF.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly AppDBContext dbContext;
        public IRepository<T> UOFRepository { get; set; }
        
        public UnitOfWork(AppDBContext _dbContext)
        {
            dbContext = _dbContext;
            UOFRepository = new Repository<T>(_dbContext);
        }

        public int SubmitDBChages()
        {
            return dbContext.SaveChanges();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}

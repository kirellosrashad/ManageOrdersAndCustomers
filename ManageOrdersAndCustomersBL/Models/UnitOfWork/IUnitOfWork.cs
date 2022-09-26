using ManageOrdersAndCustomersBL;
using ManageOrdersAndCustomersBL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL.UnitOfWork
{
    public interface IUnitOfWork<T> : IDisposable where T : class
    {
        //public IRepository<Customer> customersRepository { get; set; }
        //public IRepository<Order> ordersRepository { get; set; }

        public IRepository<T> UOFRepository { get; set; }

        public int SubmitDBChages();
    }
}

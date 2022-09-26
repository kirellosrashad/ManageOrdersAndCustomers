
using ManageOrdersAndCustomersBL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL.Models.ViewModels
{
    public class CustomersVM
    {
        private readonly IUnitOfWork<Customer> customersUOF;
        private readonly IUnitOfWork<Order> ordersUOF;
        private readonly IUnitOfWork<Item> ItemsUOF;
        private readonly IUnitOfWork<Product> productUOF;

        public int CustomerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerFullName { get => FirstName + " " + LastName ;}
        public string Address { get; set; }
        public int PostalCode { get; set; }
        public List<OrderVM?> OredersVM { get; set; }

        public CustomersVM(IUnitOfWork<Customer> _customersUOF,
                                IUnitOfWork<Order> _ordersUOF,
                                IUnitOfWork<Item> _ItemsUOF,
                                IUnitOfWork<Product> _productUOF)
        {
            customersUOF = _customersUOF;
            ordersUOF = _ordersUOF;
            ItemsUOF = _ItemsUOF;
            productUOF = _productUOF;
        }

        public List<CustomersVM> GetAllCustomers()
        {
            List<CustomersVM> customers = new List<CustomersVM>();
            var data = customersUOF.UOFRepository.GetAll();
            if (data != null)
            {
                var customerList = from o in data
                                   select new
                                   {
                                       customerId = o.Id,
                                       FistName = o.FirstName,
                                       LastName = o.LastName,
                                       Address = o.Address,
                                       PostalCode = o.PostalCode
                                   };
                foreach (var cust in customerList)
                {
                    customers.Add(new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
                    {
                        CustomerId = cust.customerId,
                        FirstName = cust.FistName,
                        LastName= cust.LastName,
                        Address = cust.Address,
                        PostalCode = cust.PostalCode
                    });
                }
            }

            return customers;
        }

        public CustomersVM GetCustomerOrders(int customerId)
        {
            //CustomerOrdersVM customer = new CustomerOrdersVM();
            var data = customersUOF.UOFRepository.GetByID(customerId);
            if (data != null)
                return MapCustomerToCustomerVM(data);
            else
                return null;
        }

        public CustomersVM AddNewItem(CustomersVM customerVM)
        {
            Customer customer = MapCustomerVMToCustomer(customerVM);
            customersUOF.UOFRepository.AddNew(customer);
            int result = customersUOF.SubmitDBChages();
            if (customer.Id != 0)
                customerVM = MapCustomerToCustomerVM(customer);
            return customerVM;
        }

        public CustomersVM UpdateCustomer(CustomersVM customerVM, ref int result)
        {
            Customer customer = MapCustomerVMToCustomer(customerVM);
            customersUOF.UOFRepository.Update(customer);
            result = customersUOF.SubmitDBChages();
            MapCustomerToCustomerVM(customer);
            return customerVM;
        }

        public int DeleteCustomer(int customerId)
        {
            int result = 0;
            try
            {
                List<Order> Orders = ordersUOF.UOFRepository.GetAll().Where(x => x.CustomerId == customerId).ToList();
                if (Orders != null)
                {
                    foreach (var order in Orders)
                    {
                        result = OredersVM.FirstOrDefault().DeleteOrder(order.Id);
                    }
                }
                if (result != 0)
                {
                    customersUOF.UOFRepository.Delete(customerId);
                    result = customersUOF.SubmitDBChages();
                }
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #region Helper Functions

        private Customer MapCustomerVMToCustomer(CustomersVM customerVM)
        {
            Customer customer = null;
            if (customerVM != null)
            {
                customer = new Customer()
                {
                    Id = customerVM.CustomerId,
                    FirstName = customerVM.FirstName,
                    LastName = customerVM.LastName,
                    Address = customerVM.Address,
                    PostalCode = customerVM.PostalCode                
                };
            }
            return customer;
        }

        private CustomersVM MapCustomerToCustomerVM(Customer customer)
        {
            if (customer != null)
            {
                CustomerId = customer.Id;
                FirstName = customer.FirstName;
                LastName = customer.LastName;
                Address = customer.Address;
                PostalCode = customer.PostalCode;
                OredersVM = new OrderVM(ordersUOF, ItemsUOF, productUOF).GetOrdersPerCustomer(customer.Id).ToList();
            }
            return this;
        }
        #endregion
    }
}

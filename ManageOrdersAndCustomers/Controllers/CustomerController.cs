using ManageOrdersAndCustomersBL.Models.ViewModels;
using ManageOrdersAndCustomersBL;
using ManageOrdersAndCustomersBL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        //private readonly IRepository<Customer> unitOfWork.customersRepository;
        private readonly IUnitOfWork<Customer> customersUOF;
        private readonly IUnitOfWork<Order> ordersUOF;
        private readonly IUnitOfWork<Item> ItemsUOF;
        private readonly IUnitOfWork<Product> productUOF;
        private CustomersVM customerVM;

        public CustomerController(IUnitOfWork<Order> _ordersUOF,
                                        IUnitOfWork<Customer> _customersUOF,
                                        IUnitOfWork<Item> _ItemsUOF,
                                        IUnitOfWork<Product> _productUOF,
                                        CustomersVM _customerOrdersVM)
        {
            ordersUOF = _ordersUOF;
            customersUOF = _customersUOF;
            ItemsUOF = _ItemsUOF;
            productUOF = _productUOF;
            customerVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF);
        }

        [HttpGet("GetAllCustomers")]
        public ActionResult GetAllCustomers()
        {
            try
            {
                IEnumerable<CustomersVM> customerList = customerVM.GetAllCustomers().ToList();
                if (customerList == null)
                    return NotFound();

                return Ok(customerList);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("GetCustomer/{customerId}")]
        public ActionResult GetCustomer(int customerId)
        {
            try
            {
                customerVM = customerVM.GetCustomerOrders(customerId);
                if (customerVM == null)
                    return NotFound();

                return Ok(customerVM);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost("AddNewCustomer")]
        //[Route("Post")]
        public ActionResult AddNewCustomer(CustomersVM customerVM)
        {
            try
            {
                if (customerVM == null || string.IsNullOrEmpty(customerVM.FirstName))
                    return BadRequest();

                this.customerVM.AddNewItem(customerVM);
                
                if (this.customerVM.CustomerId != 0)
                    return base.Ok(this.customerVM);
                else
                    return base.StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateCustomer")]
        public ActionResult UpdateCustomer(CustomersVM customerVM)
        {
            try
            {
                if (customerVM == null || customerVM.CustomerId <= 0 || string.IsNullOrEmpty(customerVM.FirstName))
                    return BadRequest();

                int result = 0;
                this.customerVM.UpdateCustomer(customerVM, ref result);

                if(this.customerVM != null && result != 0)
                    return base.Ok(this.customerVM);
                else
                    return base.StatusCode((int)HttpStatusCode.NotModified);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("DeleteCustomer/{customerId}")]
        public ActionResult DeleteCustomer(int customerId)
        {
            try
            {
                if (customerId <= 0 || customerId <= 0)
                    return NotFound();

                int result = customerVM.DeleteCustomer(customerId);

                if (result != 0)
                    return Ok("Customer Deleted");
                else
                    return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}

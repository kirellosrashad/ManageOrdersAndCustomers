using ManageOrdersAndCustomersBL.Models.ViewModels;
using ManageOrdersAndCustomersBL;
using ManageOrdersAndCustomersBL.Constants;
using ManageOrdersAndCustomersBL.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        //private readonly IRepository<Order> unitOfWork.ordersRepository;
        private readonly IUnitOfWork<Customer> customersUOF;
        private readonly IUnitOfWork<Order> ordersUOF;
        private readonly IUnitOfWork<Item> ItemsUOF;
        private readonly IUnitOfWork<Product> productUOF;
        private readonly CustomersVM customersVM;
        private readonly OrderVM orderVM;

        public OrdersController(IUnitOfWork<Order> _ordersUOF,
                                        IUnitOfWork<Customer> _customersUOF,
                                        IUnitOfWork<Item> _ItemsUOF,
                                        IUnitOfWork<Product> _productUOF)
        {
            ordersUOF = _ordersUOF;
            customersUOF = _customersUOF;
            ItemsUOF = _ItemsUOF;
            productUOF = _productUOF;
            customersVM = new CustomersVM(customersUOF, ordersUOF,ItemsUOF, productUOF);
             orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF);
        }

        [HttpGet("GetAllOrdersForACustomer/{customerId}/{sort?}")]
        public ActionResult GetAllOrdersForACustomer(int customerId, SortingCustomerOrders sort = SortingCustomerOrders.desc)
        {
            try
            {
                var customerOrders = customersVM.GetCustomerOrders(customerId);
                if (customerOrders == null)
                    return NotFound();

                if (sort == SortingCustomerOrders.desc)
                    return Ok(customerOrders.OredersVM.OrderByDescending(x => x.OrderDate));
                else
                    return Ok(customerOrders.OredersVM.OrderBy(x => x.OrderDate));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("AddNewOrder")]
        public ActionResult AddNewOrder([FromBody] JObject objData)
        {
            try
            {
                var deserializedObj = objData.ToObject<OrderVM>();
                if (deserializedObj == null)
                    return BadRequest();

                #region To Initialize Objects with the reqdonly fields
                OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
                {
                    customerId = deserializedObj.customerId,
                    Id = deserializedObj.Id,
                    OrderDate = deserializedObj.OrderDate,
                    TotalPrice = deserializedObj.TotalPrice
                };

                List<ItemVM> itemsVM = new List<ItemVM>();
                foreach (var item in deserializedObj.ItemsVM)
                {
                    ItemVM itemVM = new ItemVM(ItemsUOF, productUOF)
                    {
                         Id = item.Id,
                         OrderId = item.OrderId,
                         Quantity = item.Quantity
                    };

                    ProductVM productVM = new ProductVM(productUOF)
                    {
                        Id = item.ProductVM.Id,
                        Name = item.ProductVM.Name,
                        Price = item.ProductVM.Price
                    };

                    itemVM.ProductVM = productVM;
                    itemsVM.Add(itemVM);
                }
                orderVM.ItemsVM = itemsVM;
                #endregion

                orderVM = orderVM.AddNewOrder(orderVM);
                
                if (orderVM.Id != 0)
                    return Ok(orderVM);
                else
                    return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateOrder")]
        public ActionResult UpdateOrder([FromBody] JObject objData)
        {
            try
            {
                var deserializedObj = objData.ToObject<OrderVM>();
                if (deserializedObj == null)
                    return BadRequest();

                #region To Initialize Objects with the reqdonly fields
                OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
                {
                    customerId = deserializedObj.customerId,
                    Id = deserializedObj.Id,
                    OrderDate = deserializedObj.OrderDate,
                    TotalPrice = deserializedObj.TotalPrice
                };

                List<ItemVM> itemsVM = new List<ItemVM>();
                foreach (var item in deserializedObj.ItemsVM)
                {
                    ItemVM itemVM = new ItemVM(ItemsUOF, productUOF)
                    {
                        Id = item.Id,
                        OrderId = item.OrderId,
                        Quantity = item.Quantity
                    };

                    ProductVM productVM = new ProductVM(productUOF)
                    {
                        Id = item.ProductVM.Id,
                        Name = item.ProductVM.Name,
                        Price = item.ProductVM.Price
                    };

                    itemVM.ProductVM = productVM;
                    itemsVM.Add(itemVM);
                }
                orderVM.ItemsVM = itemsVM;
                #endregion

                if (orderVM.Id == 0 || orderVM.Id <= 0)
                    return BadRequest();

                int result = 0;
                orderVM = orderVM.UpdateOrder(orderVM, ref result);

                if (orderVM.Id != 0 && result != 0)
                    return Ok(orderVM);
                else
                    return StatusCode((int)HttpStatusCode.NotModified);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("DeleteOrder/{orderId}")]
        public ActionResult DeleteOrder(int orderId)
        {
            try
            {
                if (orderId <= 0 || orderId <= 0)
                    return NotFound();

                int result = orderVM.DeleteOrder(orderId);

                if (result != 0)
                    return Ok("Order Deleted");
                else
                    return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



        #region Unused
        //[HttpGet("GetAllCustomersOrders/{sort?}")]
        //public ActionResult GetAllCustomersOrders(SortingCustomerOrders sort = SortingCustomerOrders.desc)
        //{
        //    try
        //    {
        //        IEnumerable<Order> orderList = ordersUOF.UOFRepository.GetAll();
        //        IEnumerable<Customer> customerList = customerOrdersUOF.UOFRepository.GetAll();
        //        IEnumerable<Item> itemsList = ItemsUOF.UOFRepository.GetAll();
        //        IEnumerable<Product> producList = productUOF.UOFRepository.GetAll();
        //        var data = from c in customerList
        //                   join o in orderList on c.Id equals o.CustomerId
        //                   join i in itemsList on o.Id equals i.OrderId
        //                   join p in producList on i.ProductId equals p.Id
        //                   orderby sort == SortingCustomerOrders.asc?  o.OrderDate : o.OrderDate descending
        //                   group new { c.Id, c.FirstName, c.LastName, c.Address, c.PostalCode }  by new
        //                   {
        //                       c.Id,
        //                       c.FirstName,
        //                       c.LastName,
        //                       c.Address,
        //                       c.PostalCode,
        //                       orderId = o.Id,
        //                       o.OrderDate,
        //                       o.TotalPrice,
        //                       itemId = i.Id ,
        //                       i.Quantity,
        //                       i.ProductId,
        //                       producId = p.Id,
        //                       productName = p.Name,
        //                       p.Price
        //                   } into g
        //                   select new  {
        //                       CustomerName = g.Key.FirstName + " " + g.Key.LastName,
        //                       Address = g.Key.Address,
        //                       PostalCode = g.Key.PostalCode,
        //                       OrderDate = g.Key.OrderDate,
        //                     TotalPrice = g.Key.TotalPrice,
        //                     ProductName = g.Key.productName,
        //                     ProductPrice = g.Key.Price,
        //                     Quantity = g.Key.Quantity
        //                   };
        //        if (data == null)
        //            return NotFound();

        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound();
        //    }
        //}

        //[HttpGet("GetAllOrdersForCustomerWithMemory/{customerId}/{sort?}")]
        //public ActionResult GetAllOrdersForCustomerWithMemory(int customerId, SortingCustomerOrders sort = SortingCustomerOrders.desc)
        //{
        //    try
        //    {
        //        Order order = ordersUOF.UOFRepository.GetByID(customerId);
        //        IEnumerable<Order> orderList = ordersUOF.UOFRepository.GetAll();
        //        IEnumerable<Customer> customerList = customersUOF.UOFRepository.GetAll();
        //        IEnumerable<Item> itemsList = ItemsUOF.UOFRepository.GetAll();
        //        IEnumerable<Product> producList = productUOF.UOFRepository.GetAll();
        //        var data = from c in customerList
        //                       //join o in orderList on c.Id equals o.CustomerId
        //                       //join i in itemsList on o.Id equals i.OrderId
        //                       //join p in producList on i.ProductId equals p.Id
        //                   where c.Id == customerId
        //                   //orderby o.OrderDate descending
        //                   //group c by c.Id into grp
        //                   select new
        //                   {
        //                       CustomerName = c.FirstName + " " + c.LastName,
        //                       Address = c.Address,
        //                       PostalCode = c.PostalCode,
        //                       Order = from o in orderList
        //                               where o.CustomerId == c.Id
        //                               orderby sort == SortingCustomerOrders.asc?  o.OrderDate : o.OrderDate descending
        //                               select new
        //                               {
        //                                   Id = o.Id,
        //                                   OrderDate = o.OrderDate,
        //                                   TotalPrice = o.TotalPrice,
        //                                   Items = from i in itemsList
        //                                           where i.OrderId == o.Id
        //                                           select new
        //                                           {
        //                                               Id = i.Id,
        //                                               Quantity = i.Quantity,
        //                                               Product = from p in producList
        //                                                         where p.Id == i.Id
        //                                                         select p
        //                                           }
        //                               }
        //                   };
        //        if (data == null)
        //            return NotFound();

        //        return Ok(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound();
        //    }
        //}
        #endregion
    }
}

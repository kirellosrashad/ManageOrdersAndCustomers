using ManageOrdersAndCustomersAPI.Controllers;
using ManageOrdersAndCustomersBL;
using ManageOrdersAndCustomersBL.Models.ViewModels;
using ManageOrdersAndCustomersBL.UnitOfWork;
using ManageOrdersAndCustomersEF;
using ManageOrdersAndCustomersEF.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace ManageOrdersAndCustomersTest
{
    [TestFixture]
    public class OrderTests
    {
        private readonly IConfigurationRoot configuration;
        private readonly DbContextOptions<AppDBContext> options;
        private readonly AppDBContext dbContext;
        private readonly UnitOfWork<Customer> customersUOF;
        private readonly UnitOfWork<Order> ordersUOF;
        private readonly UnitOfWork<Item> ItemsUOF;
        private readonly UnitOfWork<Product> productUOF;
        private readonly CustomersVM customersVM;
        private readonly OrderVM orderVM;

        public OrderTests()
        {
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");

            configuration = builder.Build();
            options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlServer(configuration.GetConnectionString("OrderManager"))
                .Options;

            dbContext = new AppDBContext(options);
            customersUOF = new UnitOfWork<Customer>(dbContext);
            ordersUOF = new UnitOfWork<Order>(dbContext);
            ItemsUOF = new UnitOfWork<Item>(dbContext);
            productUOF = new UnitOfWork<Product>(dbContext);
            customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF);
        }

        #region Test Get All Orders
        [Test]
        public void TestGetAll_ReturnNotNullValue()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int customerId = 1;
            var result = controller.GetAllOrdersForACustomer(customerId);
            Assert.NotNull(result);
        }

        [Test]
        public void TestGetAll_RetuenOKResult()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int customerId = 1;
            var result = controller.GetAllOrdersForACustomer(customerId);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        //[Test]
        //public void TestGetAll_ReturnAllItems()
        //{
        //    var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
        //    int customerId = 2;
        //    var result = controller.GetAllOrdersForACustomer(customerId);
        //    // Assert
        //    var data = ((ObjectResult)result).Value;
        //    Assert.Equals(3, data);  // change the number as per exiting number of items stored in DB
        //}
        #endregion

        #region Test Get Orders

        [Test]
        public void TestGetOrders_ReturnNotFound()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int id = 20;  // id which is not exist in in DB
            var result = controller.GetAllOrdersForACustomer(id);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TestGetOrders_ReturnOKResult()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int id = 2;
            var result = controller.GetAllOrdersForACustomer(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        //[Test]
        //public void TestGetOrders_ReturnSameRequestdID()
        //{
        //    var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
        //    int id = 2;
        //    var result = controller.GetAllOrdersForACustomer(id);
        //    Assert.IsInstanceOf<OkObjectResult>(result);
        //    Assert.AreEqual(id, ((OrderVM)((ObjectResult)result).Value).Id);
        //}

        #endregion

        #region TestAddNewOrder

        [Test]
        public void TestAddNewProduct_NullInput()
        {
            var controller = new OrdersController(ordersUOF, customersUOF,ItemsUOF,productUOF);
            var result = controller.AddNewOrder(null);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestAddNewProduct_LeakageInfo()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF,productUOF)
            {
               Id = 0,
               //customerId = 3,
               OrderDate= System.DateTime.Now,
               TotalPrice= 0
            };
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 1,
                Name = "P1",
                Price = 10
            };
            orderVM.ItemsVM = new List<ItemVM?>();
            orderVM.ItemsVM.Add(new ItemVM(ItemsUOF, productUOF)
            {
                Id = 0,
                OrderId = 0,
                Quantity = 2,
                ProductVM = productVM
            });
            JObject orderVMJson = (JObject)JToken.FromObject(orderVM);
            var result = controller.AddNewOrder(orderVMJson);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestAddNewProduct_NotSaved()
        {
            //tested by removing the call to DB
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
            {
                Id = 0,
                customerId = 3,
                OrderDate = System.DateTime.Now,
                TotalPrice = 0
            };
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 1,
                Name = "P1",
                Price = 10
            };
            orderVM.ItemsVM = new List<ItemVM?>();
            orderVM.ItemsVM.Add(new ItemVM(ItemsUOF, productUOF)
            {
                Id = 0,
                OrderId = 0,
                Quantity = 2,
                ProductVM = productVM
            });
            JObject orderVMJson = (JObject)JToken.FromObject(orderVM);
            var result = controller.AddNewOrder(orderVMJson);
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestAddNewProduct_ReturnOKResult()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
            {
                Id = 0,
                customerId = 3,
                OrderDate = System.DateTime.Now,
                TotalPrice = 0
            };
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 1,
                Name = "P3",
                Price = 30
            };
            orderVM.ItemsVM = new List<ItemVM?>();
            orderVM.ItemsVM.Add(new ItemVM(ItemsUOF, productUOF)
            {
                Id = 0,
                OrderId = 0,
                Quantity = 2,
                ProductVM = productVM
            });
            JObject orderVMJson = (JObject)JToken.FromObject(orderVM);
            var result = controller.AddNewOrder(orderVMJson);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region Test Update Product

        [Test]
        public void TestUpdateProduct_NullInput()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            var result = controller.UpdateOrder(null);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestUpdateProduct_LeakageInfo()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
            {
                Id = 17,
                //customerId = 1,
                OrderDate = System.DateTime.Now,
                TotalPrice = 0
            };
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 1,
                Name = "P2",
                Price = 20
            };
            orderVM.ItemsVM = new List<ItemVM?>();
            orderVM.ItemsVM.Add(new ItemVM(ItemsUOF, productUOF)
            {
                Id = 0,
                OrderId = 0,
                Quantity = 4,
                ProductVM = productVM
            });
            JObject orderVMJson = (JObject)JToken.FromObject(orderVM);
            var result = controller.UpdateOrder(orderVMJson);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }


        [Test]
        public void TestUpdateProduct_NotSaved()
        {
            //tested by removing the call to DB
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
            {
                Id = 17,
                customerId = 1,
                OrderDate = System.DateTime.Now,
                TotalPrice = 0
            };
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 1,
                Name = "P2",
                Price = 20
            };
            orderVM.ItemsVM = new List<ItemVM?>();
            orderVM.ItemsVM.Add(new ItemVM(ItemsUOF, productUOF)
            {
                Id = 65,
                OrderId = 17,
                Quantity = 4,
                ProductVM = productVM
            });
            JObject orderVMJson = (JObject)JToken.FromObject(orderVM);
            var result = controller.UpdateOrder(orderVMJson);
            Assert.AreEqual(StatusCodes.Status304NotModified, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestUpdateProduct_ReturnOKResult()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            OrderVM orderVM = new OrderVM(ordersUOF, ItemsUOF, productUOF)
            {
                Id = 17,
                customerId = 1,
                OrderDate = System.DateTime.Now,
                TotalPrice = 0
            };
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 5,
                Name = "P5",
                Price = 50
            };
            orderVM.ItemsVM = new List<ItemVM?>();
            orderVM.ItemsVM.Add(new ItemVM(ItemsUOF, productUOF)
            {
                Id = 65,
                OrderId = 17,
                Quantity = 4,
                ProductVM = productVM
            });
            JObject orderVMJson = (JObject)JToken.FromObject(orderVM);
            var result = controller.UpdateOrder(orderVMJson);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region Test Delete Product

        [Test]
        public void TestDeleteProduct_NotExist()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int id = 0;
            var result = controller.DeleteOrder(id);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TestDeleteProduct_NotDeleted()
        {
            // tested by removing the call to DB
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int id = 4;
            var result = controller.DeleteOrder(id);
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestDeleteProduct_ReturnOKResult()
        {
            var controller = new OrdersController(ordersUOF, customersUOF, ItemsUOF, productUOF);
            int id = 18;
            var result = controller.DeleteOrder(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion
    }
}
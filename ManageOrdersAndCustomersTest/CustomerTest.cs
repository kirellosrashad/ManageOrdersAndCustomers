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
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace ManageOrdersAndCustomersTest
{
    [TestFixture]
    public class CustomerTests
    {
        private readonly IConfigurationRoot configuration;
        private readonly DbContextOptions<AppDBContext> options;
        private readonly AppDBContext dbContext;
        private readonly UnitOfWork<Customer> customersUOF;
        private readonly UnitOfWork<Order> ordersUOF;
        private readonly UnitOfWork<Item> ItemsUOF;
        private readonly UnitOfWork<Product> productUOF;
        private readonly  CustomersVM customerVM;

        public CustomerTests()
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
            customerVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF);
        }


        #region Test Get All Customers
        [Test]
        public void TestGetAll_ReturnNotNullValue()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF,productUOF, customerVM);
            var result = controller.GetAllCustomers();
            Assert.NotNull(result);
        }

        [Test]
        public void TestGetAll_RetuenOKResult()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            var result = controller.GetAllCustomers();
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void TestGetAll_ReturnAllItems()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            var result = controller.GetAllCustomers();
            // Assert
            List<CustomersVM> allCustomers = ((ObjectResult)result).Value as List<CustomersVM>;
            Assert.AreEqual(6, allCustomers.Count);  // change the number as per exiting number of items stored in DB
        }
        #endregion

        #region Test Get Customer

        [Test]
        public void TestGetProduct_ReturnNotFound()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            int id = 20;  // id which is not exist in in DB
            var result = controller.GetCustomer(id);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TestGetProduct_ReturnOKResult()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            int id = 2;
            var result = controller.GetCustomer(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void TestGetProduct_ReturnSameRequestdID()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            int id = 2;
            var result = controller.GetCustomer(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(id, ((CustomersVM)((ObjectResult)result).Value).CustomerId);
        }

        #endregion

        #region TestAddNewCustomer

        [Test]
        public void TestAddNewCustomer_NullInput()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = null;
            var result = controller.AddNewCustomer(customersVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestAddNewCustomer_NotSaved()
        {
            //tested by removing the call to DB
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
            {
                CustomerId = 0,
                FirstName = "C4",
                LastName = "C4",
                Address = "A4",
                PostalCode = 4
            };
            var result = controller.AddNewCustomer(customersVM);
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, ((StatusCodeResult)result).StatusCode);
        }

        public void TestAddNewCustomerWithWrongInfo_NotSaved()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
            {
                CustomerId = 2, //==> should be 0
                FirstName = "C4",
                LastName = "C4",
                Address = "A4",
                PostalCode = 4
            };
            var result = controller.AddNewCustomer(customersVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestAddNewProduct_ReturnOKResult()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF,ordersUOF,ItemsUOF,productUOF)
            {
                CustomerId = 0,
                FirstName = "C4",
                LastName = "C4",
                Address = "A4",
                PostalCode = 4
            };
            var result = controller.AddNewCustomer(customersVM);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region Test Update Product

        [Test]
        public void TestUpdateProduct_NullInput()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = null;
            var result = controller.UpdateCustomer(customersVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestUpdateProduct_LeakageInfo()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
            {

            };
            var result = controller.UpdateCustomer(customersVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestUpdateProduct_NotFound()
        {
            //tested by removing the call to DB
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
            {
                CustomerId = 30,  // not found ID in DB
                FirstName = "C4",
                LastName = "C4",
                Address = "A4",
                PostalCode = 4
            };
            var result = controller.UpdateCustomer(customersVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestUpdateProduct_NotSaved()
        {
            //tested by removing the call to DB
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
            {
                CustomerId = 2,
                FirstName = "C4",
                LastName = "C4",
                Address = "A4",
                PostalCode = 4
            };
            var result = controller.UpdateCustomer(customersVM);
            Assert.AreEqual(StatusCodes.Status304NotModified, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestUpdateProduct_ReturnOKResult()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            CustomersVM customersVM = new CustomersVM(customersUOF, ordersUOF, ItemsUOF, productUOF)
            {
                CustomerId = 9,
                FirstName = "C8",
                LastName = "C8",
                Address = "A8",
                PostalCode = 8
            };
            var result = controller.UpdateCustomer(customersVM);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region Test Delete Product

        [Test]
        public void TestDeleteProduct_NotExist()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            int id = 0;
            var result = controller.DeleteCustomer(id);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TestDeleteProduct_NotDeleted()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            int id = 4;
            var result = controller.DeleteCustomer(id);
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestDeleteProduct_ReturnOKResult()
        {
            var controller = new CustomerController(ordersUOF, customersUOF, ItemsUOF, productUOF, customerVM);
            int id = 10;
            var result = controller.DeleteCustomer(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion
    }
}
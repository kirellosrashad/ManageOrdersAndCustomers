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
    public class ProductTests
    {
        private readonly IConfigurationRoot configuration;
        private readonly DbContextOptions<AppDBContext> options;
        private readonly AppDBContext dbContext;
        private readonly IUnitOfWork<Product> productUOF;

        public ProductTests()
        {
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");

            configuration = builder.Build();
            options = new DbContextOptionsBuilder<AppDBContext>()
                .UseSqlServer(configuration.GetConnectionString("OrderManager"))
                .Options;

            dbContext = new AppDBContext(options);
            productUOF = new UnitOfWork<Product>(dbContext);
        }

        #region Test Get All Customers
        [Test]
        public void TestGetAll_ReturnNotNullValue()
        {
            var controller = new ProductController( productUOF );
            var result = controller.GetAllProducts();
            Assert.NotNull(result);
        }

        [Test]
        public void TestGetAll_RetuenOKResult()
        {
            var controller = new ProductController(productUOF);
            var result = controller.GetAllProducts();
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void TestGetAll_ReturnAllItems()
        {
            var controller = new ProductController(productUOF);
            var result = controller.GetAllProducts();
            // Assert
            List<ProductVM> allCustomers = ((ObjectResult)result).Value as List<ProductVM>;
            Assert.AreEqual(11, allCustomers.Count);  // change the number as per exiting number of items stored in DB
        }
        #endregion

        #region Test Get Product

        [Test]
        public void TestGetProduct_ReturnNotFound()
        {
            var controller = new ProductController(productUOF);
            int id = 20;  // id which is not exist in in DB
            var result = controller.GetProduct(id);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TestGetProduct_ReturnOKResult()
        {
            var controller = new ProductController(productUOF);
            int id = 2;
            var result = controller.GetProduct(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public void TestGetProduct_ReturnSameRequestdID()
        {
            var controller = new ProductController(productUOF);
            int id = 2;
            var result = controller.GetProduct(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(id, ((ProductVM)((ObjectResult)result).Value).Id);
        }

        #endregion

        #region TestAddNewProduct

        [Test]
        public void TestAddNewProduct_NullInput()
        {
            var controller = new ProductController(productUOF);
            ProductVM productVM = null;
            var result = controller.AddNewProduct(productVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestAddNewProduct_LeakageInfo()
        {
            var controller = new ProductController(productUOF);
            ProductVM productVM = new ProductVM(productUOF)
            {

            };
            var result = controller.AddNewProduct(productVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestAddNewProduct_NotSaved()
        {
            //tested by removing the call to DB
            var controller = new ProductController(productUOF);
            ProductVM productVM = new ProductVM(productUOF)
            {
                Name = "P8",
                Price = 80,
            };
            var result = controller.AddNewProduct(productVM);
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestAddNewProduct_ReturnOKResult()
        {
            var controller = new ProductController(productUOF);
            ProductVM productVM = new ProductVM(productUOF)
            {
                Name = "P9",
                Price = 90,
            };
            var result = controller.AddNewProduct(productVM);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region Test Update Product

        [Test]
        public void TestUpdateProduct_NullInput()
        {
            var controller = new ProductController(productUOF);
            ProductVM productVM = null;
            var result = controller.UpdateProduct(productVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }

        [Test]
        public void TestUpdateProduct_LeakageInfo()
        {
            var controller = new ProductController(productUOF);
            ProductVM productVM = new ProductVM(productUOF)
            {
                
            };
            var result = controller.UpdateProduct(productVM);
            Assert.IsInstanceOf<BadRequestResult>((BadRequestResult)result);
        }


        [Test]
        public void TestUpdateProduct_NotSaved()
        {
            //tested by removing the call to DB
            var controller = new ProductController(productUOF);
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id = 17,
                Name = "P8",
                Price = 80,
            };
            var result = controller.UpdateProduct(productVM);
            Assert.AreEqual(StatusCodes.Status304NotModified, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestUpdateProduct_ReturnOKResult()
        {
            var controller = new ProductController(productUOF);
            ProductVM productVM = new ProductVM(productUOF)
            {
                Id=17,
                Name = "P10",
                Price = 100
            };
            var result = controller.UpdateProduct(productVM);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        #region Test Delete Product

        [Test]
        public void TestDeleteProduct_NotExist()
        {
            var controller = new ProductController(productUOF);
            int id = 0;
            var result = controller.DeleteProduct(id);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void TestDeleteProduct_NotDeleted()
        {
            var controller = new ProductController(productUOF);
            int id = 4;
            var result = controller.DeleteProduct(id);
            Assert.AreEqual(StatusCodes.Status422UnprocessableEntity, ((StatusCodeResult)result).StatusCode);
        }

        [Test]
        public void TestDeleteProduct_ReturnOKResult()
        {
            var controller = new ProductController(productUOF);
            int id = 17;
            var result = controller.DeleteProduct(id);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion
    }
}
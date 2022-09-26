using ManageOrdersAndCustomersBL;
using ManageOrdersAndCustomersBL.Models.ViewModels;
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
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork<Product> productUOF;
        private ProductVM productVM;

        public ProductController(IUnitOfWork<Product> _productUOF)
        {
            productUOF = _productUOF;
            productVM = new ProductVM(productUOF);
        }

        [HttpGet("GetAllProducts")]
        public ActionResult GetAllProducts()
        {
            try
            {
                IEnumerable<ProductVM> productList = productVM.GetAllProducts().ToList();
                if (productList == null)
                    return NotFound();

                return Ok(productList);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("GetProduct/{productId}")]
        public ActionResult GetProduct(int productId)
        {
            try
            {
                productVM = productVM.GetProduct(productId);
                if (productVM == null)
                    return NotFound();

                return Ok(productVM);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPost("AddNewProduct")]
        //[Route("Post")]
        public ActionResult AddNewProduct(ProductVM productVM)
        {
            try
            {
                if (productVM == null || string.IsNullOrEmpty(productVM.Name))
                    return BadRequest();

                this.productVM.AddNewProduct(productVM);

                if (this.productVM.Id != 0)
                    return Ok(this.productVM);
                else
                    return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut("UpdateProduct")]
        public ActionResult UpdateProduct(ProductVM productVM)
        {
            try
            {
                if (productVM == null || productVM.Id <= 0 || string.IsNullOrEmpty(productVM.Name))
                    return BadRequest();

                int result = 0;
                this.productVM.UpdateProduct(productVM, ref result);

                if (this.productVM != null && result != 0)
                    return Ok(this.productVM);
                else
                    return StatusCode((int)HttpStatusCode.NotModified);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("DeleteProduct/{productId}")]
        public ActionResult DeleteProduct(int productId)
        {
            try
            {
                if (productId <= 0)
                    return NotFound();

                int result = productVM.DeleteProduct(productId);

                if (result != 0)
                    return Ok("Product Deleted");
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

    


using ManageOrdersAndCustomersBL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL.Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        private readonly IUnitOfWork<Product> productUOF;

        public ProductVM(IUnitOfWork<Product> _productUOF)
        {
            productUOF = _productUOF;
        }

        public List<ProductVM> GetAllProducts()
        {
            List<ProductVM> products = new List<ProductVM>();
            var data = productUOF.UOFRepository.GetAll();
            if (data != null)
            {
                foreach (var product in data)
                {
                    products.Add(new ProductVM(productUOF)
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price
                    });
                }
            }
            
            return products;
        }

        public ProductVM GetProduct(int productId)
        {
            var product = productUOF.UOFRepository.GetByID(productId);
            if (product != null)
                return MapProductToProductVM(product);
            else 
                return null;
        }

        public ProductVM AddNewProduct(ProductVM productVM)
        {
            Product product = MapProductVMToProduct(productVM);
            productUOF.UOFRepository.AddNew(product);
            productUOF.SubmitDBChages();
            if(product.Id != 0)
                productVM = MapProductToProductVM(product);
            return productVM;
        }

        public ProductVM UpdateProduct(ProductVM productVM, ref int result )
        {
            Product product = MapProductVMToProduct(productVM);
            productUOF.UOFRepository.Update(product);
            result = productUOF.SubmitDBChages();
            MapProductToProductVM(product);
            return productVM;
        }

        public int DeleteProduct(int productId)
        {
            productUOF.UOFRepository.Delete(productId);
            return productUOF.SubmitDBChages();
        }

        #region Helper Functions
        private Product MapProductVMToProduct(ProductVM productVM)
        {
            Product product = null;
            if (productVM != null)
            {
                product = new Product()
                {
                    Id = productVM.Id,
                    Name = productVM.Name,
                    Price = productVM.Price
                };
            }
            return product;
        }

        private ProductVM MapProductToProductVM(Product product)
        {
            if (product != null)
            {
                Id = product.Id;
                Name = product.Name;
                Price = product.Price;
            }
            return this;
        }
        #endregion
    }
}

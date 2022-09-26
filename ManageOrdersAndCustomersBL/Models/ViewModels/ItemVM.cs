using ManageOrdersAndCustomersBL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL.Models.ViewModels
{
    public class ItemVM
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public ProductVM ProductVM { get; set; }

        private readonly IUnitOfWork<Item> ItemsUOF;
        //private readonly IUnitOfWork<Item> ItemsUOF;
        private readonly IUnitOfWork<Product> productUOF;
        public ItemVM(IUnitOfWork<Item> _ItemsUOF, IUnitOfWork<Product> _productUOF)
        {
            ItemsUOF = _ItemsUOF;
            productUOF = _productUOF;
        }
        
        public List<ItemVM> GetAllItems()
        {
            List<ItemVM> items = new List<ItemVM>();
            var data = ItemsUOF.UOFRepository.GetAll();
            if (data != null)
            {
                var itemList = from d in data
                            select new { 
                             Id = d.Id,
                             Quantity = d.Quantity,
                             OrderId = d.OrderId,
                             ProductVM = ProductVM.GetProduct(d.ProductId)
                            };
                foreach (var item in itemList)
                {
                    items.Add(new ItemVM(ItemsUOF, productUOF)
                    {
                        Id= item.Id,
                        Quantity = item.Quantity,
                        OrderId = item.OrderId,
                        ProductVM = item.ProductVM
                    });
                }
            }

            return items;
        }

        public List<ItemVM> GetItemsPerOrder(int orderId)
        {
            List<ItemVM> items = new List<ItemVM>();
            var data = ItemsUOF.UOFRepository.GetAll().AsQueryable().Where(x=>x.OrderId == orderId);
            if (data != null)
            {
                var itemList = from d in data
                               select new
                               {
                                   Id = d.Id,
                                   Quantity = d.Quantity,
                                   OrderId = d.OrderId,
                                   ProductId = d.ProductId
                                   //ProductVM = ProductVM.GetProduct(d.ProductId)
                               };
                foreach (var item in itemList)
                {
                    items.Add(new ItemVM(ItemsUOF, productUOF)
                    {
                        Id = item.Id,
                        Quantity = item.Quantity,
                        OrderId = item.OrderId,
                        ProductVM = new ProductVM(productUOF).GetProduct(item.ProductId)
                    });
                }
            }

            return items;
        }

        public ItemVM GetItem(int itemId)
        {
            //ItemVM itemVM = new ItemVM(orderItemsUOF);
            var item = ItemsUOF.UOFRepository.GetByID(itemId);
            if (item != null)
            {
                Id = item.Id;
                Quantity = item.Quantity;
                OrderId = item.OrderId;
                ProductVM = ProductVM.GetProduct(item.ProductId);

                return this;
            }

            return null;
        }


        public List<ItemVM> MatchItemsToItemsVM(List<Item> Items)
        {
            List<ItemVM> ItemsVM = new List<ItemVM>();
            
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    ItemVM itemVM = new ItemVM(ItemsUOF, productUOF)
                    {
                        Id = item.Id,
                        OrderId = item.OrderId,
                        Quantity = item.Quantity
                    };
                    ProductVM productVM = new ProductVM(productUOF);
                    productVM.GetProduct(item.ProductId);
                    itemVM.ProductVM = productVM;
                    ItemsVM.Add(itemVM);
                }
            }
            return ItemsVM;
        }

        public static List<Item> MatchItemsVMToItems(List<ItemVM> ItemsVM)
        {
            List<Item> Items = new List<Item>();
            if (ItemsVM != null)
            {
                foreach (var itemVM in ItemsVM)
                {
                    Items.Add(new Item()
                    {
                        Id = itemVM.Id,
                        OrderId = itemVM.OrderId,
                        Quantity = itemVM.Quantity,
                        ProductId = itemVM.ProductVM.Id
                    });
                }
            }
            return Items;
        }

        public void AddNewItem(Item item)
        {
            ItemsUOF.UOFRepository.AddNew(item);
        }

        public void UpdateItem(Item item)
        {
            ItemsUOF.UOFRepository.Update(item);
        }

        public void DeleteOrderItem(int itemId)
        {
            ItemsUOF.UOFRepository.Delete(itemId);
        }

        public int SubmitDBChanges()
        {
            return ItemsUOF.SubmitDBChages();
        }

    }
}

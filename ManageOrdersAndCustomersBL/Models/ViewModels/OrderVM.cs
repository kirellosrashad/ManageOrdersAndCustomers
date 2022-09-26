
using ManageOrdersAndCustomersBL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL.Models.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }
        [Required]
        public int customerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ItemVM?> ItemsVM { get; set; }

        
        private readonly IUnitOfWork<Order> orderUOF;
        private readonly IUnitOfWork<Item> ItemsUOF;
        private readonly IUnitOfWork<Product> productUOF;

        public OrderVM(IUnitOfWork<Order> _orderOrderUOF, IUnitOfWork<Item> _ItemsUOF, IUnitOfWork<Product> _productUOF)
        {
            orderUOF = _orderOrderUOF;
            ItemsUOF = _ItemsUOF;
            productUOF = _productUOF;
        }

        public List<OrderVM> GetAllOrders()
        {
            List<OrderVM> orders = new List<OrderVM>();
            var data = orderUOF.UOFRepository.GetAll();
            if (data != null)
            {
                var orderList = from o in data
                                select new
                                {
                                    Id = o.Id,
                                    OrderDate = o.OrderDate,
                                    TotalPrice = o.TotalPrice,
                                    ItemsVM = ItemsVM.SingleOrDefault().GetItemsPerOrder(o.Id)
                                };
                foreach (var order in orderList)
                {
                    orders.Add(new OrderVM(orderUOF, ItemsUOF, productUOF)
                    {
                         Id = order.Id,
                         OrderDate = order.OrderDate,
                         TotalPrice = order.TotalPrice,
                         ItemsVM = order.ItemsVM
                    });
                }
            }

            return orders;
        }

        public List<OrderVM> GetOrdersPerCustomer(int customerId)
        {
            List<OrderVM> orders = new List<OrderVM>();
            var data = orderUOF.UOFRepository.GetAll().AsQueryable().Where(x=>x.CustomerId == customerId);
            if (data != null)
            {
                var orderList = from o in data
                                select new
                                {
                                    Id = o.Id,
                                    customerId = customerId,
                                    OrderDate = o.OrderDate,
                                    TotalPrice = o.TotalPrice,
                                    //ItemsVM = ItemsVM.SingleOrDefault().GetItemsPerOrder(o.Id)
                                };
                foreach (var order in orderList)
                {
                    orders.Add(new OrderVM(orderUOF,ItemsUOF, productUOF)
                    {
                        Id = order.Id,
                        customerId = order.customerId,
                        OrderDate = order.OrderDate,
                        TotalPrice = order.TotalPrice,
                        ItemsVM = new ItemVM(ItemsUOF,productUOF).GetItemsPerOrder(order.Id).ToList()
                });
                }
            }

            return orders;
        }

        public OrderVM GetOrder(int orderId)
        {
            //OrderVM orderVM = new OrderVM(orderOrderUOF);
            var order = orderUOF.UOFRepository.GetByID(orderId);
            if (order != null)
            {
                Id = order.Id;
                OrderDate = order.OrderDate;
                TotalPrice = order.TotalPrice;
                //ItemsVM = (List<ItemVM>)(from i in order.Items
                //          select new {
                //              Id = i.Id,
                //              Quantity = i.Quantity,
                //              OrderId = i.OrderId,
                //              ProductVM = new ProductVM(productUOF).GetProduct(i.ProductId)
                //          });
                return this;
            }

            return null;
        }

        public OrderVM AddNewOrder(OrderVM orderVM)
        {
            Order order = MapOrderVMToOrder(orderVM);
            if (order != null)
            {
                orderUOF.UOFRepository.AddNew(order);
                int result = orderUOF.SubmitDBChages();
                if (result != 0 && order.Id != 0)
                {
                    //foreach (var item in order.Items)
                    //{
                    //    item.OrderId = order.Id;
                    //    //ItemsUOF.UOFRepository.AddNew(item);
                    //    ItemsVM.FirstOrDefault().AddNewItem(item);
                    //}
                     
                    orderVM = MapOrderToOrderVM(order);
                }
            }
            return orderVM;
        }

        public OrderVM UpdateOrder(OrderVM orderVM, ref int result)
        {
            Order order = MapOrderVMToOrder(orderVM);
            if (order != null)
            {
                foreach (var item in order.Items)
                {
                    ItemsVM.FirstOrDefault().UpdateItem(item);
                }
                result = ItemsVM.FirstOrDefault().SubmitDBChanges();
                orderUOF.UOFRepository.Update(order);
                result = orderUOF.SubmitDBChages();

                orderVM = MapOrderToOrderVM(order);
            }
            return orderVM;
        }


        public int DeleteOrder(int orderId)
        {
            orderUOF.UOFRepository.Delete(orderId);
            int result = orderUOF.SubmitDBChages();
            return result;
        }


        #region Helper Functions
        private Order MapOrderVMToOrder(OrderVM orderVM)
        {
            Order order = null;
            if (orderVM !=null)
            {
                order = new Order()
                {
                    Id = orderVM.Id,
                    CustomerId = orderVM.customerId,
                    OrderDate = orderVM.OrderDate,
                    TotalPrice = CalculateOrderTotalPrice(orderVM.ItemsVM), //orderVM.TotalPrice,
                    Items = ItemVM.MatchItemsVMToItems(orderVM.ItemsVM)
                };
            }
            return order;
        }

        private OrderVM MapOrderToOrderVM(Order order)
        {
            if (order != null)
            {
                Id = order.Id;
                customerId = order.CustomerId;
                OrderDate = order.OrderDate;
                ItemsVM = ItemsVM.FirstOrDefault().MatchItemsToItemsVM(order.Items);
                TotalPrice = CalculateOrderVMTotalPrice(order.Items); //orderVM.TotalPrice;
            }
            return this;
        }

        private decimal CalculateOrderTotalPrice(List<ItemVM> ItemsVM)
        {
            var data = (from i in ItemsVM
                       select  i.ProductVM.Price * i.Quantity).ToList();
            return data.Sum();
        }

        private decimal CalculateOrderVMTotalPrice(List<Item> Items)
        {
            var data = (from i in Items
                        select i.Product.Price * i.Quantity).ToList();
            return data.Sum();
        }
        #endregion
    }
}

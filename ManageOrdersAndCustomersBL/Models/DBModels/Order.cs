using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        //public int ItemId { get; set; }  //==> Navigation property to the parent item
        //[ForeignKey("ItemId")]
        public List<Item> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}

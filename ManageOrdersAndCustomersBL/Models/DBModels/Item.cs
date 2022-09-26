using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int OrderId { get; set; }

    }
}

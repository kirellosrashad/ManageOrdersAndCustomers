using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageOrdersAndCustomersBL
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        public int PostalCode { get; set; }
        public List<Order> Orders  { get; set; }
    }
}

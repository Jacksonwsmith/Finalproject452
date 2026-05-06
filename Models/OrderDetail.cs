using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAppSpring.Models
{
    public class OrderDetail
    {

        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order Order { get; set; }//navigational property

        public int BookId { get; set; }
        [ForeignKey("BookId")]
        [ValidateNever]
        public Book Book { get; set; }//navigational property

        public int Quantity { get; set; }

        public decimal Price { get; set; }

    }
}

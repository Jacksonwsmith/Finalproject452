using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAppSpring.Models
{
    public class CartItem
    {

        public int CartItemId { get; set; }

        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }//navigational property


        public string UserId { get; set; }


        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }//navigational porperty 

        public int Quantity { get; set; }

        [NotMapped]
        public decimal SubTotal { get; set; }


    }
}

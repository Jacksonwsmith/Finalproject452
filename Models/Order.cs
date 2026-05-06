using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAppSpring.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [ValidateNever]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }//navigational property

        public string CustomerName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public decimal OrderTotal { get; set; }

        [ValidateNever]
        public string OrderStatus { get; set; }

        [ValidateNever]
        public string PaymentStatus { get; set; }

        [ValidateNever]
        public DateOnly OrderDate { get; set; }

        public DateOnly? ShippingDate { get; set; }

        public string? Carrier { get; set; }

        public string? TrackingNumber { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

    }
}

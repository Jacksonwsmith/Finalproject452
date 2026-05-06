using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreAppSpring.Models
{
    public class Category
    {
       public int CategoryId { get; set; }

       [DisplayName("Category Name: "), Required(ErrorMessage = "Category Name MUST be provided")]
       public string Name { get; set; }

       [DisplayName("Category Description: "), Required(ErrorMessage = "Category Description MUST be provided")]
        public string Description { get; set; }//? will make it nullable


    }
}

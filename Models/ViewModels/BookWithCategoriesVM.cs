using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreAppSpring.Models.ViewModels
{
    public class BookWithCategoriesVM
    {
        public Book Book { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ListOfCategories { get; set; }






        }
}

using riusco_mvc.Models;

namespace riusco_mvc.ViewModels
{
    public class ProductViewModel
    {
        public UserDTO Owner { get; set; }
        public ProductDTO Product { get; set; }

        public ProductViewModel(UserDTO owner, ProductDTO product)
        {
            Owner = owner;
            Product = product;
        }
    }
}
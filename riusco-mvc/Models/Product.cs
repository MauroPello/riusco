using Microsoft.AspNetCore.Http;

namespace riusco_mvc.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }

        public Product(string name, string description, IFormFile image)
        {
            Name = name;
            Description = description;
            Image = image;
        }

        public Product(int id, string name, string description, IFormFile image)
        {
            Id = id;
            Name = name;
            Description = description;
            Image = image;
        }

        public Product() { }
    }
}
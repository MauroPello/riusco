using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace riusco_mvc.Models
{
    public class ProductDTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime LastUpdate { get; set; }
        [ForeignKey("Owner")]
        public int OwnerID { get; set; }
        public UserDTO Owner { get; set; }

        public ProductDTO(string name, string description, string image, DateTime lastUpdate, bool isAvailable, int ownerId)
        {
            Name = name;
            Description = description;
            Image = image;
            LastUpdate = lastUpdate;
            IsAvailable = isAvailable;
            OwnerID = ownerId;
        }
        
        public ProductDTO(int productId, string name, string description, string image, DateTime lastUpdate, bool isAvailable, int ownerId)
        {
            ProductID = productId;
            Name = name;
            Description = description;
            Image = image;
            LastUpdate = lastUpdate;
            IsAvailable = isAvailable;
            OwnerID = ownerId;
        }
        
        public ProductDTO() { }
    }
}
using Microsoft.AspNetCore.Http;

namespace riusco_mvc.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public IFormFile Image { get; set; }
        public int Balance { get; set; }

        public User(string name, string password, string email, IFormFile image, int balance, string city)
        {
            Name = name;
            Password = password;
            Image = image;
            Balance = balance;
            Email = email;
            City = city;
        }
        
        public User()
        { }
    }
}
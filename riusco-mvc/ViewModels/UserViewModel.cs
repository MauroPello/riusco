namespace riusco_mvc.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string City { get; set; }
        public string Email { get; set; }

        public UserViewModel(int userId, string name, string image, string city, string email)
        {
            UserId = userId;
            Name = name;
            Image = image;
            City = city;
            Email = email;
        }
    }
}
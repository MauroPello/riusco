namespace riusco_mvc.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Image { get; set; }
        public string ApiKey { get; set; }
        public string Outcome { get; set; }

        public ProfileViewModel(int userId, string name, string image, string apiKey, string email, string city)
        {
            UserId = userId;
            Name = name;
            Image = image;
            ApiKey = apiKey;
            Email = email;
            City = city;
        }
    }
}
namespace riusco_mvc.ViewModels
{
    public class BuyViewModel
    {
        public int UserId { get; set; }

        public BuyViewModel(int userId)
        {
            UserId = userId;
        }
    }
}
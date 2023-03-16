namespace riusco_mvc.ViewModels
{
    public class SellViewModel
    {
        public int UserId { get; set; }
        public string Outcome { get; set; }

        public SellViewModel(int userId)
        {
            UserId = userId;
        }
        
        public SellViewModel(int userId, string outcome)
        {
            UserId = userId;
            Outcome = outcome;
        }
        public SellViewModel(string outcome)
        {
            Outcome = outcome;
        }
    }
}